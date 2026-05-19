namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.MacdSeries GetMacd(int fastPeriod, int slowPeriod, int signalPeriod)
    {
        if (fastPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(fastPeriod));
        if (slowPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(slowPeriod));
        if (signalPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(signalPeriod));
        if (fastPeriod >= slowPeriod) throw new ArgumentException("fastPeriod must be less than slowPeriod.");

        for (var i = 0; i < _macds.Count; i++)
        {
            var existing = _macds[i];
            if (existing.FastPeriod == fastPeriod && existing.SlowPeriod == slowPeriod && existing.SignalPeriod == signalPeriod)
                return existing.Series;
        }

        var fastEma = GetEma(fastPeriod);
        var slowEma = GetEma(slowPeriod);

        var macdValues = new double[_data.Count];
        for (var i = 0; i < macdValues.Length; i++)
            macdValues[i] = fastEma[i] - slowEma[i];

        var signalValues = new double[macdValues.Length];
        if (macdValues.Length > 0)
        {
            var multiplier = 2.0 / (signalPeriod + 1.0);
            var ema = macdValues[0];
            signalValues[0] = ema;

            for (var i = 1; i < macdValues.Length; i++)
            {
                ema = ((macdValues[i] - ema) * multiplier) + ema;
                signalValues[i] = ema;
            }
        }

        var histValues = new double[_data.Count];
        for (var i = 0; i < histValues.Length; i++)
            histValues[i] = macdValues[i] - signalValues[i];

        var series = new Indicators.MacdSeries(macdValues, signalValues, histValues);
        _macds.Add(new Indicators.MacdIndicator(fastPeriod, slowPeriod, signalPeriod, series));
        return series;
    }
}
