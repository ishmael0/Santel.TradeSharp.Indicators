using System.Collections.Concurrent;

namespace Santel.TradeSharp.Indicators;

public sealed class IndicatorContext
{
    private readonly IReadOnlyList<Candle> _candles;
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public IndicatorContext(IReadOnlyList<Candle> candles)
    {
        _candles = candles;
    }

    public IReadOnlyList<Candle> Candles => _candles;

    public IndicatorSeries<double> CalculateEma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        var key = $"EMA:{period}";
        return (IndicatorSeries<double>)_cache.GetOrAdd(key, _ => IndicatorCalculators.CalculateEma(_candles, period));
    }

    public MacdSeries CalculateMacd(int fastPeriod, int slowPeriod, int signalPeriod)
    {
        if (fastPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(fastPeriod));
        if (slowPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(slowPeriod));
        if (signalPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(signalPeriod));
        if (fastPeriod >= slowPeriod) throw new ArgumentException("fastPeriod must be less than slowPeriod.");

        var key = $"MACD:{fastPeriod}:{slowPeriod}:{signalPeriod}";
        return (MacdSeries)_cache.GetOrAdd(key, _ => IndicatorCalculators.CalculateMacd(this, fastPeriod, slowPeriod, signalPeriod));
    }
}
