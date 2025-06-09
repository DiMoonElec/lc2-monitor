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
    event Action<bool, bool, PLCStatus> OnUpdateControlStates;
    event Action<IEnumerable<DataElementBase>> OnVariablesUpdated;
    event Action EnableVariablesPoll;
    event Action DisableVariablesPoll;
    event Action InstantVariablesPoll;
    event Action<VariablesDump> VariablesDumpUpdated;
    event Action<int, int, int> UpdateMetrics;

    void Init();
    void LoadProject(string filePath);
    void ConnectToPlc(string portName);
    void DisconnectFromPlc();
    void SendBinaryToPlc();
    void RunPlc();
    void StopPlc();
    void CyclePlc();
    IEnumerable<string> GetAvailablePorts();
    void UpdateMemoryDump(IEnumerable<MemoryRequest> requests);
    void VariableViewerValueChanged(DataElementBase element);
    void LCVMSaveDump(string file);
    void LCVMPrintDump();
    void GetMetrics();
    void RTCSync();
    void SaveProgramToFlash();
  }

}
