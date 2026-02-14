namespace Santel.TradeSharp.Indicators;

public sealed class IndicatorContext
{
    private readonly IReadOnlyList<Candle> _candles;
    private readonly List<EmaIndicator> _emas = new();
    private readonly List<MacdIndicator> _macds = new();

    public IndicatorContext(IReadOnlyList<Candle> candles)
    {
        _candles = candles;
    }

    public IReadOnlyList<Candle> Candles => _candles;
    public IReadOnlyList<EmaIndicator> Emas => _emas;
    public IReadOnlyList<MacdIndicator> Macds => _macds;

    public double[] GetEma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _emas.Count; i++)
        {
            if (_emas[i].Period == period)
            {
                return _emas[i].Values;
            }
        }

        var values = new double[_candles.Count];

        if (_candles.Count > 0)
        {
            var multiplier = 2.0 / (period + 1.0);
            var ema = _candles[0].Close;
            values[0] = ema;

            for (var i = 1; i < _candles.Count; i++)
            {
                ema = ((_candles[i].Close - ema) * multiplier) + ema;
                values[i] = ema;
            }
        }

        _emas.Add(new EmaIndicator(period, values));
        return values;
    }

    public MacdSeries GetMacd(int fastPeriod, int slowPeriod, int signalPeriod)
    {
        if (fastPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(fastPeriod));
        if (slowPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(slowPeriod));
        if (signalPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(signalPeriod));
        if (fastPeriod >= slowPeriod) throw new ArgumentException("fastPeriod must be less than slowPeriod.");

        for (var i = 0; i < _macds.Count; i++)
        {
            var existing = _macds[i];
            if (existing.FastPeriod == fastPeriod && existing.SlowPeriod == slowPeriod && existing.SignalPeriod == signalPeriod)
            {
                return existing.Series;
            }
        }

        var fastEma = GetEma(fastPeriod);
        var slowEma = GetEma(slowPeriod);

        var macdValues = new double[_candles.Count];
        for (var i = 0; i < macdValues.Length; i++)
        {
            macdValues[i] = fastEma[i] - slowEma[i];
        }

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

        var histValues = new double[_candles.Count];
        for (var i = 0; i < histValues.Length; i++)
        {
            histValues[i] = macdValues[i] - signalValues[i];
        }

        var series = new MacdSeries(macdValues, signalValues, histValues);
        _macds.Add(new MacdIndicator(fastPeriod, slowPeriod, signalPeriod, series));
        return series;
    }
}
