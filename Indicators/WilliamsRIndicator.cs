namespace Santel.TradeSharp.Indicators.Indicators;

public sealed class WilliamsRIndicator
{
    public WilliamsRIndicator(int period, double[] values)
    {
        Period = period;
        Values = values;
    }

    public int Period { get; }
    public double[] Values { get; }
}
