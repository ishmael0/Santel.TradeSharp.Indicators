namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    // ?? core implementation ???????????????????????????????????????????????????

    private static double[] ComputeEma(IReadOnlyList<double> closes, int period, int? targetIndex = null)
    {
        var end = targetIndex ?? closes.Count - 1;
        var multiplier = 2.0 / (period + 1.0);
        var ema = closes[0];

        if (targetIndex is not null)
        {
            for (var i = 1; i <= end; i++)
                ema = ((closes[i] - ema) * multiplier) + ema;
            return [ema];
        }

        var values = new double[closes.Count];
        values[0] = ema;
        for (var i = 1; i < closes.Count; i++)
        {
            ema = ((closes[i] - ema) * multiplier) + ema;
            values[i] = ema;
        }
        return values;
    }

    // ?? public API ????????????????????????????????????????????????????????????

    /// <summary>Returns (and caches) the full EMA array for the context data.</summary>
    public double[] GetEma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _emas.Count; i++)
            if (_emas[i].Period == period)
                return _emas[i].Values;

        var closes = new double[_data.Count];
        for (var i = 0; i < _data.Count; i++)
            closes[i] = Close(i);

        var values = _data.Count > 0 ? ComputeEma(closes, period) : [];
        _emas.Add(new Indicators.EmaIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the EMA value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, iterates from bar 0 up to the target bar only — no full array allocation.
    /// </summary>
    /// <param name="period">The EMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetEma(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _emas.Count; i++)
            if (_emas[i].Period == period)
                return _emas[i].Values[targetIndex];

        // Slow path: compute inline up to targetIndex only, no array allocation.
        var multiplier = 2.0 / (period + 1.0);
        var ema = Close(0);
        for (var i = 1; i <= targetIndex; i++)
            ema = ((Close(i) - ema) * multiplier) + ema;

        return ema;
    }

    /// <summary>
    /// Returns the EMA value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute EMA on.</param>
    /// <param name="period">The EMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetEma(IReadOnlyList<T> data, int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        var closes = new double[targetIndex + 1];
        for (var i = 0; i <= targetIndex; i++)
            closes[i] = _close(data[i]);

        return ComputeEma(closes, period, targetIndex)[0];
    }
}
