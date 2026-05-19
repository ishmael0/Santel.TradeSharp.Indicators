namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetMfi(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _mfis.Count; i++)
        {
            if (_mfis[i].Period == period)
                return _mfis[i].Values;
        }

        var values = new double[_data.Count];

        if (_data.Count > 1)
        {
            var typicalPrices = new double[_data.Count];
            var moneyFlows = new double[_data.Count];

            for (var i = 0; i < _data.Count; i++)
            {
                typicalPrices[i] = (High(i) + Low(i) + Close(i)) / 3.0;
                moneyFlows[i] = typicalPrices[i] * Volume(i);
            }

            for (var i = period; i < _data.Count; i++)
            {
                var positiveFlow = 0.0;
                var negativeFlow = 0.0;

                for (var j = i - period + 1; j <= i; j++)
                {
                    if (j > 0)
                    {
                        if (typicalPrices[j] > typicalPrices[j - 1])
                            positiveFlow += moneyFlows[j];
                        else if (typicalPrices[j] < typicalPrices[j - 1])
                            negativeFlow += moneyFlows[j];
                    }
                }

                if (negativeFlow == 0)
                    values[i] = 100;
                else
                    values[i] = 100 - (100 / (1 + positiveFlow / negativeFlow));
            }
        }

        _mfis.Add(new Indicators.MfiIndicator(period, values));
        return values;
    }
}
