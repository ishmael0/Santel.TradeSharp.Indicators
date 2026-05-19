namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public double[] GetParabolicSar(double accelerationFactor = 0.02, double maxAcceleration = 0.2)
    {
        if (accelerationFactor <= 0) throw new ArgumentOutOfRangeException(nameof(accelerationFactor));
        if (maxAcceleration <= 0) throw new ArgumentOutOfRangeException(nameof(maxAcceleration));

        for (var i = 0; i < _parabolicSars.Count; i++)
        {
            var existing = _parabolicSars[i];
            if (existing.AccelerationFactor == accelerationFactor && existing.MaxAcceleration == maxAcceleration)
                return existing.Values;
        }

        var values = new double[_data.Count];

        if (_data.Count > 1)
        {
            var isUptrend = Close(1) > Close(0);
            var sar = isUptrend ? Low(0) : High(0);
            var ep = isUptrend ? High(1) : Low(1);
            var af = accelerationFactor;

            values[0] = sar;

            for (var i = 1; i < _data.Count; i++)
            {
                sar = sar + af * (ep - sar);

                if (isUptrend)
                {
                    if (i > 1)
                        sar = Math.Min(sar, Math.Min(Low(i - 1), Low(i - 2)));

                    if (Low(i) < sar)
                    {
                        isUptrend = false;
                        sar = ep;
                        ep = Low(i);
                        af = accelerationFactor;
                    }
                    else if (High(i) > ep)
                    {
                        ep = High(i);
                        af = Math.Min(af + accelerationFactor, maxAcceleration);
                    }
                }
                else
                {
                    if (i > 1)
                        sar = Math.Max(sar, Math.Max(High(i - 1), High(i - 2)));

                    if (High(i) > sar)
                    {
                        isUptrend = true;
                        sar = ep;
                        ep = High(i);
                        af = accelerationFactor;
                    }
                    else if (Low(i) < ep)
                    {
                        ep = Low(i);
                        af = Math.Min(af + accelerationFactor, maxAcceleration);
                    }
                }

                values[i] = sar;
            }
        }

        _parabolicSars.Add(new Indicators.ParabolicSarIndicator(accelerationFactor, maxAcceleration, values));
        return values;
    }
}
