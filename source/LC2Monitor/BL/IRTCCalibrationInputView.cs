using System;
using System.Windows.Forms;

namespace LC2Monitor.BL
{
  public interface IRTCCalibrationInputView : IDisposable
  {
    int CalibrationValue { get; }
    DialogResult ShowDialog();
  }
}
