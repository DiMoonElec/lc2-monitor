using System;

namespace DebugViews.DataClasses
{
  public class DataElementByte : DataElementBase
  {
    public static int Sizeof { get { return 1; } }
    private byte _value;
    private bool _update;
    public override byte[] Bytes => new byte[] { (byte)(_value) };

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

    public DataElementByte(string name, int address)
        : base(name, "byte", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!byte.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.  Valid range is {sbyte.MinValue}..{sbyte.MaxValue}.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = data[0];

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
