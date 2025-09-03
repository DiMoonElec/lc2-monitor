using System;
using System.Windows.Forms;

namespace LC2Monitor.BL
{
  public interface IDateTimeInputView : IDisposable
  {
    DateTime SelectedDateTime { get; }
    DialogResult ShowDialog();
  }
}
