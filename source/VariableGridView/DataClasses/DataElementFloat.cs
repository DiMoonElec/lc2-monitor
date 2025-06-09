using System;

namespace DebugViews.DataClasses
{
  public class DataElementFloat : DataElementBase
  {
    private float _value;
    private bool _update;

    public override byte[] Bytes => BitConverter.GetBytes(_value);

    public static int Sizeof { get { return 4; } }

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

    public DataElementFloat(string name, int address)
        : base(name, "float", address, Sizeof)
    {
      _value = 0;
    }

    public override void SetValue(string value)
    {
      if (!float.TryParse(value, out var parsedValue))
      {
        throw new ArgumentException($"Invalid value.");
      }

      _value = parsedValue;
    }

    protected override void UpdateValueFromData(byte[] data)
    {
      var v = BitConverter.ToSingle(data, 0);

      if (v == _value)
        ValueChanged = false;
      else
        ValueChanged = true;

      _value = v;
    }
  }
}
