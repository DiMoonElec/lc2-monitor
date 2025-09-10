using System;
using System.Collections.Generic;
using DebugViews.DataClasses;
using LC2Monitor.BL.Message;

namespace LC2Monitor.BL
{
  public partial class BusinessLogic : IBusinessLogic
  {
    public static readonly DateTime RTCMinDateTime = new DateTime(2001, 1, 1, 0, 0, 0);
    public static readonly DateTime RTCMaxDateTime = new DateTime(2099, 12, 31, 23, 59, 59);

    public event Action<string> OnLogUpdated;
    public event Action<string, string> OnStatusbarUpdated;
    public event Action<IEnumerable<DataElementBase>> OnVariablesUpdated;
    public event Action<bool, bool, ModelState> OnUpdateControlStates;
    public event Action EnableVariablesPoll;
    public event Action DisableVariablesPoll;
    public event Action InstantVariablesPoll;
    public event Action<VariablesDump> VariablesDumpUpdated;
    public event Action<int, int, int> UpdateMetrics;
    public event Action<DateTime> DisplayRTCTime;

    private readonly IWatchVariablesProvider WatchVariablesProvider;

    private PeriodicTask _liveWatchTask;
    private PeriodicTask _rtcUpdateTask;

    private Model _Model;

    public BusinessLogic(IWatchVariablesProvider watchVariablesProvider)
    {
      WatchVariablesProvider = watchVariablesProvider ?? throw new ArgumentNullException(nameof(watchVariablesProvider));

      _Model = new Model();
      _Model.plcConnector = new SerialPLCConnector();
      _Model.plcRequestManager = new PLCClientRequestManager(_Model.plcConnector);
      _Model.plcRequests = new PLCClientRequests(_Model.plcRequestManager);

      _Model.plcRequests.ConnectionError += PlcRequests_ConnectionError;
      _Model.plcRequests.PLCStatusChanged += PlcRequests_PLCStatusChanged;

      _liveWatchTask = new PeriodicTask(() => LiveWatchUpdate(), 1000);
      _liveWatchTask.OnError += _liveWatchTask_OnError;

      _rtcUpdateTask = new PeriodicTask(() => RTCUpdate(), 250);
      _rtcUpdateTask.OnError += _rtcUpdateTask_OnError;

      MessageLoopHandleStart();
    }

    public void Init()
    {
      UpdateUi();
    }

    public void Dispose()
    {
      _liveWatchTask?.Dispose();
      _rtcUpdateTask?.Dispose();
      MessageLoopHandleStop();
    }

    private void UpdateUi()
    {
      string connectionStatus = _Model.State != ModelState.Disconnected ? "Connected" : "Disconnected";
      OnStatusbarUpdated?.Invoke(connectionStatus, _Model.State.ToString());

      OnUpdateControlStates?.Invoke(_Model.State != ModelState.Disconnected,
        _Model.Project != null, _Model.State);
    }

    private void PlcRequests_PLCStatusChanged(PLCStatus status)
    {
      PostMessage(new StateChangedDeviceMessage(status));
    }

    private void PlcRequests_ConnectionError()
    {
      PostMessage(new ConnectionLostDeviceMessage());
    }

    public void LoadProject(string filePath)
    {
      PostMessage(new LoadProjectUIMessage(filePath));
    }

    public void ConnectToPlc(string portName)
    {
      PostMessage(new ConnectUIMessage(portName));
    }

    public void DisconnectFromPlc()
    {
      PostMessage(new CloseUIMessage());
    }

    public void SendBinaryToPlc()
    {
      PostMessage(new UploadExeFileUIMessage());
    }

    public void RunPlc()
    {
      PostMessage(new RuntimeRunUIMessage());
    }

    public void StopPlc()
    {
      PostMessage(new RuntimeStopUIMessage());
    }

    public void CyclePlc()
    {
      PostMessage(new RuntimeCycleUIMessage());
    }

    public IEnumerable<string> GetAvailablePorts()
    {
      return SerialPLCConnector.GetPortNames();
    }

    public void VariableViewerValueChanged(DataElementBase element)
    {
      PostMessage(new VariableViewerValueChangedUIMessage(element));
    }

    public void RTCSync(DateTime dt)
    {
      PostMessage(new RTCSetUIMessage(dt));
    }

    public void RTCSyncWithPC()
    {
      PostMessage(new RTCSyncWithPCUIMessage());
    }

    public void LCVMPrintDump()
    {
      PostMessage(new RuntimePrintDumpUIMessage());
    }

    public void LCVMSaveDump(string file)
    {
      throw new NotImplementedException();
    }

    public void SaveProgramToFlash()
    {
      PostMessage(new FlashExeUIMessage());
    }

    private void RTCUpdate()
    {
      var timestamp = _Model.plcRequests.GetRTCTimestamp();
      DateTime startDate = RTCMinDateTime; //Начало эпохи RTC
      DateTime dateTime = startDate.AddSeconds(timestamp); // Добавляем к ней timestamp
      DisplayRTCTime?.Invoke(dateTime);
    }

    private void LiveWatchUpdate()
    {
      VariablesDump dump = new VariablesDump();

      //Список переменных, которые необходимо обновить
      var variables = WatchVariablesProvider.GetWatchVariables();
      //Формируем список запросов к областям памяти
      var requests = DataElementHelper.BuildMemoryRequests(variables, 0);

      foreach (var item in requests)
      {
        var startAddress = item.StartAddress;
        var size = item.Size;
        var data = _Model.plcRequests.PLCGetMemory((uint)startAddress, (uint)size);
        dump.AddChunk(data, startAddress);
      }

      VariablesDumpUpdated?.Invoke(dump);

      //Обновляем метрики
      _Model.plcRequests.GetRuntimeMetrics(out int CyclePeriod,
           out int CycleDuration,
           out int CycleDurationMax);

      UpdateMetrics?.Invoke(CyclePeriod, CycleDuration, CycleDurationMax);
    }

    private void _rtcUpdateTask_OnError(PeriodicTask sender, Exception ex)
    {
      //Вызывается при возникновении исключения в RTCUpdate
    }

    private void _liveWatchTask_OnError(PeriodicTask sender, Exception ex)
    {
      //Вызывается при возникновении исключения в LiveWatchUpdate
    }
  }
}
