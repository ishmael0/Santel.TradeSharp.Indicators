namespace Santel.TradeSharp.Indicators;

public sealed class ObvIndicator
{
    public ObvIndicator(double[] values)
    {
        Values = values;
    }

    public double[] Values { get; }
}
