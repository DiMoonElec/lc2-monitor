using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LC2.LCCompiler;
using LC2Monitor.BL.Message;
using LC2Monitor.MISC;
using Serilog;

namespace LC2Monitor.BL
{
  public partial class BusinessLogic : IBusinessLogic
  {
    private readonly BlockingCollection<BLMessage> _queue = new BlockingCollection<BLMessage>();
    CancellationTokenSource MessageLoopCancellationToken;

    private void StateChanged(ModelState state)
    {
      if (_Model.State == ModelState.Disconnected && state != ModelState.Disconnected)
        ConnectedAction();

      switch (state)
      {
        case ModelState.Disconnected: DisconnectedAction(); break;
        case ModelState.Stop: StoppedAction(); break;
        case ModelState.Run: RunningAction(); break;
        case ModelState.Cycle: CyclingAction(); break;
        case ModelState.Pause: PausedAction(); break;
        case ModelState.Exception: ExceptionOccurredAction(); break;
      }

      Log.Debug("(BL) PLCStatus switch to '{0}'", state.ToString());

      _Model.State = state;
    }

    private void MessageLoopHandleStart()
    {
      MessageLoopCancellationToken = new CancellationTokenSource();
      Task.Run(() =>
      {
        try
        {
          foreach (var msg in _queue.GetConsumingEnumerable(MessageLoopCancellationToken.Token))
          {
            MessageHandle(msg);
            UpdateUi();
          }
        }
        catch (OperationCanceledException)
        {
        }

      }, MessageLoopCancellationToken.Token);
    }

    private void MessageLoopHandleStop()
    {
      MessageLoopCancellationToken?.Cancel();
      _queue.CompleteAdding();
    }

    private void PostMessage(BLMessage msg) { _queue.Add(msg); }

    private void MessageHandle(BLMessage message)
    {
      if (message is ConnectUIMessage connectUIMessage)
        ConnectUIMessageHandle(connectUIMessage);
      else if (message is CloseUIMessage)
        CloseUIMessageHandle();
      else if (message is LoadProjectUIMessage loadProjectUIMessage)
        LoadProjectUIMessageHandle(loadProjectUIMessage.Path);
      else if (message is UploadExeFileUIMessage)
        UploadExeFileUIMessageHandle();
      else if (message is RuntimeRunUIMessage)
        RuntimeRunUIMessageHandle();
      else if (message is RuntimeStopUIMessage)
        RuntimeStopUIMessageHandle();
      else if (message is RuntimeCycleUIMessage)
        RuntimeCycleUIMessageHandle();
      else if (message is FlashExeUIMessage)
        FlashExeUIMessageHandle();
      else if (message is RTCSetUIMessage rtcSetUIMessage)
        RTCSetUIMessageHandle(rtcSetUIMessage);
      else if (message is RTCSyncWithPCUIMessage)
        RTCSyncWithPCUIMessageHandle();
      else if (message is RuntimePrintDumpUIMessage)
        RuntimePrintDumpUIMessageHandle();
      else if (message is StateChangedDeviceMessage stateChangedDeviceMessage)
        StateChangedDeviceMessageHandle(stateChangedDeviceMessage.State);
      else if (message is ConnectionLostDeviceMessage)
        ConnectionLostDeviceMessageHandle();
      else if (message is VariableViewerValueChangedUIMessage variableViewerValueChangedUIMessage)
        VariableViewerValueChangedUIMessageHandle(variableViewerValueChangedUIMessage);
    }

    private void VariableViewerValueChangedUIMessageHandle(VariableViewerValueChangedUIMessage variableViewerValueChangedUIMessage)
    {
      if (_Model.State == ModelState.Disconnected)
        return;

      var element = variableViewerValueChangedUIMessage.Element;

      Log.Debug("(BL) Variable '{0}@{1}' changed to '{2}'",
      element.Name,
        element.Address.ToString(),
      element.Value);

      var bytes = element.Bytes;
      if (bytes == null)
        return;

      _Model.plcRequests.PLCSetMemory((uint)element.Address, bytes);
    }

    private void ConnectionLostDeviceMessageHandle()
    {
      _Model.plcConnector.Close();
      StateChanged(ModelState.Disconnected);
      OnLogUpdated?.Invoke("Connection Lost");
    }

    private void LoadProjectUIMessageHandle(string path)
    {
      try
      {
        _Model.Project = new LCProject(path);
        UpdateLiveVariablesList();

        OnLogUpdated?.Invoke("Project loaded successfully");
        Log.Debug("(BL) Project loaded successfully");

        ProjectOpenedAction();
      }
      catch (Exception ex)
      {
        string msg = $"Error loading project: {ex.Message}";

        OnLogUpdated?.Invoke(msg);
        Log.Debug($"(BL) {msg}");
      }
    }

    private void StateChangedDeviceMessageHandle(PLCStatus state)
    {
      ModelState modelState;

      switch (state)
      {
        case PLCStatus.Stop: modelState = ModelState.Stop; break;
        case PLCStatus.Run: modelState = ModelState.Run; break;
        case PLCStatus.Cycle: modelState = ModelState.Cycle; break;
        case PLCStatus.Pause: modelState = ModelState.Pause; break;
        case PLCStatus.Exception: modelState = ModelState.Exception; break;
        default: throw new NotImplementedException();
      }

      StateChanged(modelState);
    }

    private LCVMDump GetLCVMDump()
    {
      try
      {
        _Model.plcRequests.GetRegisterValue(out uint PC, out uint FP,
          out short OperationSP,
          out short ReturnSP,
          out byte Except);

        uint[] operationStack = _Model.plcRequests.GetStackValue(LCVMStack.Operation, OperationSP + 1);
        uint[] returnStack = _Model.plcRequests.GetStackValue(LCVMStack.Return, ReturnSP + 1);

        List<byte> memory = new List<byte>();

        return new LCVMDump(PC, OperationSP, ReturnSP, FP, Except,
          operationStack, returnStack, memory.ToArray());
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error creating dump: {ex.Message}");
        return null;
      }
    }

    private void RuntimePrintDumpUIMessageHandle()
    {
      if (_Model.State == ModelState.Stop
        || _Model.State == ModelState.Pause
        || _Model.State == ModelState.Exception)
      {
        try
        {
          Log.Debug("(BL) Start get LCVM Dump");
          var dump = GetLCVMDump();
          if (dump != null)
          {
            var dumpReport = dump.ToString();

            OnLogUpdated?.Invoke(dumpReport);
            Log.Debug($"(BL) {dumpReport}");
          }
        }
        catch (Exception ex)
        {
          string msg = $"Error disconnecting: {ex.Message}";
          Log.Error(msg);
          OnLogUpdated?.Invoke(msg);
        }
      }
    }

    private void FlashExeUIMessageHandle()
    {
      if (_Model.State == ModelState.Stop
       || _Model.State == ModelState.Pause
       || _Model.State == ModelState.Exception)
      {
        try
        {
          Log.Debug("(BL) Save program to flash...");

          var result = _Model.plcRequests.SaveProgramToFlash();

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
          Log.Debug($"(BL) {msg}");
        }
        catch (Exception ex)
        {
          string msg = $"Error disconnecting: {ex.Message}";
          Log.Error(msg);
          OnLogUpdated?.Invoke(msg);
        }
      }
    }

    private void RTCSyncWithPCUIMessageHandle()
    {
      RTCSetDateTime(DateTime.Now);
    }


    private void RTCSetUIMessageHandle(RTCSetUIMessage rtcSetUIMessage)
    {
      RTCSetDateTime(rtcSetUIMessage.DT);
    }

    private void RTCSetDateTime(DateTime dt)
    {
      if (_Model.State == ModelState.Disconnected)
        return;

      try
      {
        // Вычислим разницу
        TimeSpan timeSpan = dt - RTCMinDateTime;

        // Получим общее количество секунд
        var totalSeconds = (uint)timeSpan.TotalSeconds;

        _Model.plcRequests.SetRTCTimestamp(totalSeconds);

        var msg = $"Time is synchronized: {dt.ToString()}";
        OnLogUpdated?.Invoke(msg);
      }
      catch (Exception ex)
      {
        string msg = $"Error disconnecting: {ex.Message}";
        Log.Error(msg);
        OnLogUpdated?.Invoke(msg);
      }
    }


    private void RuntimeCycleUIMessageHandle()
    {
      // Логика пошагового выполнения ПЛК
      if (_Model.State == ModelState.Run
       || _Model.State == ModelState.Stop
       || _Model.State == ModelState.Pause)
      {
        var result = _Model.plcRequests.PLCCycle();

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
      }
    }

    private void RuntimeStopUIMessageHandle()
    {
      // Логика остановки ПЛК
      if (_Model.State == ModelState.Run || _Model.State == ModelState.Pause)
        _Model.plcRequests.PLCStop();
    }

    private void RuntimeRunUIMessageHandle()
    {
      if (_Model.State == ModelState.Stop || _Model.State == ModelState.Pause)
      {
        var result = _Model.plcRequests.PLCRun();

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
              Log.Debug($"(BL) {msg}");
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
      }
    }

    private void UploadExeFileUIMessageHandle()
    {
      if (_Model.Project == null || _Model.State == ModelState.Disconnected)
      {
        OnLogUpdated?.Invoke("Cannot send binary: no project loaded or not connected.");
        return;
      }

      try
      {
        var app = File.ReadAllBytes(_Model.Project.OutputBinaryFilePath);
        OnLogUpdated?.Invoke($"Load binary program {_Model.Project.OutputBinaryFilePath}");

        _Model.plcRequests.PLCStop();
        OnLogUpdated?.Invoke("Upload start");
        _Model.plcRequests.PLCUploadApplication(app);
        OnLogUpdated?.Invoke("Upload complated");

        UpdateLiveVariablesList();
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error sending binary: {ex.Message}");
      }
    }

    private void CloseUIMessageHandle()
    {
      try
      {
        _Model.plcConnector.Close();
        StateChanged(ModelState.Disconnected);
        OnLogUpdated?.Invoke("Connection Closed");
      }
      catch (Exception ex)
      {
        OnLogUpdated?.Invoke($"Error disconnecting: {ex.Message}");
      }
    }

    private void ConnectUIMessageHandle(ConnectUIMessage connectUIMessage)
    {
      try
      {
        _Model.plcConnector.Open(connectUIMessage.Port, 115200);
        //plcConnector.Connect("192.168.88.233", 8888);
        _Model.plcRequests.Ping();
        var info = _Model.plcRequests.GetInformation();
        printInformation(info);

        StateChangedDeviceMessageHandle(_Model.plcRequests.GetStatus());
      }
      catch (Exception ex)
      {
        _Model.plcConnector.Close();
        OnLogUpdated?.Invoke($"Error connecting to PLC: {ex.Message}");
      }
    }

    private void UpdateLiveVariablesList()
    {
      var variables = MemoryAllocationInformationDeserializer.Deserialize(
        _Model.Project.OutputMemoryAllocationReportFilePath);
      OnVariablesUpdated?.Invoke(variables);
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

  }
}
