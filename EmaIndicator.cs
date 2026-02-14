namespace Santel.TradeSharp.Indicators;

public sealed class EmaIndicator
{
    public EmaIndicator(int period, IndicatorSeries<double> series)
    {
        Period = period;
        Series = series;
    }

    public int Period { get; }
    public IndicatorSeries<double> Series { get; }
}
