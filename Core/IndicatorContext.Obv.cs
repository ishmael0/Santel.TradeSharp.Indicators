namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the OBV array for the context data.</summary>
    public double[] GetObv()
    {
        if (_obvs.Count > 0)
            return _obvs[0].Values;

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            values[0] = Volume(0);

            for (var i = 1; i < _data.Count; i++)
            {
                if (Close(i) > Close(i - 1))
                    values[i] = values[i - 1] + Volume(i);
                else if (Close(i) < Close(i - 1))
                    values[i] = values[i - 1] - Volume(i);
                else
                    values[i] = values[i - 1];
            }
        }

        _obvs.Add(new Indicators.ObvIndicator(values));
        return values;
    }

    /// <summary>
    /// Returns the OBV value for a specific point in time.
    /// If the full array is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, iterates from bar 0 up to the target bar only.
    /// </summary>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetObv(int offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        if (_obvs.Count > 0)
            return _obvs[0].Values[targetIndex];

        // Slow path: compute inline up to targetIndex only.
        var obv = Volume(0);
        for (var i = 1; i <= targetIndex; i++)
        {
            if (Close(i) > Close(i - 1))
                obv += Volume(i);
            else if (Close(i) < Close(i - 1))
                obv -= Volume(i);
        }
        return obv;
    }

    /// <summary>
    /// Returns the OBV value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute OBV on.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetObv(IReadOnlyList<T> data, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetObv(offset);
}
