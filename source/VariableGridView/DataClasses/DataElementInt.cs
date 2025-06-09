using System;

namespace DebugViews.DataClasses
{
  public class DataElementInt : DataElementBase
  {
    private int _value;
    private bool _update;

    public static int Sizeof { get { return 4; } }

    public override byte[] Bytes => BitConverter.GetBytes(_value);

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

    public DataElementInt(string name, int address)
        : base(name, "int", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!int.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.  Valid range is {int.MinValue}..{int.MaxValue}.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = BitConverter.ToInt32(data, 0);

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
