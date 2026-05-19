namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.Stochastic GetStochastic(int kPeriod, int dPeriod)
    {
        if (kPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(kPeriod));
        if (dPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(dPeriod));

        for (var i = 0; i < _stochastics.Count; i++)
        {
            var existing = _stochastics[i];
            if (existing.KPeriod == kPeriod && existing.DPeriod == dPeriod)
                return existing.Stochastic;
        }

        var kValues = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - kPeriod + 1);
            var high = double.MinValue;
            var low = double.MaxValue;

            for (var j = start; j <= i; j++)
            {
                if (High(j) > high) high = High(j);
                if (Low(j) < low) low = Low(j);
            }

            var range = high - low;
            kValues[i] = range > 0 ? ((Close(i) - low) / range) * 100 : 0;
        }

        var dValues = new double[_data.Count];
        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - dPeriod + 1);
            var sum = 0.0;
            var count = 0;

            for (var j = start; j <= i; j++)
            {
                sum += kValues[j];
                count++;
            }

            dValues[i] = count > 0 ? sum / count : 0;
        }

        var stochastic = new Indicators.Stochastic(kValues, dValues);
        _stochastics.Add(new Indicators.StochasticIndicator(kPeriod, dPeriod, stochastic));
        return stochastic;
    }

    /// <summary>
    /// Returns %K and %D Stochastic values for a specific point in time.
    /// If the full series is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes the full series and caches it before indexing.
    /// </summary>
    /// <param name="kPeriod">The %K period.</param>
    /// <param name="dPeriod">The %D smoothing period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    /// <returns>A tuple of (K, D) at the target bar.</returns>
    public (double K, double D) GetStochastic(int kPeriod, int dPeriod, int offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        var stochastic = GetStochastic(kPeriod, dPeriod); // uses cache if available
        return (stochastic.K[targetIndex], stochastic.D[targetIndex]);
    }

    /// <summary>
    /// Returns %K and %D Stochastic values at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute Stochastic on.</param>
    /// <param name="kPeriod">The %K period.</param>
    /// <param name="dPeriod">The %D smoothing period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public (double K, double D) GetStochastic(IReadOnlyList<T> data, int kPeriod, int dPeriod, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetStochastic(kPeriod, dPeriod, offset);
}
