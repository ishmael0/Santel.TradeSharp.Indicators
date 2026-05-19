namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetAtr(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _atrs.Count; i++)
        {
            if (_atrs[i].Period == period)
                return _atrs[i].Values;
        }

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
}
