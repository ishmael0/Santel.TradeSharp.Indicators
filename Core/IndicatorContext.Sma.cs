namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full SMA array for the context data.</summary>
    public double[] GetSma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _smas.Count; i++)
            if (_smas[i].Period == period)
                return _smas[i].Values;

        var values = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sum = 0.0;

            for (var j = start; j <= i; j++)
                sum += Close(j);

            values[i] = sum / count;
        }

        _smas.Add(new Indicators.SmaIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the SMA value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes only the window ending at the target bar.
    /// </summary>
    /// <param name="period">The SMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetSma(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _smas.Count; i++)
            if (_smas[i].Period == period)
                return _smas[i].Values[targetIndex];

        // Slow path: SMA is a simple window average — no state dependency.
        var start = Math.Max(0, targetIndex - period + 1);
        var sum = 0.0;
        for (var j = start; j <= targetIndex; j++)
            sum += Close(j);

        return sum / (targetIndex - start + 1);
    }

    /// <summary>
    /// Returns the SMA value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute SMA on.</param>
    /// <param name="period">The SMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetSma(IReadOnlyList<T> data, int period, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetSma(period, offset);
}
