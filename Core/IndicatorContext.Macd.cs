namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.MacdSeries GetMacd(int fastPeriod, int slowPeriod, int signalPeriod)
    {
        if (fastPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(fastPeriod));
        if (slowPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(slowPeriod));
        if (signalPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(signalPeriod));
        if (fastPeriod >= slowPeriod) throw new ArgumentException("fastPeriod must be less than slowPeriod.");

        for (var i = 0; i < _macds.Count; i++)
        {
            var existing = _macds[i];
            if (existing.FastPeriod == fastPeriod && existing.SlowPeriod == slowPeriod && existing.SignalPeriod == signalPeriod)
                return existing.Series;
        }

        var fastEma = GetEma(fastPeriod);
        var slowEma = GetEma(slowPeriod);

        var macdValues = new double[_data.Count];
        for (var i = 0; i < macdValues.Length; i++)
            macdValues[i] = fastEma[i] - slowEma[i];

        var signalValues = new double[macdValues.Length];
        if (macdValues.Length > 0)
        {
            var multiplier = 2.0 / (signalPeriod + 1.0);
            var ema = macdValues[0];
            signalValues[0] = ema;

            for (var i = 1; i < macdValues.Length; i++)
            {
                ema = ((macdValues[i] - ema) * multiplier) + ema;
                signalValues[i] = ema;
            }
        }

        var histValues = new double[_data.Count];
        for (var i = 0; i < histValues.Length; i++)
            histValues[i] = macdValues[i] - signalValues[i];

        var series = new Indicators.MacdSeries(macdValues, signalValues, histValues);
        _macds.Add(new Indicators.MacdIndicator(fastPeriod, slowPeriod, signalPeriod, series));
        return series;
    }

    /// <summary>
    /// Returns MACD, Signal and Histogram values for a specific point in time.
    /// If the full series is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes the full series and caches it before indexing.
    /// </summary>
    /// <param name="fastPeriod">The fast EMA period.</param>
    /// <param name="slowPeriod">The slow EMA period.</param>
    /// <param name="signalPeriod">The signal EMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    /// <returns>A tuple of (Macd, Signal, Histogram) at the target bar.</returns>
    public (double Macd, double Signal, double Histogram) GetMacd(int fastPeriod, int slowPeriod, int signalPeriod, int offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        var series = GetMacd(fastPeriod, slowPeriod, signalPeriod); // uses cache if available
        return (series.Macd[targetIndex], series.Signal[targetIndex], series.Histogram[targetIndex]);
    }

    /// <summary>
    /// Returns MACD, Signal and Histogram values at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute MACD on.</param>
    /// <param name="fastPeriod">The fast EMA period.</param>
    /// <param name="slowPeriod">The slow EMA period.</param>
    /// <param name="signalPeriod">The signal EMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public (double Macd, double Signal, double Histogram) GetMacd(IReadOnlyList<T> data, int fastPeriod, int slowPeriod, int signalPeriod, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetMacd(fastPeriod, slowPeriod, signalPeriod, offset);
}
