namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetWilliamsR(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _williamsRs.Count; i++)
        {
            if (_williamsRs[i].Period == period)
                return _williamsRs[i].Values;
        }

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
}
