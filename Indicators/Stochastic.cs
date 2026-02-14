namespace Santel.TradeSharp.Indicators.Indicators;

public sealed class Stochastic
{
    public Stochastic(double[] k, double[] d)
    {
        K = k;
        D = d;
    }

    public double[] K { get; }
    public double[] D { get; }
}
