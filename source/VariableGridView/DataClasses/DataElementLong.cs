using System;

namespace DebugViews.DataClasses
{
  public class DataElementLong : DataElementBase
  {
    private long _value;
    private bool _update;

    public static int Sizeof { get { return 8; } }

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

    public DataElementLong(string name, int address)
        : base(name, "long", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!long.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.  Valid range is {long.MinValue}..{long.MaxValue}.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = BitConverter.ToInt64(data, 0);

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
