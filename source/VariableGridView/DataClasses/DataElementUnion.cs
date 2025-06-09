using System;
using System.Collections.Generic;

namespace DebugViews.DataClasses
{
  public class DataElementUnion : DataElementBase
  {
    public bool IsExpanded { get; set; }

    private bool _update;
    public List<DataElementBase> Elements { get; }
    public override byte[] Bytes => null;

    public DataElementUnion(string name, string type, int size)
      : base(name, type, 0, size)
    {
      Elements = new List<DataElementBase>(size);
      IsExpanded = false;
    }

    public override string Value
    {
      get => "";
      set { }
    }

    public override bool Watch
    {
      get { return _update; }
      set { _update = value; foreach (var e in Elements) e.Watch = value; }
    }


    public override void SetValue(string value)
    {
      throw new InvalidOperationException("Cannot set value of array directly.");
    }

    public override void UpdateValue(VariablesDump dump)
    {
      foreach (var element in Elements)
        element.UpdateValue(dump);
    }

    protected override void UpdateValueFromData(byte[] data)
    {

    }
  }
}
