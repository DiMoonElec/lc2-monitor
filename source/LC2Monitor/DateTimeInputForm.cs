using System;
using System.Windows.Forms;
using LC2Monitor.BL;

namespace LC2Monitor
{
  public partial class DateTimeInputForm : Form, IDateTimeInputView
  {
    public DateTime SelectedDateTime { get; private set; }

    //private DateTimePicker dateTimePicker;
    //private Button okButton;
    //private Button cancelButton;

    public DateTimeInputForm(DateTime MinDate, DateTime MaxDate)
    {
      InitializeComponent();
      BuildUI(MinDate, MaxDate);
    }

    private void BuildUI(DateTime minDate, DateTime maxDate)
    {
      //this.Text = "Введите дату и время";
      this.StartPosition = FormStartPosition.CenterParent;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      //this.ClientSize = new System.Drawing.Size(300, 120);

      dateTimePicker.Value = DateTime.Now;
      dateTimePicker.MinDate = minDate;
      dateTimePicker.MaxDate = maxDate;

      /*
      dateTimePicker = new DateTimePicker
      {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "dd.MM.yyyy HH:mm:ss",
        Width = 250,
        Location = new System.Drawing.Point(20, 20),
        Value = DateTime.Now,
        MinDate = minDate,
        MaxDate = maxDate
      };
      */

      okButton.Click += OkButton_Click;

      //this.Controls.Add(dateTimePicker);
      //this.Controls.Add(okButton);
      //this.Controls.Add(cancelButton);

      this.AcceptButton = okButton;
      this.CancelButton = cancelButton;
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
      SelectedDateTime = dateTimePicker.Value;
    }
  }
}
