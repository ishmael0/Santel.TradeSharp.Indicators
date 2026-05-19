namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full Williams %R array for the context data.</summary>
    public double[] GetWilliamsR(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _williamsRs.Count; i++)
            if (_williamsRs[i].Period == period)
                return _williamsRs[i].Values;

        var values = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var high = double.MinValue;
            var low = double.MaxValue;

            for (var j = start; j <= i; j++)
            {
                if (High(j) > high) high = High(j);
                if (Low(j) < low) low = Low(j);
            }

            var range = high - low;
            values[i] = range > 0 ? ((high - Close(i)) / range) * -100 : 0;
        }

        _williamsRs.Add(new Indicators.WilliamsRIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the Williams %R value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes only the window ending at the target bar.
    /// </summary>
    /// <param name="period">The Williams %R period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetWilliamsR(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _williamsRs.Count; i++)
            if (_williamsRs[i].Period == period)
                return _williamsRs[i].Values[targetIndex];

        // Slow path: Williams %R only needs the window ending at targetIndex.
        var start = Math.Max(0, targetIndex - period + 1);
        var high = double.MinValue;
        var low = double.MaxValue;

        for (var j = start; j <= targetIndex; j++)
        {
            if (High(j) > high) high = High(j);
            if (Low(j) < low) low = Low(j);
        }

        var range = high - low;
        return range > 0 ? ((high - Close(targetIndex)) / range) * -100 : 0;
    }

    /// <summary>
    /// Returns the Williams %R value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute Williams %R on.</param>
    /// <param name="period">The Williams %R period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetWilliamsR(IReadOnlyList<T> data, int period, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetWilliamsR(period, offset);
}
