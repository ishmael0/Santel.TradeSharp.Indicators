namespace Santel.TradeSharp.Indicators.Indicators;

public sealed class SmaIndicator
{
    public SmaIndicator(int period, double[] values)
    {
        Period = period;
        Values = values;
    }

    public int Period { get; }
    public double[] Values { get; }
}
