using Santel.TradeSharp.Indicators.Indicators;
using Santel.TradeSharp.Indicators.Models;

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
    private readonly List<ObvIndicator> _obvs = new();
    private readonly List<MfiIndicator> _mfis = new();
    private readonly List<ParabolicSarIndicator> _parabolicSars = new();
    private readonly List<IchimokuIndicator> _ichimokus = new();
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
    public IReadOnlyList<ObvIndicator> Obvs => _obvs;
    public IReadOnlyList<MfiIndicator> Mfis => _mfis;
    public IReadOnlyList<ParabolicSarIndicator> ParabolicSars => _parabolicSars;
    public IReadOnlyList<IchimokuIndicator> Ichimokus => _ichimokus;
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

    public double[] GetObv()
    {
        if (_obvs.Count > 0)
        {
            return _obvs[0].Values;
        }

        var values = new double[_candles.Count];

        if (_candles.Count > 0)
        {
            values[0] = _candles[0].Volume;

            for (var i = 1; i < _candles.Count; i++)
            {
                if (_candles[i].Close > _candles[i - 1].Close)
                {
                    values[i] = values[i - 1] + _candles[i].Volume;
                }
                else if (_candles[i].Close < _candles[i - 1].Close)
                {
                    values[i] = values[i - 1] - _candles[i].Volume;
                }
                else
                {
                    values[i] = values[i - 1];
                }
            }
        }

        _obvs.Add(new ObvIndicator(values));
        return values;
    }

    public double[] GetMfi(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _mfis.Count; i++)
        {
            if (_mfis[i].Period == period)
            {
                return _mfis[i].Values;
            }
        }

        var values = new double[_candles.Count];

        if (_candles.Count > 1)
        {
            var typicalPrices = new double[_candles.Count];
            var moneyFlows = new double[_candles.Count];

            for (var i = 0; i < _candles.Count; i++)
            {
                typicalPrices[i] = (_candles[i].High + _candles[i].Low + _candles[i].Close) / 3.0;
                moneyFlows[i] = typicalPrices[i] * _candles[i].Volume;
            }

            for (var i = period; i < _candles.Count; i++)
            {
                var positiveFlow = 0.0;
                var negativeFlow = 0.0;

                for (var j = i - period + 1; j <= i; j++)
                {
                    if (j > 0)
                    {
                        if (typicalPrices[j] > typicalPrices[j - 1])
                        {
                            positiveFlow += moneyFlows[j];
                        }
                        else if (typicalPrices[j] < typicalPrices[j - 1])
                        {
                            negativeFlow += moneyFlows[j];
                        }
                    }
                }

                if (negativeFlow == 0)
                {
                    values[i] = 100;
                }
                else
                {
                    var moneyFlowRatio = positiveFlow / negativeFlow;
                    values[i] = 100 - (100 / (1 + moneyFlowRatio));
                }
            }
        }

        _mfis.Add(new MfiIndicator(period, values));
        return values;
    }

    public double[] GetParabolicSar(double accelerationFactor = 0.02, double maxAcceleration = 0.2)
    {
        if (accelerationFactor <= 0) throw new ArgumentOutOfRangeException(nameof(accelerationFactor));
        if (maxAcceleration <= 0) throw new ArgumentOutOfRangeException(nameof(maxAcceleration));

        for (var i = 0; i < _parabolicSars.Count; i++)
        {
            var existing = _parabolicSars[i];
            if (existing.AccelerationFactor == accelerationFactor && existing.MaxAcceleration == maxAcceleration)
            {
                return existing.Values;
            }
        }

        var values = new double[_candles.Count];

        if (_candles.Count > 1)
        {
            var isUptrend = _candles[1].Close > _candles[0].Close;
            var sar = isUptrend ? _candles[0].Low : _candles[0].High;
            var ep = isUptrend ? _candles[1].High : _candles[1].Low;
            var af = accelerationFactor;

            values[0] = sar;

            for (var i = 1; i < _candles.Count; i++)
            {
                sar = sar + af * (ep - sar);

                if (isUptrend)
                {
                    if (i > 1)
                    {
                        sar = Math.Min(sar, Math.Min(_candles[i - 1].Low, _candles[i - 2].Low));
                    }

                    if (_candles[i].Low < sar)
                    {
                        isUptrend = false;
                        sar = ep;
                        ep = _candles[i].Low;
                        af = accelerationFactor;
                    }
                    else
                    {
                        if (_candles[i].High > ep)
                        {
                            ep = _candles[i].High;
                            af = Math.Min(af + accelerationFactor, maxAcceleration);
                        }
                    }
                }
                else
                {
                    if (i > 1)
                    {
                        sar = Math.Max(sar, Math.Max(_candles[i - 1].High, _candles[i - 2].High));
                    }

                    if (_candles[i].High > sar)
                    {
                        isUptrend = true;
                        sar = ep;
                        ep = _candles[i].High;
                        af = accelerationFactor;
                    }
                    else
                    {
                        if (_candles[i].Low < ep)
                        {
                            ep = _candles[i].Low;
                            af = Math.Min(af + accelerationFactor, maxAcceleration);
                        }
                    }
                }

                values[i] = sar;
            }
        }

        _parabolicSars.Add(new ParabolicSarIndicator(accelerationFactor, maxAcceleration, values));
        return values;
    }

    public Ichimoku GetIchimoku(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
    {
        if (tenkanPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(tenkanPeriod));
        if (kijunPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(kijunPeriod));
        if (senkouSpanBPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(senkouSpanBPeriod));

        for (var i = 0; i < _ichimokus.Count; i++)
        {
            var existing = _ichimokus[i];
            if (existing.TenkanPeriod == tenkanPeriod && existing.KijunPeriod == kijunPeriod && existing.SenkouSpanBPeriod == senkouSpanBPeriod)
            {
                return existing.Ichimoku;
            }
        }

        var tenkanSen = new double[_candles.Count];
        var kijunSen = new double[_candles.Count];
        var senkouSpanA = new double[_candles.Count];
        var senkouSpanB = new double[_candles.Count];
        var chikouSpan = new double[_candles.Count];

        for (var i = 0; i < _candles.Count; i++)
        {
            var tenkanStart = Math.Max(0, i - tenkanPeriod + 1);
            var tenkanHigh = double.MinValue;
            var tenkanLow = double.MaxValue;
            for (var j = tenkanStart; j <= i; j++)
            {
                if (_candles[j].High > tenkanHigh) tenkanHigh = _candles[j].High;
                if (_candles[j].Low < tenkanLow) tenkanLow = _candles[j].Low;
            }
            tenkanSen[i] = (tenkanHigh + tenkanLow) / 2.0;

            var kijunStart = Math.Max(0, i - kijunPeriod + 1);
            var kijunHigh = double.MinValue;
            var kijunLow = double.MaxValue;
            for (var j = kijunStart; j <= i; j++)
            {
                if (_candles[j].High > kijunHigh) kijunHigh = _candles[j].High;
                if (_candles[j].Low < kijunLow) kijunLow = _candles[j].Low;
            }
            kijunSen[i] = (kijunHigh + kijunLow) / 2.0;

            var senkouBStart = Math.Max(0, i - senkouSpanBPeriod + 1);
            var senkouBHigh = double.MinValue;
            var senkouBLow = double.MaxValue;
            for (var j = senkouBStart; j <= i; j++)
            {
                if (_candles[j].High > senkouBHigh) senkouBHigh = _candles[j].High;
                if (_candles[j].Low < senkouBLow) senkouBLow = _candles[j].Low;
            }
            var senkouBValue = (senkouBHigh + senkouBLow) / 2.0;

            var senkouAValue = (tenkanSen[i] + kijunSen[i]) / 2.0;

            var senkouAIndex = i + kijunPeriod;
            if (senkouAIndex < _candles.Count)
            {
                senkouSpanA[senkouAIndex] = senkouAValue;
            }

            var senkouBIndex = i + kijunPeriod;
            if (senkouBIndex < _candles.Count)
            {
                senkouSpanB[senkouBIndex] = senkouBValue;
            }

            var chikouIndex = i - kijunPeriod;
            if (chikouIndex >= 0)
            {
                chikouSpan[chikouIndex] = _candles[i].Close;
            }
        }

        var ichimoku = new Ichimoku(tenkanSen, kijunSen, senkouSpanA, senkouSpanB, chikouSpan);
        _ichimokus.Add(new IchimokuIndicator(tenkanPeriod, kijunPeriod, senkouSpanBPeriod, ichimoku));
        return ichimoku;
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
