namespace Santel.TradeSharp.Indicators;

public sealed class BollingerBands
{
    public BollingerBands(double[] upper, double[] middle, double[] lower)
    {
        Upper = upper;
        Middle = middle;
        Lower = lower;
    }

    public double[] Upper { get; }
    public double[] Middle { get; }
    public double[] Lower { get; }
}
