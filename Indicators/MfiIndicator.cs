namespace Santel.TradeSharp.Indicators;

public sealed class MfiIndicator
{
    public MfiIndicator(int period, double[] values)
    {
        Period = period;
        Values = values;
    }

    public int Period { get; }
    public double[] Values { get; }
}
