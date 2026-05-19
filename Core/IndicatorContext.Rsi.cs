namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetRsi(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _rsis.Count; i++)
        {
            if (_rsis[i].Period == period)
                return _rsis[i].Values;
        }

        var values = new double[_data.Count];

        if (_data.Count > 1)
        {
            var gains = new double[_data.Count];
            var losses = new double[_data.Count];

            for (var i = 1; i < _data.Count; i++)
            {
                var change = Close(i) - Close(i - 1);
                gains[i] = change > 0 ? change : 0;
                losses[i] = change < 0 ? -change : 0;
            }

            var avgGain = 0.0;
            var avgLoss = 0.0;

            for (var i = 1; i <= period && i < _data.Count; i++)
            {
                avgGain += gains[i];
                avgLoss += losses[i];
            }
            avgGain /= period;
            avgLoss /= period;

            values[period] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));

            for (var i = period + 1; i < _data.Count; i++)
            {
                avgGain = (avgGain * (period - 1) + gains[i]) / period;
                avgLoss = (avgLoss * (period - 1) + losses[i]) / period;
                values[i] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));
            }
        }

        _rsis.Add(new Indicators.RsiIndicator(period, values));
        return values;
    }
}
