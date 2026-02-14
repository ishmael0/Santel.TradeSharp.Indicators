namespace Santel.TradeSharp.Indicators;

public sealed class StochasticIndicator
{
    public StochasticIndicator(int kPeriod, int dPeriod, Stochastic stochastic)
    {
        KPeriod = kPeriod;
        DPeriod = dPeriod;
        Stochastic = stochastic;
    }

    public int KPeriod { get; }
    public int DPeriod { get; }
    public Stochastic Stochastic { get; }
}
