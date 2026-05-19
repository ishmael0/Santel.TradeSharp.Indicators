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

public partial class IndicatorContext : IndicatorContext<Candle>
{
    public IndicatorContext(IReadOnlyList<Candle> data)
        : base(data, c => c.Time, c => c.Open, c => c.High, c => c.Low, c => c.Close, c => c.Volume)
    {
    }
}

public partial class IndicatorContext<T>
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
}
