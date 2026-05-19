namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full CCI array for the context data.</summary>
    public double[] GetCci(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _ccis.Count; i++)
            if (_ccis[i].Period == period)
                return _ccis[i].Values;

        var values = new double[_data.Count];
        var typicalPrices = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
            typicalPrices[i] = (High(i) + Low(i) + Close(i)) / 3.0;

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sum = 0.0;

            for (var j = start; j <= i; j++)
                sum += typicalPrices[j];

            var sma = sum / count;
            var meanDeviation = 0.0;

            for (var j = start; j <= i; j++)
                meanDeviation += Math.Abs(typicalPrices[j] - sma);

            meanDeviation /= count;
            values[i] = meanDeviation > 0 ? (typicalPrices[i] - sma) / (0.015 * meanDeviation) : 0;
        }

        _ccis.Add(new Indicators.CciIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the CCI value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes only the window ending at the target bar.
    /// </summary>
    /// <param name="period">The CCI period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetCci(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _ccis.Count; i++)
            if (_ccis[i].Period == period)
                return _ccis[i].Values[targetIndex];

        // Slow path: CCI only needs the window ending at targetIndex — no prior state dependency.
        var start = Math.Max(0, targetIndex - period + 1);
        var count = targetIndex - start + 1;
        var sum = 0.0;

        for (var j = start; j <= targetIndex; j++)
            sum += (High(j) + Low(j) + Close(j)) / 3.0;

        var sma = sum / count;
        var tp = (High(targetIndex) + Low(targetIndex) + Close(targetIndex)) / 3.0;
        var meanDeviation = 0.0;

        for (var j = start; j <= targetIndex; j++)
            meanDeviation += Math.Abs((High(j) + Low(j) + Close(j)) / 3.0 - sma);

        meanDeviation /= count;
        return meanDeviation > 0 ? (tp - sma) / (0.015 * meanDeviation) : 0;
    }

    /// <summary>
    /// Returns the CCI value at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute CCI on.</param>
    /// <param name="period">The CCI period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetCci(IReadOnlyList<T> data, int period, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetCci(period, offset);
}
