namespace Santel.TradeSharp.Indicators;

public sealed class IndicatorContext
{
    private readonly IReadOnlyList<Candle> _candles;
    private readonly List<EmaIndicator> _emas = new();
    private readonly List<SmaIndicator> _smas = new();
    private readonly List<RsiIndicator> _rsis = new();
    private readonly List<AtrIndicator> _atrs = new();
    private readonly List<AdxIndicator> _adxs = new();
    private readonly List<CciIndicator> _ccis = new();
    private readonly List<WilliamsRIndicator> _williamsRs = new();
    private readonly List<BollingerBandsIndicator> _bollingerBands = new();
    private readonly List<StochasticIndicator> _stochastics = new();
    private readonly List<MacdIndicator> _macds = new();

    public IndicatorContext(IReadOnlyList<Candle> candles)
    {
        _candles = candles;
    }

    public IReadOnlyList<Candle> Candles => _candles;
    public IReadOnlyList<EmaIndicator> Emas => _emas;
    public IReadOnlyList<SmaIndicator> Smas => _smas;
    public IReadOnlyList<RsiIndicator> Rsis => _rsis;
    public IReadOnlyList<AtrIndicator> Atrs => _atrs;
    public IReadOnlyList<AdxIndicator> Adxs => _adxs;
    public IReadOnlyList<CciIndicator> Ccis => _ccis;
    public IReadOnlyList<WilliamsRIndicator> WilliamsRs => _williamsRs;
    public IReadOnlyList<BollingerBandsIndicator> BollingerBands => _bollingerBands;
    public IReadOnlyList<StochasticIndicator> Stochastics => _stochastics;
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

    public double[] GetAtr(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _atrs.Count; i++)
        {
            if (_atrs[i].Period == period)
            {
                return _atrs[i].Values;
            }
        }

        var values = new double[_candles.Count];

        if (_candles.Count > 0)
        {
            var trueRanges = new double[_candles.Count];

            trueRanges[0] = _candles[0].High - _candles[0].Low;

            for (var i = 1; i < _candles.Count; i++)
            {
                var highLow = _candles[i].High - _candles[i].Low;
                var highClose = Math.Abs(_candles[i].High - _candles[i - 1].Close);
                var lowClose = Math.Abs(_candles[i].Low - _candles[i - 1].Close);
                trueRanges[i] = Math.Max(highLow, Math.Max(highClose, lowClose));
            }

            var sum = 0.0;
            for (var i = 0; i < period && i < _candles.Count; i++)
            {
                sum += trueRanges[i];
            }
            values[period - 1] = sum / period;

            for (var i = period; i < _candles.Count; i++)
            {
                values[i] = (values[i - 1] * (period - 1) + trueRanges[i]) / period;
            }
        }

        _atrs.Add(new AtrIndicator(period, values));
        return values;
    }

    public Adx GetAdx(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _adxs.Count; i++)
        {
            if (_adxs[i].Period == period)
            {
                return _adxs[i].Adx;
            }
        }

        var plusDm = new double[_candles.Count];
        var minusDm = new double[_candles.Count];

        for (var i = 1; i < _candles.Count; i++)
        {
            var highDiff = _candles[i].High - _candles[i - 1].High;
            var lowDiff = _candles[i - 1].Low - _candles[i].Low;

            plusDm[i] = (highDiff > lowDiff && highDiff > 0) ? highDiff : 0;
            minusDm[i] = (lowDiff > highDiff && lowDiff > 0) ? lowDiff : 0;
        }

        var atr = GetAtr(period);
        var plusDi = new double[_candles.Count];
        var minusDi = new double[_candles.Count];

        var smoothedPlusDm = 0.0;
        var smoothedMinusDm = 0.0;

        for (var i = 0; i < period && i < _candles.Count; i++)
        {
            smoothedPlusDm += plusDm[i];
            smoothedMinusDm += minusDm[i];
        }

        for (var i = period - 1; i < _candles.Count; i++)
        {
            if (i > period - 1)
            {
                smoothedPlusDm = smoothedPlusDm - (smoothedPlusDm / period) + plusDm[i];
                smoothedMinusDm = smoothedMinusDm - (smoothedMinusDm / period) + minusDm[i];
            }

            plusDi[i] = atr[i] > 0 ? (smoothedPlusDm / atr[i]) * 100 : 0;
            minusDi[i] = atr[i] > 0 ? (smoothedMinusDm / atr[i]) * 100 : 0;
        }

        var dx = new double[_candles.Count];
        for (var i = 0; i < _candles.Count; i++)
        {
            var diSum = plusDi[i] + minusDi[i];
            dx[i] = diSum > 0 ? (Math.Abs(plusDi[i] - minusDi[i]) / diSum) * 100 : 0;
        }

        var adxValues = new double[_candles.Count];
        var sum = 0.0;
        for (var i = period - 1; i < period * 2 - 1 && i < _candles.Count; i++)
        {
            sum += dx[i];
        }
        if (_candles.Count > period * 2 - 2)
        {
            adxValues[period * 2 - 2] = sum / period;

            for (var i = period * 2 - 1; i < _candles.Count; i++)
            {
                adxValues[i] = (adxValues[i - 1] * (period - 1) + dx[i]) / period;
            }
        }

        var adx = new Adx(adxValues, plusDi, minusDi);
        _adxs.Add(new AdxIndicator(period, adx));
        return adx;
    }

    public double[] GetCci(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _ccis.Count; i++)
        {
            if (_ccis[i].Period == period)
            {
                return _ccis[i].Values;
            }
        }

        var values = new double[_candles.Count];
        var typicalPrices = new double[_candles.Count];

        for (var i = 0; i < _candles.Count; i++)
        {
            typicalPrices[i] = (_candles[i].High + _candles[i].Low + _candles[i].Close) / 3.0;
        }

        for (var i = 0; i < _candles.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sum = 0.0;

            for (var j = start; j <= i; j++)
            {
                sum += typicalPrices[j];
            }

            var sma = sum / count;
            var meanDeviation = 0.0;

            for (var j = start; j <= i; j++)
            {
                meanDeviation += Math.Abs(typicalPrices[j] - sma);
            }
            meanDeviation /= count;

            values[i] = meanDeviation > 0 ? (typicalPrices[i] - sma) / (0.015 * meanDeviation) : 0;
        }

        _ccis.Add(new CciIndicator(period, values));
        return values;
    }

    public double[] GetWilliamsR(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _williamsRs.Count; i++)
        {
            if (_williamsRs[i].Period == period)
            {
                return _williamsRs[i].Values;
            }
        }

        var values = new double[_candles.Count];

        for (var i = 0; i < _candles.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var high = double.MinValue;
            var low = double.MaxValue;

            for (var j = start; j <= i; j++)
            {
                if (_candles[j].High > high) high = _candles[j].High;
                if (_candles[j].Low < low) low = _candles[j].Low;
            }

            var range = high - low;
            values[i] = range > 0 ? ((high - _candles[i].Close) / range) * -100 : 0;
        }

        _williamsRs.Add(new WilliamsRIndicator(period, values));
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

    public Stochastic GetStochastic(int kPeriod, int dPeriod)
    {
        if (kPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(kPeriod));
        if (dPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(dPeriod));

        for (var i = 0; i < _stochastics.Count; i++)
        {
            var existing = _stochastics[i];
            if (existing.KPeriod == kPeriod && existing.DPeriod == dPeriod)
            {
                return existing.Stochastic;
            }
        }

        var kValues = new double[_candles.Count];

        for (var i = 0; i < _candles.Count; i++)
        {
            var start = Math.Max(0, i - kPeriod + 1);
            var high = double.MinValue;
            var low = double.MaxValue;

            for (var j = start; j <= i; j++)
            {
                if (_candles[j].High > high) high = _candles[j].High;
                if (_candles[j].Low < low) low = _candles[j].Low;
            }

            var range = high - low;
            kValues[i] = range > 0 ? ((_candles[i].Close - low) / range) * 100 : 0;
        }

        var dValues = new double[_candles.Count];
        for (var i = 0; i < _candles.Count; i++)
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

        var stochastic = new Stochastic(kValues, dValues);
        _stochastics.Add(new StochasticIndicator(kPeriod, dPeriod, stochastic));
        return stochastic;
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
