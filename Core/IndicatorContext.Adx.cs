namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.Adx GetAdx(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _adxs.Count; i++)
        {
            if (_adxs[i].Period == period)
                return _adxs[i].Adx;
        }

        var plusDm = new double[_data.Count];
        var minusDm = new double[_data.Count];

        for (var i = 1; i < _data.Count; i++)
        {
            var highDiff = High(i) - High(i - 1);
            var lowDiff = Low(i - 1) - Low(i);
            plusDm[i] = (highDiff > lowDiff && highDiff > 0) ? highDiff : 0;
            minusDm[i] = (lowDiff > highDiff && lowDiff > 0) ? lowDiff : 0;
        }

        var atr = GetAtr(period);
        var plusDi = new double[_data.Count];
        var minusDi = new double[_data.Count];
        var smoothedPlusDm = 0.0;
        var smoothedMinusDm = 0.0;

        for (var i = 0; i < period && i < _data.Count; i++)
        {
            smoothedPlusDm += plusDm[i];
            smoothedMinusDm += minusDm[i];
        }

        for (var i = period - 1; i < _data.Count; i++)
        {
            if (i > period - 1)
            {
                smoothedPlusDm = smoothedPlusDm - (smoothedPlusDm / period) + plusDm[i];
                smoothedMinusDm = smoothedMinusDm - (smoothedMinusDm / period) + minusDm[i];
            }
            plusDi[i] = atr[i] > 0 ? (smoothedPlusDm / atr[i]) * 100 : 0;
            minusDi[i] = atr[i] > 0 ? (smoothedMinusDm / atr[i]) * 100 : 0;
        }

        var dx = new double[_data.Count];
        for (var i = 0; i < _data.Count; i++)
        {
            var diSum = plusDi[i] + minusDi[i];
            dx[i] = diSum > 0 ? (Math.Abs(plusDi[i] - minusDi[i]) / diSum) * 100 : 0;
        }

        var adxValues = new double[_data.Count];
        var sum = 0.0;
        for (var i = period - 1; i < period * 2 - 1 && i < _data.Count; i++)
            sum += dx[i];

        if (_data.Count > period * 2 - 2)
        {
            adxValues[period * 2 - 2] = sum / period;
            for (var i = period * 2 - 1; i < _data.Count; i++)
                adxValues[i] = (adxValues[i - 1] * (period - 1) + dx[i]) / period;
        }

        var adx = new Indicators.Adx(adxValues, plusDi, minusDi);
        _adxs.Add(new Indicators.AdxIndicator(period, adx));
        return adx;
    }

    /// <summary>
    /// Returns ADX, +DI and -DI values for a specific point in time.
    /// If the full series for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes the full series and caches it before indexing.
    /// </summary>
    /// <param name="period">The ADX period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    /// <returns>A tuple of (Adx, PlusDi, MinusDi) at the target bar.</returns>
    public (double Adx, double PlusDi, double MinusDi) GetAdx(int period, int offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        var adx = GetAdx(period); // uses cache if available
        return (adx.Values[targetIndex], adx.PlusDi[targetIndex], adx.MinusDi[targetIndex]);
    }

    /// <summary>
    /// Returns ADX, +DI and -DI values at <paramref name="offset"/> bars back,
    /// computed over a custom data list instead of the context data.
    /// </summary>
    /// <param name="data">External bar data to compute ADX on.</param>
    /// <param name="period">The ADX period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public (double Adx, double PlusDi, double MinusDi) GetAdx(IReadOnlyList<T> data, int period, int offset)
        => new IndicatorContext<T>(data, _time, _open, _high, _low, _close, _volume).GetAdx(period, offset);
}
