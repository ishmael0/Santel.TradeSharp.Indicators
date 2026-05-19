namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.Stochastic GetStochastic(int kPeriod, int dPeriod)
    {
        if (kPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(kPeriod));
        if (dPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(dPeriod));

        for (var i = 0; i < _stochastics.Count; i++)
        {
            var existing = _stochastics[i];
            if (existing.KPeriod == kPeriod && existing.DPeriod == dPeriod)
                return existing.Stochastic;
        }

        var kValues = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - kPeriod + 1);
            var high = double.MinValue;
            var low = double.MaxValue;

            for (var j = start; j <= i; j++)
            {
                if (High(j) > high) high = High(j);
                if (Low(j) < low) low = Low(j);
            }

            var range = high - low;
            kValues[i] = range > 0 ? ((Close(i) - low) / range) * 100 : 0;
        }

        var dValues = new double[_data.Count];
        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - dPeriod + 1);
            var sum = 0.0;
            var count = 0;

            for (var j = start; j <= i; j++)
            {
                sum += kValues[j];
                count++;
            }

            dValues[i] = count > 0 ? sum / count : 0;
        }

        var stochastic = new Indicators.Stochastic(kValues, dValues);
        _stochastics.Add(new Indicators.StochasticIndicator(kPeriod, dPeriod, stochastic));
        return stochastic;
    }
}
