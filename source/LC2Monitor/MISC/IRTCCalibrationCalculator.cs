namespace LC2Monitor.BL
{
  public interface IRTCCalibrationCalculator
  {
    int MinRaw { get; }
    int MaxRaw { get; }
    double ToPPM(int rawValue);
    double ToSecPerDay(int rawValue);
  }
}
