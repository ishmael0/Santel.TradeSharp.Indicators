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
}
