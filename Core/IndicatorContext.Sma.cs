namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetSma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _smas.Count; i++)
        {
            if (_smas[i].Period == period)
                return _smas[i].Values;
        }

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
}
