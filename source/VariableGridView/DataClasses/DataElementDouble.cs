using System;

namespace DebugViews.DataClasses
{
  public class DataElementDouble : DataElementBase
  {
    private double _value;
    private bool _update;

    public override byte[] Bytes => BitConverter.GetBytes(_value);

    public static int Sizeof { get { return 8; } }

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

    public DataElementDouble(string name, int address)
        : base(name, "double", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!double.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = BitConverter.ToDouble(data, 0);

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
