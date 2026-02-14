namespace Santel.TradeSharp.Indicators.Indicators;

public sealed class BollingerBandsIndicator
{
    public BollingerBandsIndicator(int period, double standardDeviation, BollingerBands bands)
    {
        Period = period;
        StandardDeviation = standardDeviation;
        Bands = bands;
    }

    public int Period { get; }
    public double StandardDeviation { get; }
    public BollingerBands Bands { get; }
}
