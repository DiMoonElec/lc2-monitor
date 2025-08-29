using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DebugViews.DataClasses;
using LC2Monitor.BL;

namespace LC2Monitor
{
  public partial class MainForm : Form, IView
  {
    public event Action<string> OnOpenProject;
    public event Action<string> OnDumpSaveClicked;
    public event Action OnDumpPrintClicked;
    public event Action OnDisconnectClicked;
    public event Action OnSendBinaryClicked;
    public event Action OnRunClicked;
    public event Action OnStopClicked;
    public event Action OnStepClicked;
    public event Action<object, EventArgs> OnConnectMenuOpening;
    public event Action<DataElementBase> VariableViewerValueChanged;
    public event Action FormLoad;
    public event Action OnRTCSyncClicked;
    public event Action OnSaveProgramToFlashClicked;

    public MainForm()
    {
      InitializeComponent();
      disconnectToolStripMenuItem.Click += (s, e) => OnDisconnectClicked?.Invoke();
      rTCSyncToolStripMenuItem.Click += (s, e) => OnRTCSyncClicked?.Invoke();
      saveProgramToFlashToolStripMenuItem.Click += (s, e) => OnSaveProgramToFlashClicked?.Invoke();


      btnSendBinary.Click += (s, e) => OnSendBinaryClicked?.Invoke();
      btnRun.Click += (s, e) => OnRunClicked?.Invoke();
      btnStop.Click += (s, e) => OnStopClicked?.Invoke();
      btnStep.Click += (s, e) => OnStepClicked?.Invoke();
      connectToolStripMenuItem.DropDownOpening += (s, e) => OnConnectMenuOpening?.Invoke(s, e);
      lCVMDumpToolStripMenuItem.Click += LCVMDumpToolStripMenuItem_Click;
      variableViewer.VariableChanged += OnVariableChanged;


      // Добавление контекстного меню
      ContextMenu contextMenu = new ContextMenu();

      // Пункт "Очистить"
      MenuItem clearMenuItem = new MenuItem("Очистить");
      clearMenuItem.Click += (s, e) => logsListBox.Items.Clear();

      // Пункт "Копировать"
      MenuItem copyMenuItem = new MenuItem("Копировать");
      copyMenuItem.Click += (s, e) => CopyItemsToClipboard();

      // Добавление пунктов в контекстное меню
      contextMenu.MenuItems.Add(copyMenuItem);
      contextMenu.MenuItems.Add(clearMenuItem);

      // Установка контекстного меню для ListBox
      logsListBox.ContextMenu = contextMenu;
    }

    // Метод для копирования всех Items в буфер обмена
    private void CopyItemsToClipboard()
    {
      if (logsListBox.Items.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var item in logsListBox.Items)
        {
          stringBuilder.AppendLine(item.ToString());
        }

        Clipboard.SetText(stringBuilder.ToString());
      }
    }

    private void LCVMDumpToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OnDumpPrintClicked?.Invoke();
      return;

      // Создаем диалог выбора файла
      SaveFileDialog fileDialog = new SaveFileDialog
      {
        Filter = "LCVM Dump (*.xml)|*.lcprj|All Files (*.*)|*.*", // Фильтр по расширению
        Title = "Сохранение LCVM Dump",
      };

      // Открываем диалоговое окно и проверяем, была ли нажата кнопка "ОК"
      if (fileDialog.ShowDialog() == DialogResult.OK)
        OnDumpSaveClicked?.Invoke(fileDialog.FileName);
    }

    public void UpdateStatus(string connectionStatus, string plcStatus)
    {
      lblConnectionStatus.Text = connectionStatus;
      lblPlcStatus.Text = plcStatus;
    }

    public void UpdatePortsList(IEnumerable<string> ports)
    {
      //comboBoxPorts.Items.Clear();
      //comboBoxPorts.Items.AddRange(ports.ToArray());
    }

    public void UpdateVariablesList(IEnumerable<DataElementBase> variables)
    {
      this.InvokeIfRequired(() => variableViewer.LoadVariables(variables));
    }

    public void UpdateLog(string message)
    {
      this.InvokeIfRequired(() => AddMessageToListBox(message));
    }

    public void UpdateControlStates(bool isConnected, bool isProjectLoaded,
      PLCStatus plcStatus)
    {
      this.InvokeIfRequired(() =>
      {
        btnSendBinary.Enabled = isConnected && isProjectLoaded && plcStatus == PLCStatus.Stop;

        connectToolStripMenuItem.Enabled = !isConnected;

        disconnectToolStripMenuItem.Enabled = isConnected;

        btnRun.Enabled = isConnected
            && (plcStatus == PLCStatus.Stop || plcStatus == PLCStatus.Pause || plcStatus == PLCStatus.Exception);

        btnStop.Enabled = isConnected
            && (plcStatus == PLCStatus.Run || plcStatus == PLCStatus.Pause || plcStatus == PLCStatus.Cycle);

        btnStep.Enabled = isConnected && isProjectLoaded;

        lCVMDumpToolStripMenuItem.Enabled = isConnected
            && (plcStatus == PLCStatus.Stop || plcStatus == PLCStatus.Pause || plcStatus == PLCStatus.Exception);

        rTCSyncToolStripMenuItem.Enabled = isConnected;

        saveProgramToFlashToolStripMenuItem.Enabled = isConnected && isProjectLoaded
         && (plcStatus == PLCStatus.Stop || plcStatus == PLCStatus.Pause || plcStatus == PLCStatus.Exception);
      });
    }

    public void DisplayMetrics(int cycleValue, int duration, int durationMax)
    {
      this.InvokeIfRequired(() =>
      {
        lblPLCCycle.Text = $"{cycleValue.ToString()}mS";

        if (duration == 0)
          lblPLCDuration.Text = "<1mS";
        else
          lblPLCDuration.Text = $"{duration.ToString()}mS";

        if (durationMax == 0)
          lblPLCDurationMax.Text = "<1mS";
        else
          lblPLCDurationMax.Text = $"{durationMax.ToString()}mS";
      });
    }

    public void DisplayRTCTime(DateTime dateTime)
    {
      this.InvokeIfRequired(() =>
      {
        lblRTCValue.Text = dateTime.ToString();
      });
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // Создаем диалог выбора файла
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Filter = "LC Project Files (*.lcprj)|*.lcprj|All Files (*.*)|*.*", // Фильтр по расширению
        Title = "Выберите файл LCProject",
        CheckFileExists = true, // Проверяем существование файла
        Multiselect = false // Отключаем возможность выбора нескольких файлов
      };

      // Открываем диалоговое окно и проверяем, была ли нажата кнопка "ОК"
      if (openFileDialog.ShowDialog() == DialogResult.OK)
        OnOpenProject?.Invoke(openFileDialog.FileName);
    }

    public IEnumerable<DataElementBase> GetWatchVariables()
    {
      return this.InvokeIfRequired(() => { return variableViewer.GetWatchVariables(); });
      //return variableViewer.GetWatchVariables();
    }

    public void SetWatchVariables(VariablesDump variablesDump)
    {
      this.InvokeIfRequired(() => { variableViewer.UpdateValues(variablesDump); });
    }

    private void OnVariableChanged(DataElementBase variable)
    {
      VariableViewerValueChanged?.Invoke(variable);
    }

    // Метод для добавления многострочного сообщения в ListBox
    private void AddMessageToListBox(string message)
    {
      // Разбиваем сообщение по строкам
      var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

      // Добавляем каждую строку как отдельный элемент
      foreach (var line in lines)
      {
        logsListBox.Items.Add(line);
      }

      // Прокручиваем ListBox к последнему элементу
      if (logsListBox.Items.Count > 0)
      {
        logsListBox.SelectedIndex = logsListBox.Items.Count - 1;
        logsListBox.SelectedIndex = -1; // Сбрасываем выделение
      }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      // Восстановление размера формы
      if (Properties.Settings.Default.FormSize.Width > 0 && Properties.Settings.Default.FormSize.Height > 0)
      {
        this.Size = Properties.Settings.Default.FormSize;
      }

      // Восстановление расстояния Splitter1
      int savedDistance1 = Properties.Settings.Default.DistanceSplitter1;
      if (savedDistance1 > splitContainer1.Panel1MinSize &&
          savedDistance1 < splitContainer1.Width - splitContainer1.Panel2MinSize)
      {
        splitContainer1.SplitterDistance = savedDistance1;
      }

      // Восстановление расстояния Splitter2
      int savedDistance2 = Properties.Settings.Default.DistanceSplitter2;
      if (savedDistance2 > splitContainer2.Panel1MinSize &&
          savedDistance2 < splitContainer2.Width - splitContainer2.Panel2MinSize)
      {
        splitContainer2.SplitterDistance = savedDistance2;
      }

      FormLoad?.Invoke();
    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      // Сохранение размера формы
      Properties.Settings.Default.FormSize = this.Size;

      // Сохранение расстояния Splitter1
      Properties.Settings.Default.DistanceSplitter1 = splitContainer1.SplitterDistance;

      // Сохранение расстояния Splitter2
      Properties.Settings.Default.DistanceSplitter2 = splitContainer2.SplitterDistance;

      // Сохранение настроек
      Properties.Settings.Default.Save();
    }
  }
}
