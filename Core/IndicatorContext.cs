using Santel.TradeSharp.Indicators.Indicators;

namespace Santel.TradeSharp.Indicators;

public sealed class Candle
{
    public Candle(DateTime time, double open, double high, double low, double close, double volume = 0)
    {
        Time = time;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    public DateTime Time { get; }
    public double Open { get; }
    public double High { get; }
    public double Low { get; }
    public double Close { get; }
    public double Volume { get; }
}
public class IndicatorContext : IndicatorContext<Candle>
{
    public IndicatorContext(IReadOnlyList<Candle> data) : base(data, c => c.Time, c => c.Open, c => c.High, c => c.Low, c => c.Close, c => c.Volume)
    {
    }
}
public  class IndicatorContext<T>
{
    private readonly Func<T, DateTime> _time;
    private readonly Func<T, double> _open;
    private readonly Func<T, double> _high;
    private readonly Func<T, double> _low;
    private readonly Func<T, double> _close;
    private readonly Func<T, double> _volume;
    private readonly IReadOnlyList<T> _data;
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
    public IndicatorContext(
     IReadOnlyList<T> data,
     Func<T, DateTime> time,
     Func<T, double> open,
     Func<T, double> high,
     Func<T, double> low,
     Func<T, double> close,
     Func<T, double>? volume)
    {
        _data = data;
        _time = time;
        _open = open;
        _high = high;
        _low = low;
        _close = close;
        _volume = volume ?? (_ => 0);
    }
    public DateTime Time(int i) => _time(_data[i]);
    public double Open(int i) => _open(_data[i]);
    public double High(int i) => _high(_data[i]);
    public double Low(int i) => _low(_data[i]);
    public double Close(int i) => _close(_data[i]);
    public double Volume(int i) => _volume(_data[i]);
    //public IndicatorContext(IReadOnlyList<Candle> candles)
    //{
    //    _data = candles;
    //}

    //public IReadOnlyList<Candle> Candles => _data;
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

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            var multiplier = 2.0 / (period + 1.0);
            var ema = Close(0);
            values[0] = ema;

            for (var i = 1; i < _data.Count; i++)
            {
                ema = ((Close(i) - ema) * multiplier) + ema;
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

        var values = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sum = 0.0;

            for (var j = start; j <= i; j++)
            {
                sum += Close(j);
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

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            var trueRanges = new double[_data.Count];

            trueRanges[0] = High(0) - Low(0);

            for (var i = 1; i < _data.Count; i++)
            {
                var highLow = High(i) - Low(i);
                var highClose = Math.Abs(High(i) - Close(i - 1));
                var lowClose = Math.Abs(Low(i) - Close(i - 1));
                trueRanges[i] = Math.Max(highLow, Math.Max(highClose, lowClose));
            }

            var sum = 0.0;
            for (var i = 0; i < period && i < _data.Count; i++)
            {
                sum += trueRanges[i];
            }
            values[period - 1] = sum / period;

            for (var i = period; i < _data.Count; i++)
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

        var plusDm = new double[_data.Count];
        var minusDm = new double[_data.Count];

        for (var i = 1; i < _data.Count; i++)
        {
            var highDiff = High(i) - High(i - 1);
            var lowDiff = Low(i - 1) - Low(i);

            plusDm[i] = (highDiff > lowDiff && highDiff > 0) ? highDiff : 0;
            minusDm[i] = (lowDiff > highDiff && lowDiff > 0) ? lowDiff : 0;
        }

        var atr = GetAtr(period);
        var plusDi = new double[_data.Count];
        var minusDi = new double[_data.Count];

        var smoothedPlusDm = 0.0;
        var smoothedMinusDm = 0.0;

        for (var i = 0; i < period && i < _data.Count; i++)
        {
            smoothedPlusDm += plusDm[i];
            smoothedMinusDm += minusDm[i];
        }

        for (var i = period - 1; i < _data.Count; i++)
        {
            if (i > period - 1)
            {
                smoothedPlusDm = smoothedPlusDm - (smoothedPlusDm / period) + plusDm[i];
                smoothedMinusDm = smoothedMinusDm - (smoothedMinusDm / period) + minusDm[i];
            }

            plusDi[i] = atr[i] > 0 ? (smoothedPlusDm / atr[i]) * 100 : 0;
            minusDi[i] = atr[i] > 0 ? (smoothedMinusDm / atr[i]) * 100 : 0;
        }

        var dx = new double[_data.Count];
        for (var i = 0; i < _data.Count; i++)
        {
            var diSum = plusDi[i] + minusDi[i];
            dx[i] = diSum > 0 ? (Math.Abs(plusDi[i] - minusDi[i]) / diSum) * 100 : 0;
        }

        var adxValues = new double[_data.Count];
        var sum = 0.0;
        for (var i = period - 1; i < period * 2 - 1 && i < _data.Count; i++)
        {
            sum += dx[i];
        }
        if (_data.Count > period * 2 - 2)
        {
            adxValues[period * 2 - 2] = sum / period;

            for (var i = period * 2 - 1; i < _data.Count; i++)
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

        var values = new double[_data.Count];
        var typicalPrices = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            typicalPrices[i] = (High(i) + Low(i) + Close(i)) / 3.0;
        }

        for (var i = 0; i < _data.Count; i++)
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

        var values = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var high = double.MinValue;
            var low = double.MaxValue;

            for (var j = start; j <= i; j++)
            {
                if (High(j) > high) high = High(j);
                if (Low(j) < low) low = Low(j);
            }

            var range = high - low;
            values[i] = range > 0 ? ((high - Close(i)) / range) * -100 : 0;
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

        var values = new double[_data.Count];

        if (_data.Count > 0)
        {
            values[0] = Volume(0);

            for (var i = 1; i < _data.Count; i++)
            {
                if (Close(i) > Close(i - 1))
                {
                    values[i] = values[i - 1] + Volume(i);
                }
                else if (Close(i) < Close(i - 1))
                {
                    values[i] = values[i - 1] - Volume(i);
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
                    {
                        sar = Math.Min(sar, Math.Min(Low(i - 1), Low(i - 2)));
                    }

                    if (Low(i) < sar)
                    {
                        isUptrend = false;
                        sar = ep;
                        ep = Low(i);
                        af = accelerationFactor;
                    }
                    else
                    {
                        if (High(i) > ep)
                        {
                            ep = High(i);
                            af = Math.Min(af + accelerationFactor, maxAcceleration);
                        }
                    }
                }
                else
                {
                    if (i > 1)
                    {
                        sar = Math.Max(sar, Math.Max(High(i - 1), High(i - 2)));
                    }

                    if (High(i) > sar)
                    {
                        isUptrend = true;
                        sar = ep;
                        ep = High(i);
                        af = accelerationFactor;
                    }
                    else
                    {
                        if (Low(i) < ep)
                        {
                            ep = Low(i);
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

        var tenkanSen = new double[_data.Count];
        var kijunSen = new double[_data.Count];
        var senkouSpanA = new double[_data.Count];
        var senkouSpanB = new double[_data.Count];
        var chikouSpan = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var tenkanStart = Math.Max(0, i - tenkanPeriod + 1);
            var tenkanHigh = double.MinValue;
            var tenkanLow = double.MaxValue;
            for (var j = tenkanStart; j <= i; j++)
            {
                if (High(j) > tenkanHigh) tenkanHigh = High(j);
                if (Low(j) < tenkanLow) tenkanLow = Low(j);
            }
            tenkanSen[i] = (tenkanHigh + tenkanLow) / 2.0;

            var kijunStart = Math.Max(0, i - kijunPeriod + 1);
            var kijunHigh = double.MinValue;
            var kijunLow = double.MaxValue;
            for (var j = kijunStart; j <= i; j++)
            {
                if (High(j) > kijunHigh) kijunHigh = High(j);
                if (Low(j) < kijunLow) kijunLow = Low(j);
            }
            kijunSen[i] = (kijunHigh + kijunLow) / 2.0;

            var senkouBStart = Math.Max(0, i - senkouSpanBPeriod + 1);
            var senkouBHigh = double.MinValue;
            var senkouBLow = double.MaxValue;
            for (var j = senkouBStart; j <= i; j++)
            {
                if (High(j) > senkouBHigh) senkouBHigh = High(j);
                if (Low(j) < senkouBLow) senkouBLow = Low(j);
            }
            var senkouBValue = (senkouBHigh + senkouBLow) / 2.0;

            var senkouAValue = (tenkanSen[i] + kijunSen[i]) / 2.0;

            var senkouAIndex = i + kijunPeriod;
            if (senkouAIndex < _data.Count)
            {
                senkouSpanA[senkouAIndex] = senkouAValue;
            }

            var senkouBIndex = i + kijunPeriod;
            if (senkouBIndex < _data.Count)
            {
                senkouSpanB[senkouBIndex] = senkouBValue;
            }

            var chikouIndex = i - kijunPeriod;
            if (chikouIndex >= 0)
            {
                chikouSpan[chikouIndex] = Close(i);
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
        var upper = new double[_data.Count];
        var lower = new double[_data.Count];

        for (var i = 0; i < _data.Count; i++)
        {
            var start = Math.Max(0, i - period + 1);
            var count = i - start + 1;
            var sumSquares = 0.0;

            for (var j = start; j <= i; j++)
            {
                var diff = Close(j) - sma[i];
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

        var macdValues = new double[_data.Count];
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

        var histValues = new double[_data.Count];
        for (var i = 0; i < histValues.Length; i++)
        {
            histValues[i] = macdValues[i] - signalValues[i];
        }

        var series = new MacdSeries(macdValues, signalValues, histValues);
        _macds.Add(new MacdIndicator(fastPeriod, slowPeriod, signalPeriod, series));
        return series;
    }
}
