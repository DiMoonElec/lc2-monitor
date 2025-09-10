using System;
using System.Collections.Generic;
using DebugViews.DataClasses;

namespace LC2Monitor.BL
{
  public interface IView : IWatchVariablesProvider
  {
    event Action FormLoad;
    event Action<string> OnOpenProject;
    event Action<string> OnDumpSaveClicked;
    event Action OnDumpPrintClicked;
    event Action<object, EventArgs> OnConnectMenuOpening;
    event Action OnDisconnectClicked;
    event Action OnSendBinaryClicked;
    event Action OnRunClicked;
    event Action OnStopClicked;
    event Action OnStepClicked;
    event Action<DataElementBase> VariableViewerValueChanged;
    event Action OnRTCSyncWithPCClicked;
    event Action OnRTCSyncSetDateTimeClicked;
    event Action OnSaveProgramToFlashClicked;

    void UpdateStatus(string connectionStatus, string plcStatus);
    void UpdatePortsList(IEnumerable<string> ports);
    void UpdateVariablesList(IEnumerable<DataElementBase> variables);
    void UpdateLog(string message);
    void UpdateControlStates(bool isConnected, bool isProjectLoaded, ModelState state);
    void SetWatchVariables(VariablesDump variablesDump);
    void DisplayMetrics(int cycleValue, int duration, int durationMax);
    void DisplayRTCTime(DateTime dateTime);
  }
}
