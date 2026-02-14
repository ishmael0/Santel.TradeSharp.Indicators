namespace Santel.TradeSharp.Indicators;

public sealed class IndicatorContext
{
    private readonly IReadOnlyList<Candle> _candles;
    private readonly List<EmaIndicator> _emas = new();
    private readonly List<SmaIndicator> _smas = new();
    private readonly List<RsiIndicator> _rsis = new();
    private readonly List<BollingerBandsIndicator> _bollingerBands = new();
    private readonly List<MacdIndicator> _macds = new();

    public IndicatorContext(IReadOnlyList<Candle> candles)
    {
        _candles = candles;
    }

    public IReadOnlyList<Candle> Candles => _candles;
    public IReadOnlyList<EmaIndicator> Emas => _emas;
    public IReadOnlyList<SmaIndicator> Smas => _smas;
    public IReadOnlyList<RsiIndicator> Rsis => _rsis;
    public IReadOnlyList<BollingerBandsIndicator> BollingerBands => _bollingerBands;
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

    public double[] GetSma(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _smas.Count; i++)
        {
            if (_smas[i].Period == period)
            {
                return _smas[i].Values;
            }
        }

        var values = new double[_candles.Count];

        for (var i = 0; i < _candles.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sum = 0.0;

            for (var j = start; j <= i; j++)
            {
                sum += _candles[j].Close;
            }

            values[i] = sum / count;
        }

        _smas.Add(new SmaIndicator(period, values));
        return values;
    }

    public double[] GetRsi(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _rsis.Count; i++)
        {
            if (_rsis[i].Period == period)
            {
                return _rsis[i].Values;
            }
        }

        var values = new double[_candles.Count];

        if (_candles.Count > 1)
        {
            var gains = new double[_candles.Count];
            var losses = new double[_candles.Count];

            for (var i = 1; i < _candles.Count; i++)
            {
                var change = _candles[i].Close - _candles[i - 1].Close;
                gains[i] = change > 0 ? change : 0;
                losses[i] = change < 0 ? -change : 0;
            }

            var avgGain = 0.0;
            var avgLoss = 0.0;

            for (var i = 1; i <= period && i < _candles.Count; i++)
            {
                avgGain += gains[i];
                avgLoss += losses[i];
            }
            avgGain /= period;
            avgLoss /= period;

            values[period] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));

            for (var i = period + 1; i < _candles.Count; i++)
            {
                avgGain = (avgGain * (period - 1) + gains[i]) / period;
                avgLoss = (avgLoss * (period - 1) + losses[i]) / period;
                values[i] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));
            }
        }

        _rsis.Add(new RsiIndicator(period, values));
        return values;
    }

    public BollingerBands GetBollingerBands(int period, double standardDeviation = 2.0)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (standardDeviation <= 0) throw new ArgumentOutOfRangeException(nameof(standardDeviation));

        for (var i = 0; i < _bollingerBands.Count; i++)
        {
            var existing = _bollingerBands[i];
            if (existing.Period == period && existing.StandardDeviation == standardDeviation)
            {
                return existing.Bands;
            }
        }

        var sma = GetSma(period);
        var upper = new double[_candles.Count];
        var lower = new double[_candles.Count];

        for (var i = 0; i < _candles.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sumSquares = 0.0;

            for (var j = start; j <= i; j++)
            {
                var diff = _candles[j].Close - sma[i];
                sumSquares += diff * diff;
            }

            var stdDev = Math.Sqrt(sumSquares / count);
            upper[i] = sma[i] + (stdDev * standardDeviation);
            lower[i] = sma[i] - (stdDev * standardDeviation);
        }

        var bands = new BollingerBands(upper, sma, lower);
        _bollingerBands.Add(new BollingerBandsIndicator(period, standardDeviation, bands));
        return bands;
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
