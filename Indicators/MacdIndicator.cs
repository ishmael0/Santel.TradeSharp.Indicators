namespace Santel.TradeSharp.Indicators;

public sealed class MacdIndicator
{
    public MacdIndicator(int fastPeriod, int slowPeriod, int signalPeriod, MacdSeries series)
    {
        FastPeriod = fastPeriod;
        SlowPeriod = slowPeriod;
        SignalPeriod = signalPeriod;
        Series = series;
    }

    public int FastPeriod { get; }
    public int SlowPeriod { get; }
    public int SignalPeriod { get; }
    public MacdSeries Series { get; }
}
