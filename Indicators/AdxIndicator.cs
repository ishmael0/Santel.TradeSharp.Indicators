namespace Santel.TradeSharp.Indicators.Indicators;

public sealed class AdxIndicator
{
    public AdxIndicator(int period, Adx adx)
    {
        Period = period;
        Adx = adx;
    }

    public int Period { get; }
    public Adx Adx { get; }
}
