namespace LC2Monitor.BL
{
  public partial class BusinessLogic : IBusinessLogic
  {
    private void ProjectOpenedAction()
    {
      if (_Model.State == ModelState.Run)
        _liveWatchTask.Start();
    }

    private void ProjectClosedAction()
    {

    }

    private void StoppedAction()
    {
      _liveWatchTask.Stop();
    }

    private void RunningAction()
    {
      if (_Model.Project != null)
        _liveWatchTask.Start();
    }

    private void CyclingAction()
    {
      _liveWatchTask.Stop();
    }

    private void PausedAction()
    {
      _liveWatchTask.Stop();
      LiveWatchUpdate();
    }

    private void ExceptionOccurredAction()
    {
      _liveWatchTask.Stop();
    }

    private void DisconnectedAction()
    {
      _liveWatchTask.Stop();
      _rtcUpdateTask.Stop();
    }

    private void ConnectedAction()
    {
      _rtcUpdateTask.Start();
    }
  }
}
