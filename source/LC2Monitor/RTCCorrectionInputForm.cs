using System;
using System.Windows.Forms;
using LC2Monitor.BL;

namespace LC2Monitor
{
  public partial class RTCCorrectionInputForm : Form, IRTCCalibrationInputView
  {
    public int CalibrationValue { get; protected set; }

    IRTCCalibrationCalculator _calibrationCalculator;

    public RTCCorrectionInputForm(int initValue, IRTCCalibrationCalculator calibrationCalculator)
    {
      InitializeComponent();

      this.StartPosition = FormStartPosition.CenterParent;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;

      _calibrationCalculator = calibrationCalculator ?? throw new ArgumentNullException(nameof(calibrationCalculator));
      CalibrationValue = initValue;

      CalibrationValueUpDown.ValueChanged += CalibrationValueUpDown_ValueChanged;
      CalibrationValueUpDown.Maximum = _calibrationCalculator.MaxRaw;
      CalibrationValueUpDown.Minimum = _calibrationCalculator.MinRaw;
      CalibrationValueUpDown.Value = CalibrationValue;

      RangeValueLabel.Text = $"{_calibrationCalculator.MinRaw}..{_calibrationCalculator.MaxRaw}";

      this.AcceptButton = okButton;
      this.CancelButton = cancelButton;

      okButton.Click += OkButton_Click;
    }

    private void CalibrationValueUpDown_ValueChanged(object sender, System.EventArgs e)
    {
      int newCalibrationValue = (int)CalibrationValueUpDown.Value;
      int relativeCalibrationValue = newCalibrationValue - CalibrationValue;

      double ppmAbs = _calibrationCalculator.ToPPM(newCalibrationValue);
      double spdAbs = _calibrationCalculator.ToSecPerDay(newCalibrationValue);

      double ppmRel = _calibrationCalculator.ToPPM(relativeCalibrationValue);
      double spdRel = _calibrationCalculator.ToSecPerDay(relativeCalibrationValue);

      RelativeValueLabel.Text = $"{ppmRel:F3} ppm, {spdRel:F3} sec/day";
      AbsoluteValueLabel.Text = $"{ppmAbs:F3} ppm, {spdAbs:F3} sec/day";
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
      CalibrationValue = (int)CalibrationValueUpDown.Value;
    }
  }
}
