namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.BollingerBands GetBollingerBands(int period, double standardDeviation = 2.0)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (standardDeviation <= 0) throw new ArgumentOutOfRangeException(nameof(standardDeviation));

        for (var i = 0; i < _bollingerBands.Count; i++)
        {
            var existing = _bollingerBands[i];
            if (existing.Period == period && existing.StandardDeviation == standardDeviation)
                return existing.Bands;
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

        var bands = new Indicators.BollingerBands(upper, sma, lower);
        _bollingerBands.Add(new Indicators.BollingerBandsIndicator(period, standardDeviation, bands));
        return bands;
    }
}
