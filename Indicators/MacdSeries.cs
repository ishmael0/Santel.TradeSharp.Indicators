namespace Santel.TradeSharp.Indicators;

public sealed class MacdSeries
{
    public MacdSeries(double[] macd, double[] signal, double[] histogram)
    {
        Macd = macd;
        Signal = signal;
        Histogram = histogram;
    }

    public double[] Macd { get; }
    public double[] Signal { get; }
    public double[] Histogram { get; }
}
