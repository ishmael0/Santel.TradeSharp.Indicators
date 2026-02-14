namespace Santel.TradeSharp.Indicators;

public sealed class Adx
{
    public Adx(double[] adx, double[] plusDi, double[] minusDi)
    {
        Values = adx;
        PlusDi = plusDi;
        MinusDi = minusDi;
    }

    public double[] Values { get; }
    public double[] PlusDi { get; }
    public double[] MinusDi { get; }
}
