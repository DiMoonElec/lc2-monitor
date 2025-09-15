namespace LC2Monitor
{
  partial class RTCCorrectionInputForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.CalibrationValueUpDown = new System.Windows.Forms.NumericUpDown();
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.RangeValueLabel = new System.Windows.Forms.Label();
      this.AbsoluteValueLabel = new System.Windows.Forms.Label();
      this.RelativeValueLabel = new System.Windows.Forms.Label();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.CalibrationValueUpDown)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // CalibrationValueUpDown
      // 
      this.CalibrationValueUpDown.Location = new System.Drawing.Point(3, 3);
      this.CalibrationValueUpDown.Name = "CalibrationValueUpDown";
      this.CalibrationValueUpDown.Size = new System.Drawing.Size(128, 20);
      this.CalibrationValueUpDown.TabIndex = 0;
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(84, 3);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 1;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(3, 3);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 2;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.CalibrationValueUpDown, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(271, 155);
      this.tableLayoutPanel1.TabIndex = 3;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.RangeValueLabel, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.AbsoluteValueLabel, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.RelativeValueLabel, 1, 2);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 29);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(265, 58);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 3);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(39, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Range";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 22);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(48, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Absolute";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 41);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(46, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Relative";
      // 
      // RangeValueLabel
      // 
      this.RangeValueLabel.AutoSize = true;
      this.RangeValueLabel.Location = new System.Drawing.Point(57, 3);
      this.RangeValueLabel.Margin = new System.Windows.Forms.Padding(3);
      this.RangeValueLabel.Name = "RangeValueLabel";
      this.RangeValueLabel.Size = new System.Drawing.Size(43, 13);
      this.RangeValueLabel.TabIndex = 4;
      this.RangeValueLabel.Text = "0..4095";
      // 
      // AbsoluteValueLabel
      // 
      this.AbsoluteValueLabel.AutoSize = true;
      this.AbsoluteValueLabel.Location = new System.Drawing.Point(57, 22);
      this.AbsoluteValueLabel.Margin = new System.Windows.Forms.Padding(3);
      this.AbsoluteValueLabel.Name = "AbsoluteValueLabel";
      this.AbsoluteValueLabel.Size = new System.Drawing.Size(65, 13);
      this.AbsoluteValueLabel.TabIndex = 5;
      this.AbsoluteValueLabel.Text = "X ppm, +s/h";
      // 
      // RelativeValueLabel
      // 
      this.RelativeValueLabel.AutoSize = true;
      this.RelativeValueLabel.Location = new System.Drawing.Point(57, 41);
      this.RelativeValueLabel.Margin = new System.Windows.Forms.Padding(3);
      this.RelativeValueLabel.Name = "RelativeValueLabel";
      this.RelativeValueLabel.Size = new System.Drawing.Size(65, 13);
      this.RelativeValueLabel.TabIndex = 6;
      this.RelativeValueLabel.Text = "X ppm, +s/h";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel3.AutoSize = true;
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Controls.Add(this.okButton, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.cancelButton, 1, 0);
      this.tableLayoutPanel3.Location = new System.Drawing.Point(106, 123);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(162, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // RTCCorrectionInputForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(295, 179);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "RTCCorrectionInputForm";
      this.Text = "Enter RTC Calibration";
      ((System.ComponentModel.ISupportInitialize)(this.CalibrationValueUpDown)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.NumericUpDown CalibrationValueUpDown;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label RangeValueLabel;
    private System.Windows.Forms.Label AbsoluteValueLabel;
    private System.Windows.Forms.Label RelativeValueLabel;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
  }
}