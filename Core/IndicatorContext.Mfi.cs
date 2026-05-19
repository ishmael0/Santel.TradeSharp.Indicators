namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    /// <summary>Returns (and caches) the full MFI array for the context data.</summary>
    public double[] GetMfi(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));

        for (var i = 0; i < _mfis.Count; i++)
            if (_mfis[i].Period == period)
                return _mfis[i].Values;

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
                            positiveFlow += moneyFlows[j];
                        else if (typicalPrices[j] < typicalPrices[j - 1])
                            negativeFlow += moneyFlows[j];
                    }
                }

                if (negativeFlow == 0)
                    values[i] = 100;
                else
                    values[i] = 100 - (100 / (1 + positiveFlow / negativeFlow));
            }
        }

        _mfis.Add(new Indicators.MfiIndicator(period, values));
        return values;
    }

    /// <summary>
    /// Returns the MFI value for a specific point in time.
    /// If the full array for this period is already cached, the result is a direct index lookup (O(1)).
    /// Otherwise, computes only the window ending at the target bar.
    /// </summary>
    /// <param name="period">The MFI period.</param>
    /// <param name="offset">How many bars back from the latest bar. 0 = current bar, 1 = one bar ago, etc.</param>
    public double GetMfi(int period, int offset)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

        var targetIndex = _data.Count - 1 - offset;
        if (targetIndex < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset exceeds available data.");

        // Fast path: full array already cached — just index into it.
        for (var i = 0; i < _mfis.Count; i++)
            if (_mfis[i].Period == period)
                return _mfis[i].Values[targetIndex];

        // Slow path: MFI only needs the window ending at targetIndex.
        if (targetIndex < period) return 0;

        var positiveFlow = 0.0;
        var negativeFlow = 0.0;

        for (var j = targetIndex - period + 1; j <= targetIndex; j++)
        {
            if (j > 0)
            {
                var tp = (High(j) + Low(j) + Close(j)) / 3.0;
                var tpPrev = (High(j - 1) + Low(j - 1) + Close(j - 1)) / 3.0;
                var mf = tp * Volume(j);

                if (tp > tpPrev) positiveFlow += mf;
                else if (tp < tpPrev) negativeFlow += mf;
            }
        }

        return negativeFlow == 0 ? 100 : 100 - (100 / (1 + positiveFlow / negativeFlow));
    }
}
