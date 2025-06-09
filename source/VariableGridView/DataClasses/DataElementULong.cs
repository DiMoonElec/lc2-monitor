using System;

namespace DebugViews.DataClasses
{
  public class DataElementULong : DataElementBase
  {
    private ulong _value;
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

    public DataElementULong(string name, int address)
        : base(name, "ulong", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!ulong.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.  Valid range is {ulong.MinValue}..{ulong.MaxValue}.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = BitConverter.ToUInt64(data, 0);

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
