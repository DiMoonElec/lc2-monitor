using LC2Monitor.BL;

namespace LC2Monitor.MISC
{
  internal class RTCACalibrationCalculator : IRTCCalibrationCalculator
  {
    public int MinRaw { get { return 0; } }
    public int MaxRaw { get { return 127; } }

    private const long CycleClocks = 1 << 20; // 2^20 = 1048576
    private const double SecondsInDay = 86400.0;

    public double ToPPM(int rawValue)
    {
      return -((double)rawValue / CycleClocks * 1_000_000.0);
    }

    public double ToSecPerDay(int rawValue)
    {
      return -((double)rawValue / CycleClocks * SecondsInDay);
    }
  }
}
