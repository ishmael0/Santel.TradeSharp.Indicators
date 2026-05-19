namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full ATR array for the context data.</summary>
    public double[] GetAtr(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _atrs.Count; i++)
            if (_atrs[i].Period == period)
                return _atrs[i].Values;

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            var trueRanges = new double[_data.Count];
            trueRanges[0] = High(0) - Low(0);

            for (var i = 1; i < _data.Count; i++)
            {
                var highLow = High(i) - Low(i);
                var highClose = Math.Abs(High(i) - Close(i - 1));
                var lowClose = Math.Abs(Low(i) - Close(i - 1));
                trueRanges[i] = Math.Max(highLow, Math.Max(highClose, lowClose));
            }

            var sum = 0.0;
            for (var i = 0; i < period && i < _data.Count; i++)
                sum += trueRanges[i];

            values[period - 1] = sum / period;

            for (var i = period; i < _data.Count; i++)
                values[i] = (values[i - 1] * (period - 1) + trueRanges[i]) / period;
        }

        _atrs.Add(new Indicators.AtrIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the ATR value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, iterates from bar 0 up to the target bar only.
    /// </summary>
    /// <param name="period">The ATR period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetAtr(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _atrs.Count; i++)
            if (_atrs[i].Period == period)
                return _atrs[i].Values[targetIndex];

        // Slow path: compute inline up to targetIndex only.
        if (targetIndex < period - 1) return 0;

        var tr0 = High(0) - Low(0);
        var atr = tr0;

        for (var i = 1; i < period; i++)
        {
            var highLow = High(i) - Low(i);
            var highClose = Math.Abs(High(i) - Close(i - 1));
            var lowClose = Math.Abs(Low(i) - Close(i - 1));
            atr += Math.Max(highLow, Math.Max(highClose, lowClose));
        }
        atr /= period;

        for (var i = period; i <= targetIndex; i++)
        {
            var highLow = High(i) - Low(i);
            var highClose = Math.Abs(High(i) - Close(i - 1));
            var lowClose = Math.Abs(Low(i) - Close(i - 1));
            var tr = Math.Max(highLow, Math.Max(highClose, lowClose));
            atr = (atr * (period - 1) + tr) / period;
        }

        return atr;
    }

    /// <summary>
    /// Returns the ATR value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute ATR on.</param>
    /// <param name="period">The ATR period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetAtr(IReadOnlyList<T> data, int period, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetAtr(period, offset);
}
