namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetEma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _emas.Count; i++)
        {
            if (_emas[i].Period == period)
                return _emas[i].Values;
        }

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            var multiplier = 2.0 / (period + 1.0);
            var ema = Close(0);
            values[0] = ema;

            for (var i = 1; i < _data.Count; i++)
            {
                ema = ((Close(i) - ema) * multiplier) + ema;
                values[i] = ema;
            }
        }

        _emas.Add(new Indicators.EmaIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the EMA value for a specific point in time without caching.
    /// Only computes up to the target bar — no full array allocation.
    /// </summary>
    /// <param name="period">The EMA period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetEma(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        var multiplier = 2.0 / (period + 1.0);
        var ema = Close(0);
        for (var i = 1; i <= targetIndex; i++)
            ema = ((Close(i) - ema) * multiplier) + ema;

        return ema;
    }
}
