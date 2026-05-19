namespace Santel.TradeSharp.Indicators;

public partial class IndicatorContext<T>
{
    public Indicators.Ichimoku GetIchimoku(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
    {
        if (tenkanPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(tenkanPeriod));
        if (kijunPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(kijunPeriod));
        if (senkouSpanBPeriod <= 0) throw new ArgumentOutOfRangeException(nameof(senkouSpanBPeriod));

        for (var i = 0; i < _ichimokus.Count; i++)
        {
            var existing = _ichimokus[i];
            if (existing.TenkanPeriod == tenkanPeriod && existing.KijunPeriod == kijunPeriod && existing.SenkouSpanBPeriod == senkouSpanBPeriod)
                return existing.Ichimoku;
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

            var forwardIndex = i + kijunPeriod;
            if (forwardIndex < _data.Count)
            {
                senkouSpanA[forwardIndex] = senkouAValue;
                senkouSpanB[forwardIndex] = senkouBValue;
            }

            var chikouIndex = i - kijunPeriod;
            if (chikouIndex >= 0)
                chikouSpan[chikouIndex] = Close(i);
        }

        var ichimoku = new Indicators.Ichimoku(tenkanSen, kijunSen, senkouSpanA, senkouSpanB, chikouSpan);
        _ichimokus.Add(new Indicators.IchimokuIndicator(tenkanPeriod, kijunPeriod, senkouSpanBPeriod, ichimoku));
        return ichimoku;
    }
}
