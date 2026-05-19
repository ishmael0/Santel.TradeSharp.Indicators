namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full RSI array for the context data.</summary>
    public double[] GetRsi(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _rsis.Count; i++)
            if (_rsis[i].Period == period)
                return _rsis[i].Values;

        var values = new double[_data.Count];

        if (_data.Count > 1)
        {
            var gains = new double[_data.Count];
            var losses = new double[_data.Count];

            for (var i = 1; i < _data.Count; i++)
            {
                var change = Close(i) - Close(i - 1);
                gains[i] = change > 0 ? change : 0;
                losses[i] = change < 0 ? -change : 0;
            }

            var avgGain = 0.0;
            var avgLoss = 0.0;

            for (var i = 1; i <= period && i < _data.Count; i++)
            {
                avgGain += gains[i];
                avgLoss += losses[i];
            }
            avgGain /= period;
            avgLoss /= period;

            values[period] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));

            for (var i = period + 1; i < _data.Count; i++)
            {
                avgGain = (avgGain * (period - 1) + gains[i]) / period;
                avgLoss = (avgLoss * (period - 1) + losses[i]) / period;
                values[i] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));
            }
        }

        _rsis.Add(new Indicators.RsiIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the RSI value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, iterates from bar 0 up to the target bar only.
    /// </summary>
    /// <param name="period">The RSI period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetRsi(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _rsis.Count; i++)
            if (_rsis[i].Period == period)
                return _rsis[i].Values[targetIndex];

        // Slow path: compute inline up to targetIndex only.
        if (targetIndex < period) return 0;

        var avgGain = 0.0;
        var avgLoss = 0.0;

        for (var i = 1; i <= period; i++)
        {
            var change = Close(i) - Close(i - 1);
            avgGain += change > 0 ? change : 0;
            avgLoss += change < 0 ? -change : 0;
        }
        avgGain /= period;
        avgLoss /= period;

        for (var i = period + 1; i <= targetIndex; i++)
        {
            var change = Close(i) - Close(i - 1);
            avgGain = (avgGain * (period - 1) + (change > 0 ? change : 0)) / period;
            avgLoss = (avgLoss * (period - 1) + (change < 0 ? -change : 0)) / period;
        }

        return avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));
    }

    /// <summary>
    /// Returns the RSI value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute RSI on.</param>
    /// <param name="period">The RSI period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetRsi(IReadOnlyList<T> data, int period, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetRsi(period, offset);
}
