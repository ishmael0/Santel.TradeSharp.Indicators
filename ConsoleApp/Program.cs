using Santel.TradeSharp.Indicators;
using Santel.TradeSharp.Indicators.Models;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║    Santel.TradeSharp.Indicators - Technical Analysis Demo     ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Create sample candle data (simulating a realistic price series)
var candles = GenerateSampleCandles(100);

Console.WriteLine($"Loaded {candles.Count} candles");
Console.WriteLine($"Date Range: {candles[0].Time:yyyy-MM-dd} to {candles[^1].Time:yyyy-MM-dd}");
Console.WriteLine($"Price Range: {candles.Min(c => c.Low):F2} - {candles.Max(c => c.High):F2}");
Console.WriteLine($"Current Price: {candles[^1].Close:F2}");
Console.WriteLine();

// Initialize indicator context
var context = new IndicatorContext(candles);

// ═══════════════════════════════════════════════════════════════
// TREND INDICATORS
// ═══════════════════════════════════════════════════════════════
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("TREND INDICATORS");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// EMA (Exponential Moving Average)
var ema12 = context.GetEma(12);
var ema26 = context.GetEma(26);
Console.WriteLine($"EMA(12):  {ema12[^1]:F2}");
Console.WriteLine($"EMA(26):  {ema26[^1]:F2}");
Console.WriteLine($"Trend:    {(ema12[^1] > ema26[^1] ? "Bullish ↑" : "Bearish ↓")}");
Console.WriteLine();

// SMA (Simple Moving Average)
var sma20 = context.GetSma(20);
var sma50 = context.GetSma(50);
Console.WriteLine($"SMA(20):  {sma20[^1]:F2}");
Console.WriteLine($"SMA(50):  {sma50[^1]:F2}");
Console.WriteLine($"Trend:    {(sma20[^1] > sma50[^1] ? "Bullish ↑" : "Bearish ↓")}");
Console.WriteLine();

// MACD (Moving Average Convergence Divergence)
var macd = context.GetMacd(12, 26, 9);
Console.WriteLine($"MACD:      {macd.Macd[^1]:F4}");
Console.WriteLine($"Signal:    {macd.Signal[^1]:F4}");
Console.WriteLine($"Histogram: {macd.Histogram[^1]:F4}");
Console.WriteLine($"Status:    {(macd.Histogram[^1] > 0 ? "Bullish ↑" : "Bearish ↓")}");
Console.WriteLine();

// ADX (Average Directional Index)
var adx = context.GetAdx(14);
Console.WriteLine($"ADX:       {adx.Values[^1]:F2}");
Console.WriteLine($"+DI:       {adx.PlusDi[^1]:F2}");
Console.WriteLine($"-DI:       {adx.MinusDi[^1]:F2}");
var trendStrength = adx.Values[^1] > 25 ? "Strong" : adx.Values[^1] > 20 ? "Moderate" : "Weak";
Console.WriteLine($"Strength:  {trendStrength}");
Console.WriteLine();

// Parabolic SAR
var sar = context.GetParabolicSar();
var currentPrice = candles[^1].Close;
Console.WriteLine($"SAR:       {sar[^1]:F2}");
Console.WriteLine($"Position:  {(currentPrice > sar[^1] ? "Long ↑" : "Short ↓")}");
Console.WriteLine();

// Ichimoku Cloud
var ichimoku = context.GetIchimoku();
Console.WriteLine($"Tenkan:    {ichimoku.TenkanSen[^1]:F2}");
Console.WriteLine($"Kijun:     {ichimoku.KijunSen[^1]:F2}");
Console.WriteLine($"Span A:    {ichimoku.SenkouSpanA[^1]:F2}");
Console.WriteLine($"Span B:    {ichimoku.SenkouSpanB[^1]:F2}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════════════
// MOMENTUM INDICATORS
// ═══════════════════════════════════════════════════════════════
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("MOMENTUM INDICATORS");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// RSI (Relative Strength Index)
var rsi = context.GetRsi(14);
Console.WriteLine($"RSI(14):   {rsi[^1]:F2}");
var rsiStatus = rsi[^1] > 70 ? "Overbought" : rsi[^1] < 30 ? "Oversold" : "Neutral";
Console.WriteLine($"Status:    {rsiStatus}");
Console.WriteLine();

// Stochastic Oscillator
var stochastic = context.GetStochastic(14, 3);
Console.WriteLine($"%K:        {stochastic.K[^1]:F2}");
Console.WriteLine($"%D:        {stochastic.D[^1]:F2}");
var stochStatus = stochastic.K[^1] > 80 ? "Overbought" : stochastic.K[^1] < 20 ? "Oversold" : "Neutral";
Console.WriteLine($"Status:    {stochStatus}");
Console.WriteLine();

// CCI (Commodity Channel Index)
var cci = context.GetCci(20);
Console.WriteLine($"CCI(20):   {cci[^1]:F2}");
var cciStatus = cci[^1] > 100 ? "Overbought" : cci[^1] < -100 ? "Oversold" : "Neutral";
Console.WriteLine($"Status:    {cciStatus}");
Console.WriteLine();

// Williams %R
var williamsR = context.GetWilliamsR(14);
Console.WriteLine($"Williams:  {williamsR[^1]:F2}");
var willStatus = williamsR[^1] > -20 ? "Overbought" : williamsR[^1] < -80 ? "Oversold" : "Neutral";
Console.WriteLine($"Status:    {willStatus}");
Console.WriteLine();

// MFI (Money Flow Index)
var mfi = context.GetMfi(14);
Console.WriteLine($"MFI(14):   {mfi[^1]:F2}");
var mfiStatus = mfi[^1] > 80 ? "Overbought" : mfi[^1] < 20 ? "Oversold" : "Neutral";
Console.WriteLine($"Status:    {mfiStatus}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════════════
// VOLATILITY INDICATORS
// ═══════════════════════════════════════════════════════════════
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("VOLATILITY INDICATORS");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// ATR (Average True Range)
var atr = context.GetAtr(14);
Console.WriteLine($"ATR(14):   {atr[^1]:F2}");
var volatility = atr[^1] / currentPrice * 100;
Console.WriteLine($"Vol %:     {volatility:F2}%");
Console.WriteLine();

// Bollinger Bands
var bb = context.GetBollingerBands(20, 2.0);
Console.WriteLine($"Upper:     {bb.Upper[^1]:F2}");
Console.WriteLine($"Middle:    {bb.Middle[^1]:F2}");
Console.WriteLine($"Lower:     {bb.Lower[^1]:F2}");
Console.WriteLine($"Price:     {currentPrice:F2}");
var bbWidth = (bb.Upper[^1] - bb.Lower[^1]) / bb.Middle[^1] * 100;
Console.WriteLine($"Width %:   {bbWidth:F2}%");

if (currentPrice > bb.Upper[^1])
    Console.WriteLine($"Position:  Above upper band (potential overbought)");
else if (currentPrice < bb.Lower[^1])
    Console.WriteLine($"Position:  Below lower band (potential oversold)");
else
    Console.WriteLine($"Position:  Within bands");
Console.WriteLine();

// ═══════════════════════════════════════════════════════════════
// VOLUME INDICATORS
// ═══════════════════════════════════════════════════════════════
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("VOLUME INDICATORS");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// OBV (On-Balance Volume)
var obv = context.GetObv();
Console.WriteLine($"OBV:       {obv[^1]:F0}");
var obvTrend = obv.Length > 1 && obv[^1] > obv[^2] ? "Rising ↑" : "Falling ↓";
Console.WriteLine($"Trend:     {obvTrend}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════════════
// TRADING SIGNALS
// ═══════════════════════════════════════════════════════════════
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("TRADING SIGNALS");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// MACD Crossover
if (candles.Count >= 2)
{
    var macdCross = DetectMacdCrossover(macd);
    if (macdCross != null)
        Console.WriteLine($"✓ MACD: {macdCross}");
}

// RSI Oversold/Overbought
if (rsi[^1] > 70)
    Console.WriteLine($"⚠ RSI: Overbought condition ({rsi[^1]:F2})");
else if (rsi[^1] < 30)
    Console.WriteLine($"✓ RSI: Oversold condition ({rsi[^1]:F2})");

// Bollinger Bands Squeeze
if (bbWidth < 5)
    Console.WriteLine($"⚠ BB: Squeeze detected - volatility breakout expected");

// Strong Trend Confirmation
if (adx.Values[^1] > 25 && macd.Histogram[^1] > 0 && rsi[^1] > 50)
    Console.WriteLine($"✓ Strong Bullish Trend Confirmed");
else if (adx.Values[^1] > 25 && macd.Histogram[^1] < 0 && rsi[^1] < 50)
    Console.WriteLine($"✓ Strong Bearish Trend Confirmed");

// Price vs Moving Averages
if (currentPrice > sma20[^1] && currentPrice > sma50[^1])
    Console.WriteLine($"✓ Price above both SMA(20) and SMA(50) - Bullish");
else if (currentPrice < sma20[^1] && currentPrice < sma50[^1])
    Console.WriteLine($"⚠ Price below both SMA(20) and SMA(50) - Bearish");

Console.WriteLine();

// ═══════════════════════════════════════════════════════════════
// CACHE STATISTICS
// ═══════════════════════════════════════════════════════════════
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("CACHE STATISTICS");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

Console.WriteLine($"EMAs cached:            {context.Emas.Count}");
Console.WriteLine($"SMAs cached:            {context.Smas.Count}");
Console.WriteLine($"RSIs cached:            {context.Rsis.Count}");
Console.WriteLine($"MACDs cached:           {context.Macds.Count}");
Console.WriteLine($"Bollinger Bands cached: {context.BollingerBands.Count}");
Console.WriteLine($"ATRs cached:            {context.Atrs.Count}");
Console.WriteLine($"ADXs cached:            {context.Adxs.Count}");
Console.WriteLine($"Stochastics cached:     {context.Stochastics.Count}");
Console.WriteLine($"CCIs cached:            {context.Ccis.Count}");
Console.WriteLine($"Williams %Rs cached:    {context.WilliamsRs.Count}");
Console.WriteLine($"MFIs cached:            {context.Mfis.Count}");
Console.WriteLine($"OBVs cached:            {context.Obvs.Count}");
Console.WriteLine($"Parabolic SARs cached:  {context.ParabolicSars.Count}");
Console.WriteLine($"Ichimokus cached:       {context.Ichimokus.Count}");
Console.WriteLine();

Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("Demo completed successfully!");
Console.WriteLine("═══════════════════════════════════════════════════════════════");

// ═══════════════════════════════════════════════════════════════
// HELPER FUNCTIONS
// ═══════════════════════════════════════════════════════════════

static List<Candle> GenerateSampleCandles(int count)
{
    var candles = new List<Candle>();
    var random = new Random(42); // Fixed seed for reproducibility
    var baseTime = DateTime.UtcNow.AddDays(-count);
    var price = 100.0;

    for (int i = 0; i < count; i++)
    {
        // Simulate realistic price movement with trend and volatility
        var trend = Math.Sin(i / 10.0) * 0.5; // Cyclical trend
        var volatility = random.NextDouble() - 0.5;
        var change = trend + volatility;
        
        price += change;
        price = Math.Max(90, Math.Min(110, price)); // Keep price in range
        
        var open = price + (random.NextDouble() - 0.5) * 0.5;
        var close = price + (random.NextDouble() - 0.5) * 0.5;
        var high = Math.Max(open, close) + random.NextDouble() * 0.8;
        var low = Math.Min(open, close) - random.NextDouble() * 0.8;
        var volume = 1000 + random.NextDouble() * 500;

        candles.Add(new Candle(
            baseTime.AddDays(i),
            open,
            high,
            low,
            close,
            volume
        ));
    }

    return candles;
}

static string? DetectMacdCrossover(Santel.TradeSharp.Indicators.Indicators.MacdSeries macd)
{
    if (macd.Macd.Length < 2) return null;

    var current = macd.Macd[^1];
    var previous = macd.Macd[^2];
    var currentSignal = macd.Signal[^1];
    var previousSignal = macd.Signal[^2];

    if (current > currentSignal && previous <= previousSignal)
        return "Bullish crossover detected ↑";
    else if (current < currentSignal && previous >= previousSignal)
        return "Bearish crossover detected ↓";

    return null;
}

