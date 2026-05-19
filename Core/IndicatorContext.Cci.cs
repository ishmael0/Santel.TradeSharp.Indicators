namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetCci(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _ccis.Count; i++)
        {
            if (_ccis[i].Period == period)
                return _ccis[i].Values;
        }

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
}
