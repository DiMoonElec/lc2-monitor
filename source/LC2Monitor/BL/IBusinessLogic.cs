using System;
using System.Collections.Generic;
using DebugViews.DataClasses;

namespace LC2Monitor.BL
{
  public enum GUIControl
  {
    ButtonRun,
    ButtonStop,
    ButtonCycle,
    ButtonConnect,
    ButtonDisconnect,
    ButtonProjectLoad,
    ButtonProjectClose,
  };

  public interface IBusinessLogic
  {

    event Action<string> OnLogUpdated;
    event Action<string, string> OnStatusbarUpdated;
    event Action<bool, bool, ModelState> OnUpdateControlStates;
    event Action<IEnumerable<DataElementBase>> OnVariablesUpdated;
    event Action<VariablesDump> VariablesDumpUpdated;
    event Action<int, int, int> UpdateMetrics;
    event Action<DateTime> DisplayRTCTime;

    void Init();
    void LoadProject(string filePath);
    void ConnectToPlc(string portName);
    void DisconnectFromPlc();
    void SendBinaryToPlc();
    void RunPlc();
    void StopPlc();
    void CyclePlc();
    IEnumerable<string> GetAvailablePorts();
    void VariableViewerValueChanged(DataElementBase element);
    void LCVMSaveDump(string file);
    void LCVMPrintDump();
    void RTCSyncWithPC();
    void RTCSync(DateTime dt);
    void SaveProgramToFlash();
  }
}
