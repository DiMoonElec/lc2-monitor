using LC2.LCCompiler;

namespace LC2Monitor.BL
{
  public enum ModelState
  {
    Disconnected = 0,
    Stop,
    Run,
    Cycle,
    Pause,
    Exception,
  };

  internal class Model
  {
    public ModelState State { get; set; }
    public LCProject Project { get; set; }
    public SerialPLCConnector plcConnector { get; set; }
    public PLCClientRequestManager plcRequestManager { get; set; }
    public PLCClientRequests plcRequests { get; set; }

    public Model()
    {
      State = ModelState.Disconnected;
      Project = null;
    }

  }
}
