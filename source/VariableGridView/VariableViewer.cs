using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DebugViews
{
  public partial class VariableViewer : UserControl
  {
    private DataGridView dataGridView;

    public VariableViewer()
    {
      InitializeComponent();
      InitializeViewer();
    }

    private void InitializeComponent()
    {
      this.dataGridView = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView.Location = new System.Drawing.Point(0, 0);
      this.dataGridView.Name = "dataGridView";
      //this.dataGridView.RowHeadersWidth = 51;
      //this.dataGridView.RowTemplate.Height = 24;
      this.dataGridView.Size = new System.Drawing.Size(255, 208);
      this.dataGridView.TabIndex = 0;
      // 
      // VariableViewer
      // 
      this.Controls.Add(this.dataGridView);
      this.Name = "VariableViewer";
      this.Size = new System.Drawing.Size(255, 208);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);

    }
  }
}
