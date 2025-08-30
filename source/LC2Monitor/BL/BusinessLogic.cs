using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DebugViews.DataClasses;
using LC2Monitor.MISC;
using LC2.LCCompiler;
using Serilog;
using System.Windows.Forms;

namespace LC2Monitor.BL
{
  public class BusinessLogic : IBusinessLogic
  {
    public static readonly DateTime RTCMinDateTime = new DateTime(2001, 1, 1, 0, 0, 0);
    public static readonly DateTime RTCMaxDateTime = new DateTime(2099, 12, 31, 23, 59, 59);

    public event Action<string> OnLogUpdated;
    public event Action<string, string> OnStatusbarUpdated;
    public event Action<IEnumerable<DataElementBase>> OnVariablesUpdated;
    public event Action<bool, bool, PLCStatus> OnUpdateControlStates;
    public event Action EnableVariablesPoll;
    public event Action DisableVariablesPoll;
    public event Action InstantVariablesPoll;
    public event Action<VariablesDump> VariablesDumpUpdated;
    public event Action<int, int, int> UpdateMetrics;
    public event Action<DateTime> DisplayRTCTime;

    private SerialPLCConnector plcConnector;
    //private TcpPLCConnector plcConnector;
    private PLCClientRequestManager plcRequestManager;
    private PLCClientRequests plcRequests;

    private LCProject _project = null;

    private bool _isConnected = false;
    private PLCStatus _plcStatus = PLCStatus.Disconnected;
    private bool _projectIsLoaded = false;

    object lockIsConnected = new object();
    object lockProjectIsLoaded = new object();
    object lockPLCStatus = new object();

    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private CancellationTokenSource _pollingCancellationTokenSource;

    private bool isConnected
    {
      get { return _isConnected; }
      set
      {
        lock (lockIsConnected)
        {
          Log.Information("(BL) IsConnected switch to '{0}'", value.ToString());

          if (value == true)
          {
            plcRequests.ConnectionError += PlcRequests_ConnectionError;
            plcRequests.PLCStatusChanged += PlcRequests_PLCStatusChanged;
            OnLogUpdated?.Invoke("Connected to PLC.");

            //Запуск цикла опроса
            _pollingCancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => pollingLoop(_pollingCancellationTokenSource.Token));
          }
          if (value == false)
          {
            plcRequests.ConnectionError -= PlcRequests_ConnectionError;
            plcRequests.PLCStatusChanged -= PlcRequests_PLCStatusChanged;
            OnLogUpdated?.Invoke("Disconnected from PLC.");
            plcStatus = PLCStatus.Disconnected;

            //Останов цикла опроса
            _pollingCancellationTokenSource?.Cancel();
          }

          _isConnected = value;
          UpdateStatus();
        }
      }
    }

    private PLCStatus plcStatus
    {
      get { return _plcStatus; }
      set
      {
        lock (lockPLCStatus)
        {
          Log.Information("(BL) PLCStatus switch to '{0}'", value.ToString());

          if (value == PLCStatus.Run)
          {
            //В состоянии RUN выполняем обновление переменных
            //если проект загружен
            if (projectIsLoaded && VariablesPoll == false)
            {
              VariablesPoll = true;
              EnableVariablesPoll?.Invoke();
            }
          }
          else if (value == PLCStatus.Stop)
          {
            //В состоянии STOP 
            //не обновляем переменные
            if (VariablesPoll == true)
            {
              VariablesPoll = false;
              DisableVariablesPoll?.Invoke();
            }
          }
          else if (value == PLCStatus.Cycle)
          {
            ;
          }
          else if (value == PLCStatus.Pause)
          {
            //В состоянии PAUSE
            //отключаем периодическое обновление переменных,
            if (VariablesPoll == true)
            {
              VariablesPoll = false;
              DisableVariablesPoll?.Invoke();
            }

            //Делаем одно обновление
            InstantVariablesPoll?.Invoke();
          }
          else if (value == PLCStatus.Disconnected)
          {
            //Если ПЛК отключен
            //то оснанавливаем обновление переменных
            if (VariablesPoll == true)
            {
              VariablesPoll = false;
              DisableVariablesPoll?.Invoke();
            }
          }

          _plcStatus = value;
          UpdateStatus();
        }
      }
    }

    private bool projectIsLoaded
    {
      get { return _projectIsLoaded; }
      set
      {
        lock (lockProjectIsLoaded)
        {
          Log.Information("(BL) ProjectIsLoaded switch to '{0}'", value.ToString());

          if (value == true)
          {
            //Если проект загрузили

            if (plcStatus == PLCStatus.Run)
            {
              //ПЛК находится в состоянии Run
              //то запускаем обновление переменных
              if (VariablesPoll == false)
              {
                VariablesPoll = true;
                EnableVariablesPoll?.Invoke();
              }
            }
            else if (plcStatus == PLCStatus.Pause)
            {
              //ПЛК находится в состоянии Pause
              //Делаем одно обновление переменных
              InstantVariablesPoll?.Invoke();
            }
          }
          else
          {
            //Если проект закрыли
            //то останавливаем обновление переменных
            if (VariablesPoll == true)
            {
              VariablesPoll = false;
              DisableVariablesPoll?.Invoke();
            }
          }
          _projectIsLoaded = value;
          UpdateStatus();
        }
      }
    }

    bool VariablesPoll { get; set; }

    public BusinessLogic()
    {
      plcConnector = new SerialPLCConnector();
      //plcConnector = new TcpPLCConnector();
      plcRequestManager = new PLCClientRequestManager(plcConnector);
      plcRequests = new PLCClientRequests(plcRequestManager);
    }

    public void Init()
    {
      isConnected = false;
    }

    private void PlcRequests_PLCStatusChanged(PLCStatus status)
    {
      plcStatus = status;
    }

    private void PlcRequests_ConnectionError()
    {
      isConnected = false;
      plcConnector.Close();
    }

    public void LoadProject(string filePath)
    {
      try
      {
        _project = new LCProject(filePath);
        UpdateLiveVariablesList();
        OnLogUpdated?.Invoke("Project loaded successfully.");
        projectIsLoaded = true;
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error loading project: {ex.Message}");
      }
    }

    public void ConnectToPlc(string portName)
    {
      RunTask(() =>
      {
        try
        {
          plcConnector.Open(portName, 115200);
          //plcConnector.Connect("192.168.88.233", 8888);
          plcRequests.Ping();
          var info = plcRequests.GetInformation();
          printInformation(info);
          isConnected = true;

          plcStatus = plcRequests.GetStatus();
        }
        catch (Exception ex)
        {
          plcConnector.Close();
          OnLogUpdated?.Invoke($"Error connecting to PLC: {ex.Message}");
        }
      });
    }

    public void DisconnectFromPlc()
    {
      try
      {
        plcConnector.Close();
        isConnected = false;
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error disconnecting: {ex.Message}");
      }
    }

    public void SendBinaryToPlc()
    {
      if (_project == null || !isConnected)
      {
        OnLogUpdated?.Invoke("Cannot send binary: no project loaded or not connected.");
        return;
      }

      RunTask(() =>
      {
        try
        {
          var app = File.ReadAllBytes(_project.OutputBinaryFilePath);
          OnLogUpdated?.Invoke($"Load binary program {_project.OutputBinaryFilePath}");

          plcRequests.PLCStop();
          OnLogUpdated?.Invoke("Upload start");
          plcRequests.PLCUploadApplication(app);
          OnLogUpdated?.Invoke("Upload complated");

          UpdateLiveVariablesList();
        }
        catch (Exception ex)
        {
          OnLogUpdated?.Invoke($"Error sending binary: {ex.Message}");
        }
      });

      /*
      try
      {
        // Код отправки бинарника через PLCClientRequests
        OnLogUpdated?.Invoke("Binary sent successfully.");
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error sending binary: {ex.Message}");
      }
      */
    }

    public void RunPlc()
    {
      if (plcStatus == PLCStatus.Stop || plcStatus == PLCStatus.Pause)
        RunTask(() =>
        {
          var result = plcRequests.PLCRun();


          switch (result)
          {
            case PLCRunStatus.LaunchOk:
              {
                var msg = "Program launch successful";
                OnLogUpdated?.Invoke(msg);
                Log.Information($"(BL) {msg}");
              }
              break;

            case PLCRunStatus.ContinueOk:
              {
                var msg = "Continue";
                OnLogUpdated?.Invoke(msg);
                Log.Information($"(BL) {msg}");
              }
              break;

            case PLCRunStatus.ProgramIsDamagedError:
              {
                var msg = "Launch Error: The program is damaged or missing";
                OnLogUpdated?.Invoke(msg);
                Log.Error($"(BL) {msg}");
              }
              break;

            case PLCRunStatus.InvalidOperationError:
              {
                var msg = "Launch Error: Operation Error";
                OnLogUpdated?.Invoke(msg);
                Log.Error($"(BL) {msg}");
              }
              break;
          }

        });
    }

    public void StopPlc()
    {
      // Логика остановки ПЛК
      if (plcStatus == PLCStatus.Run || plcStatus == PLCStatus.Pause)
        RunTask(() => plcRequests.PLCStop());
    }

    public void CyclePlc()
    {
      // Логика пошагового выполнения ПЛК
      if (plcStatus == PLCStatus.Run || plcStatus == PLCStatus.Stop || plcStatus == PLCStatus.Pause)
        RunTask(() =>
        {
          var result = plcRequests.PLCCycle();

          switch (result)
          {
            case PLCCycleStatus.ProgramIsDamagedError:
              {
                var msg = "Launch Error: The program is damaged or missing";
                OnLogUpdated?.Invoke(msg);
                Log.Error($"(BL) {msg}");
              }
              break;

            case PLCCycleStatus.InvalidOperationError:
              {
                var msg = "Launch Error: Operation Error";
                OnLogUpdated?.Invoke(msg);
                Log.Error($"(BL) {msg}");
              }
              break;
          }

        });

      //RunTask(() => plcRequests.PLCCycle());
    }

    public IEnumerable<string> GetAvailablePorts()
    {
      return SerialPLCConnector.GetPortNames();
    }


    private void UpdateStatus()
    {
      string connectionStatus = isConnected ? "Connected" : "Disconnected";
      OnStatusbarUpdated?.Invoke(connectionStatus, plcStatus.ToString());

      OnUpdateControlStates?.Invoke(isConnected, projectIsLoaded, plcStatus);
    }

    public void UpdateMemoryDump(IEnumerable<MemoryRequest> requests)
    {
      if (plcConnector.IsOpen() == false)
        return;

      RunTask(() =>
      {
        VariablesDump dump = new VariablesDump();

        foreach (var item in requests)
        {
          var startAddress = item.StartAddress;
          var size = item.Size;
          var data = plcRequests.PLCGetMemory((uint)startAddress, (uint)size);
          dump.AddChunk(data, startAddress);
        }

        VariablesDumpUpdated?.Invoke(dump);
      });
    }

    public void VariableViewerValueChanged(DataElementBase element)
    {
      if (plcConnector.IsOpen() == false)
        return;


      Log.Information("(BL) Variable '{0}@{1}' changed to '{2}'",
        element.Name,
        element.Address.ToString(),
        element.Value);

      var bytes = element.Bytes;
      if (bytes == null)
        return;

      plcRequests.PLCSetMemory((uint)element.Address, bytes);
    }

    public void RTCSync(DateTime dt)
    {
      if (plcStatus == PLCStatus.Disconnected)
        return;

      RunTask(() =>
      {
        try
        {
          // Вычислим разницу
          TimeSpan timeSpan = dt - RTCMinDateTime;

          // Получим общее количество секунд
          var totalSeconds = (uint)timeSpan.TotalSeconds;

          plcRequests.SetRTCTimestamp(totalSeconds);

          var msg = $"Time is synchronized: {dt.ToString()}";
          OnLogUpdated?.Invoke(msg);
          /*
          plcRequests.GetRuntimeMetrics(out int CyclePeriod,
            out int CycleDuration,
            out int CycleDurationMax);

          var msg = string.Format($"Runtime metrics:\r\n   Cycle Period: {CyclePeriod}\r\n   Cycle Duration: {CycleDuration}\r\n   Cycle Duration Max: {CycleDurationMax}");
          OnLogUpdated?.Invoke(msg);
          */
        }
        catch (Exception ex)
        {
          string msg = $"Error disconnecting: {ex.Message}";
          Log.Error(msg);
          OnLogUpdated?.Invoke(msg);
        }
      });
    }

    public void RTCSyncWithPC()
    {
      RTCSync(DateTime.Now);
    }

    public void GetMetrics()
    {
      if (plcStatus == PLCStatus.Disconnected)
        return;

      RunTask(() =>
      {
        try
        {
          plcRequests.GetRuntimeMetrics(out int CyclePeriod,
            out int CycleDuration,
            out int CycleDurationMax);

          UpdateMetrics?.Invoke(CyclePeriod, CycleDuration, CycleDurationMax); 

          /*
          //var msg = string.Format($"Runtime metrics:\r\n   Cycle Period: {CyclePeriod}\r\n   Cycle Duration: {CycleDuration}\r\n   Cycle Duration Max: {CycleDurationMax}");
          string msg = "Runtime metrics:\r\n";
          msg += $"   Cycle Period: {CyclePeriod}ms\r\n";

          if (CycleDuration != 0)
            msg += $"   Cycle Duration: {CycleDuration}ms\r\n";
          else
            msg += $"   Cycle Duration: <1ms\r\n";

          if (CycleDurationMax != 0)
            msg += $"   Cycle Duration Max: {CycleDurationMax}ms";
          else
            msg += $"   Cycle Duration Max: <1ms";

          OnLogUpdated?.Invoke(msg);
          */
        }
        catch (Exception ex)
        {
          string msg = $"Error disconnecting: {ex.Message}";
          Log.Error(msg);
          OnLogUpdated?.Invoke(msg);
        }
      });
    }

    public void LCVMPrintDump()
    {
      if (plcStatus == PLCStatus.Stop
        || plcStatus == PLCStatus.Pause
        || plcStatus == PLCStatus.Exception)
      {
        RunTask(() =>
        {
          try
          {
            Log.Information("(BL) Start get LCVM Dump");
            var dump = GetLCVMDump();

            var dumpReport = dump.ToString();

            OnLogUpdated?.Invoke(dumpReport);
            Log.Information($"(BL) {dumpReport}");
          }
          catch (Exception ex)
          {
            string msg = $"Error disconnecting: {ex.Message}";
            Log.Error(msg);
            OnLogUpdated?.Invoke(msg);
          }
        });
      }
    }

    public void LCVMSaveDump(string file)
    {
      if (plcStatus == PLCStatus.Stop
        || plcStatus == PLCStatus.Pause
        || plcStatus == PLCStatus.Exception)
      {
        Log.Information("(BL) Start save LCVM Dump");
        var dump = GetLCVMDump();
        dump.Save(file);

        var dumpReport = dump.ToString();

        OnLogUpdated?.Invoke($"Dump saved to file: '{file}'");
        OnLogUpdated?.Invoke(dumpReport);

        Log.Information($"(BL) Dump saved to file '{file}'");
        Log.Information($"(BL) {dumpReport}");
      }
    }

    public void SaveProgramToFlash()
    {
      if (plcStatus == PLCStatus.Stop
        || plcStatus == PLCStatus.Pause
        || plcStatus == PLCStatus.Exception)
      {
        RunTask(() =>
        {
          try
          {
            Log.Information("(BL) Save program to flash...");

            var result = plcRequests.SaveProgramToFlash();

            string msg = "";
            switch (result)
            {
              case SaveProgramResult.OK:
                msg = "Program saved successfully.";
                break;

              case SaveProgramResult.EmptyProgram:
                msg = "Error: The program is empty.";
                break;

              case SaveProgramResult.ProgramTooLarge:
                msg = "Error: The program exceeds the flash memory size.";
                break;

              case SaveProgramResult.FlashEraseFailed:
                msg = "Error: Failed to erase flash memory.";
                break;

              case SaveProgramResult.FlashWriteFailed:
                msg = "Error: Failed to write to flash memory.";
                break;

              case SaveProgramResult.OperationNotAllowed:
                msg = "Error: The operation is not allowed in the current PLC state.";
                break;

              case SaveProgramResult.UnknownError:
              default:
                msg = "Error: Unknown error occurred.";
                break;
            }

            OnLogUpdated?.Invoke(msg);
            Log.Information($"(BL) {msg}");
          }
          catch (Exception ex)
          {
            string msg = $"Error disconnecting: {ex.Message}";
            Log.Error(msg);
            OnLogUpdated?.Invoke(msg);
          }
        });
      }
    }

    void UpdateLiveVariablesList()
    {
      var variables = MemoryAllocationInformationDeserializer.Deserialize(_project.OutputMemoryAllocationReportFilePath);
      OnVariablesUpdated?.Invoke(variables);
    }

    private LCVMDump GetLCVMDump()
    {
      try
      {
        plcRequests.GetRegisterValue(out uint PC, out uint FP,
          out short OperationSP, out short ReturnSP,
          out byte Except);

        uint[] operationStack = plcRequests.GetStackValue(LCVMStack.Operation, OperationSP + 1);
        uint[] returnStack = plcRequests.GetStackValue(LCVMStack.Return, ReturnSP + 1);

        List<byte> memory = new List<byte>();
        /*
        for (uint adr = 0; ; adr += 128)
        {
          var chunk = plcRequests.PLCGetMemory(adr, 128);
          memory.AddRange(chunk);
          if (chunk.Length < 128)
            break;
        }
        */
        return new LCVMDump(PC, OperationSP, ReturnSP, FP, Except,
          operationStack, returnStack, memory.ToArray());
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error creating dump: {ex.Message}");
        return null;
      }
    }

    private void RunTask(Action taskFunc)
    {
      Task.Run(() =>
      {
        semaphore.WaitAsync();
        try
        {
          taskFunc();
        }
        finally
        {
          semaphore.Release();
        }
      });
    }

    private void printInformation(PLCInfo info)
    {
      if (info == null)
      {
        OnLogUpdated?.Invoke("PLC info is null");
        return;
      }

      OnLogUpdated?.Invoke($"PLC: SN={info.SerialNumber}, FW={info.MajorVersion}.{info.MinorVersion}.{info.PatchVersion}");
    }

    private async Task pollingLoop(CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        try
        {
          var timestamp = plcRequests.GetRTCTimestamp();
          DateTime startDate = RTCMinDateTime; //Начало эпохи RTC
          DateTime dateTime = startDate.AddSeconds(timestamp); // Добавляем к ней timestamp
          DisplayRTCTime?.Invoke(dateTime);

          // Задержка между циклами
          await Task.Delay(250, token);
          //OnLogUpdated?.Invoke("Info: cycle 250ms");
        }
        catch (OperationCanceledException)
        {
          // Прерывание цикла
          break;
        }
        catch (Exception ex)
        {
          // логирование ошибок
          OnLogUpdated?.Invoke($"Error: {ex.Message}");
        }
      }
    }
  }

}
