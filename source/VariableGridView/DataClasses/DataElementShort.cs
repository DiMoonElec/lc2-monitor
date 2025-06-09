using System;

namespace DebugViews.DataClasses
{
  public class DataElementShort : DataElementBase
  {
    private short _value;
    private bool _update;

    public static int Sizeof { get { return 2; } }

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

    public DataElementShort(string name, int address)
        : base(name, "short", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!short.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.  Valid range is {short.MinValue}..{short.MaxValue}.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = BitConverter.ToInt16(data, 0);

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
