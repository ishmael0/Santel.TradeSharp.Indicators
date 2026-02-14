namespace Santel.TradeSharp.Indicators;

public static class IndicatorCalculators
{
    public static IndicatorSeries<double> CalculateEma(IReadOnlyList<Candle> candles, int period)
    {
        var values = new double[candles.Count];
        var name = $"EMA({period})";

        if (candles.Count == 0)
        {
            return new IndicatorSeries<double>(name, values);
        }

        var multiplier = 2.0 / (period + 1.0);
        var ema = candles[0].Close;
        values[0] = ema;

        for (var i = 1; i < candles.Count; i++)
        {
            ema = ((candles[i].Close - ema) * multiplier) + ema;
            values[i] = ema;
        }

        return new IndicatorSeries<double>(name, values);
    }

    public static MacdSeries CalculateMacd(IndicatorContext context, int fastPeriod, int slowPeriod, int signalPeriod)
    {
        var fastEma = context.CalculateEma(fastPeriod);
        var slowEma = context.CalculateEma(slowPeriod);

        var macdValues = new double[context.Candles.Count];
        for (var i = 0; i < macdValues.Length; i++)
        {
            macdValues[i] = fastEma.Values[i] - slowEma.Values[i];
        }

        var macdSeries = new IndicatorSeries<double>($"MACD({fastPeriod},{slowPeriod})", macdValues);
        var signalSeries = CalculateEmaFromSeries(macdSeries, signalPeriod);

        var histValues = new double[context.Candles.Count];
        for (var i = 0; i < histValues.Length; i++)
        {
            histValues[i] = macdValues[i] - signalSeries.Values[i];
        }

        var histSeries = new IndicatorSeries<double>($"MACD-H({fastPeriod},{slowPeriod},{signalPeriod})", histValues);
        return new MacdSeries(macdSeries, signalSeries, histSeries);
    }

    private static IndicatorSeries<double> CalculateEmaFromSeries(IndicatorSeries<double> source, int period)
    {
        var values = new double[source.Values.Length];
        if (values.Length == 0)
        {
            return new IndicatorSeries<double>($"EMA({period})", values);
        }

        var multiplier = 2.0 / (period + 1.0);
        var ema = source.Values[0];
        values[0] = ema;

        for (var i = 1; i < values.Length; i++)
        {
            ema = ((source.Values[i] - ema) * multiplier) + ema;
            values[i] = ema;
        }

        return new IndicatorSeries<double>($"EMA({period})", values);
    }
}
