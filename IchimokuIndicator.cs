namespace Santel.TradeSharp.Indicators;

public sealed class IchimokuIndicator
{
    public IchimokuIndicator(int tenkanPeriod, int kijunPeriod, int senkouSpanBPeriod, Ichimoku ichimoku)
    {
        TenkanPeriod = tenkanPeriod;
        KijunPeriod = kijunPeriod;
        SenkouSpanBPeriod = senkouSpanBPeriod;
        Ichimoku = ichimoku;
    }

    public int TenkanPeriod { get; }
    public int KijunPeriod { get; }
    public int SenkouSpanBPeriod { get; }
    public Ichimoku Ichimoku { get; }
}
