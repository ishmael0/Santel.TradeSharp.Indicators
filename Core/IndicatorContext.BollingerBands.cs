namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full Bollinger Bands series for the context data.</summary>
    public Indicators.BollingerBands GetBollingerBands(int period, double standardDeviation = 2.0)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (standardDeviation <= 0) throw new ArgumentOutOfRangeException(nameof(standardDeviation));

        for (var i = 0; i < _bollingerBands.Count; i++)
        {
            var existing = _bollingerBands[i];
            if (existing.Period == period && existing.StandardDeviation == standardDeviation)
                return existing.Bands;
        }

        var sma = GetSma(period);
        var upper = new double[_data.Count];
        var lower = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sumSquares = 0.0;

            for (var j = start; j <= i; j++)
            {
                var diff = Close(j) - sma[i];
                sumSquares += diff * diff;
            }

            var stdDev = Math.Sqrt(sumSquares / count);
            upper[i] = sma[i] + (stdDev * standardDeviation);
            lower[i] = sma[i] - (stdDev * standardDeviation);
        }

        var bands = new Indicators.BollingerBands(upper, sma, lower);
        _bollingerBands.Add(new Indicators.BollingerBandsIndicator(period, standardDeviation, bands));
        return bands;
    }

    /// <summary>
    /// Returns upper, middle and lower Bollinger Bands values for a specific point in time.
    /// If the full series is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes the full series and caches it before indexing.
    /// </summary>
    /// <param name="period">The Bollinger Bands period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    /// <param name="standardDeviation">The standard deviation multiplier.</param>
    /// <returns>A tuple of (Upper, Middle, Lower) at the target bar.</returns>
    public (double Upper, double Middle, double Lower) GetBollingerBands(int period, int offset, double standardDeviation = 2.0)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (standardDeviation <= 0) throw new ArgumentOutOfRangeException(nameof(standardDeviation));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        var bands = GetBollingerBands(period, standardDeviation); // uses cache if available
        return (bands.Upper[targetIndex], bands.Middle[targetIndex], bands.Lower[targetIndex]);
    }

    /// <summary>
    /// Returns upper, middle and lower Bollinger Bands values at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    public (double Upper, double Middle, double Lower) GetBollingerBands(IReadOnlyList<T> data, int period, int offset, double standardDeviation = 2.0)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetBollingerBands(period, offset, standardDeviation);
}
