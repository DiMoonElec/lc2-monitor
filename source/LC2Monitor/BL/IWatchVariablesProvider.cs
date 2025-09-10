using System.Collections.Generic;
using DebugViews.DataClasses;

namespace LC2Monitor.BL
{
  public interface IWatchVariablesProvider
  {
    IEnumerable<DataElementBase> GetWatchVariables();
  }

}
