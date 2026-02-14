namespace Santel.TradeSharp.Indicators.Indicators;

public sealed class Ichimoku
{
    public Ichimoku(double[] tenkanSen, double[] kijunSen, double[] senkouSpanA, double[] senkouSpanB, double[] chikouSpan)
    {
        TenkanSen = tenkanSen;
        KijunSen = kijunSen;
        SenkouSpanA = senkouSpanA;
        SenkouSpanB = senkouSpanB;
        ChikouSpan = chikouSpan;
    }

    public double[] TenkanSen { get; }
    public double[] KijunSen { get; }
    public double[] SenkouSpanA { get; }
    public double[] SenkouSpanB { get; }
    public double[] ChikouSpan { get; }
}
