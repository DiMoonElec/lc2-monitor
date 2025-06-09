using System;
using System.Windows.Forms;

namespace DebugViews.DataClasses
{
  public class DataElementBool : DataElementBase
  {
    private bool _value;
    private bool _update;
    public override string Value
    {
      get => _value.ToString();
      set => SetValue(value);
    }
    public override bool Watch
    {
      get { return _update; }
      set { _update = value; }
    }

    public static int Sizeof { get { return 1; } }

    public override byte[] Bytes => new byte[] { (byte)(_value ? 1 : 0) };

    public DataElementBool(string name, int address)
        : base(name, "bool", address, Sizeof)
    {
      _value = false;
    }

    public override void SetValue(string value)
    {
      if (!bool.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = data[0] != 0;

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
