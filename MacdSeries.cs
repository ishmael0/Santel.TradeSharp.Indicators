namespace Santel.TradeSharp.Indicators;

public sealed class MacdSeries
{
    public MacdSeries(IndicatorSeries<double> macd, IndicatorSeries<double> signal, IndicatorSeries<double> histogram)
    {
        Macd = macd;
        Signal = signal;
        Histogram = histogram;
    }

    public IndicatorSeries<double> Macd { get; }
    public IndicatorSeries<double> Signal { get; }
    public IndicatorSeries<double> Histogram { get; }
}
