namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetObv()
    {
        if (_obvs.Count > 0)
            return _obvs[0].Values;

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            values[0] = Volume(0);

            for (var i = 1; i < _data.Count; i++)
            {
                if (Close(i) > Close(i - 1))
                    values[i] = values[i - 1] + Volume(i);
                else if (Close(i) < Close(i - 1))
                    values[i] = values[i - 1] - Volume(i);
                else
                    values[i] = values[i - 1];
            }
        }

        _obvs.Add(new Indicators.ObvIndicator(values));
        return values;
    }
}
