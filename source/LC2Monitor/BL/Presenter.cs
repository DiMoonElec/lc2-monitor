using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LC2Monitor.BL
{
  public class Presenter
  {
    private readonly IView _view;
    private readonly IBusinessLogic _logic;
    private readonly Func<IDateTimeInputView> _dateTimeInputFactory;

    private CancellationTokenSource _pollingCancellationTokenSource;
    private AutoResetEvent _variablesUpdateEvent = new AutoResetEvent(false);
    public Presenter(IView view,
      Func<IDateTimeInputView> dateTimeInputFactory,
      IBusinessLogic logic)
    {
      _view = view;
      _logic = logic;
      _dateTimeInputFactory = dateTimeInputFactory;

      _view.FormLoad += () => _logic.Init();
      _view.OnOpenProject += (file) => _logic.LoadProject(file);
      _view.OnDumpSaveClicked += (file) => _logic.LCVMSaveDump(file);
      _view.OnDumpPrintClicked += () => _logic.LCVMPrintDump();
      _view.OnDisconnectClicked += () => _logic.DisconnectFromPlc();
      _view.OnSendBinaryClicked += () => _logic.SendBinaryToPlc();
      _view.OnRunClicked += () => _logic.RunPlc();
      _view.OnStopClicked += () => _logic.StopPlc();
      _view.OnStepClicked += () => _logic.CyclePlc();
      _view.OnConnectMenuOpening += _view_OnConnectMenuOpening;
      _view.VariableViewerValueChanged += (element) => _logic.VariableViewerValueChanged(element);
      _view.OnRTCSyncWithPCClicked += () => _logic.RTCSyncWithPC();
      _view.OnRTCSyncSetDateTimeClicked += () =>
      {
        using (var inputForm = dateTimeInputFactory())
        {
          if (inputForm.ShowDialog() == DialogResult.OK)
          {
            DateTime dt = inputForm.SelectedDateTime;
            _logic.RTCSync(dt);
          }
        }
      };

      _view.OnSaveProgramToFlashClicked += () => _logic.SaveProgramToFlash();

      _logic.OnLogUpdated += _view.UpdateLog;
      _logic.OnStatusbarUpdated += (connection, plc) => _view.UpdateStatus(connection, plc);
      _logic.OnUpdateControlStates += (isConnected, isProjectLoaded, plcStatus) => _view.UpdateControlStates(isConnected, isProjectLoaded, plcStatus);
      _logic.UpdateMetrics += (CyclePeriod, CycleDuration, CycleDurationMax) => _view.DisplayMetrics(CyclePeriod, CycleDuration, CycleDurationMax);
      _logic.DisplayRTCTime += (dateTime) => _view.DisplayRTCTime(dateTime);

      _logic.OnVariablesUpdated += _view.UpdateVariablesList;

      _logic.EnableVariablesPoll += () =>
      {
        _pollingCancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => PollingLoop(_pollingCancellationTokenSource.Token));
      };

      _logic.DisableVariablesPoll += () =>
      {
        _pollingCancellationTokenSource?.Cancel();
        _variablesUpdateEvent.Set();
      };

      _logic.InstantVariablesPoll += () => { LiveVariablesUpdate(); };

      _logic.VariablesDumpUpdated += (dump) =>
      {
        _view.SetWatchVariables(dump);
        _variablesUpdateEvent.Set();
      };
    }

    private void _view_OnConnectMenuOpening(object sender, EventArgs e)
    {
      ToolStripMenuItem menu = (ToolStripMenuItem)sender;
      menu.DropDownItems.Clear();

      var ports = _logic.GetAvailablePorts();

      if (ports.Count() == 0)
      {
        ToolStripMenuItem element = new ToolStripMenuItem();
        element.Text = "[none]";
        element.Enabled = false;
        menu.DropDownItems.Add(element);
        return;
      }

      foreach (var c in ports)
      {
        ToolStripMenuItem element = new ToolStripMenuItem();
        element.Text = c;
        element.Enabled = true;
        //element.Enabled = c.IsActive;
        //element.Checked = c.Checked;
        element.Click += Element_Click;
        element.Tag = c;
        menu.DropDownItems.Add(element);
      }
    }

    private void Element_Click(object sender, EventArgs e)
    {
      var currentElement = (ToolStripMenuItem)sender;
      var port = (string)currentElement.Tag;
      _logic.ConnectToPlc(port);
    }

    private void LiveVariablesUpdate()
    {
      //Список переменных, которые необходимо обновить
      var variables = _view.GetWatchVariables();

      //Формируем список запросов к областям памяти
      var requests = DataElementHelper.BuildMemoryRequests(variables, 0);

      //Запустить чтение дампа ОЗУ ПЛК
      _logic.UpdateMemoryDump(requests);
    }

    private async Task PollingLoop(CancellationToken token)
    {
      _variablesUpdateEvent.Reset();

      while (!token.IsCancellationRequested)
      {
        try
        {
          LiveVariablesUpdate();
          _logic.GetMetrics();

          // Задержка между циклами
          await Task.Delay(1000, token);
          _variablesUpdateEvent.WaitOne();
        }
        catch (OperationCanceledException)
        {
          // Прерывание цикла
          break;
        }
        catch (Exception ex)
        {
          // Обработка ошибок (например, логирование)
          _view.UpdateLog($"Error: {ex.Message}");
        }
      }
    }
  }

}
