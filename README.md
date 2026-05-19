# Santel.TradeSharp.Indicators

A high-performance .NET library for calculating technical analysis indicators on financial candle data.

## Features

- **Memory Efficient**: Caches calculated indicators to avoid redundant computation
- **Type Safe**: Strongly typed indicator models with clear parameter definitions
- **Time Aligned**: All indicator values are aligned by index with source candle data
- **Generic Support**: Works with any custom OHLCV model via `IndicatorContext<T>`
- **Easy to Use**: Simple, intuitive API with method chaining support
- **Point-in-Time Queries**: Single-value offset overloads skip full-array allocation when possible
- **External Data Overloads**: Compute any indicator over an arbitrary data slice without a separate context

## Supported Indicators

### Trend Indicators
- **EMA (Exponential Moving Average)**: Smooths price data with exponential weighting
- **SMA (Simple Moving Average)**: Simple arithmetic mean of closing prices
- **MACD (Moving Average Convergence Divergence)**: Shows relationship between two EMAs
- **ADX (Average Directional Index)**: Measures trend strength with directional indicators
- **Parabolic SAR**: Identifies potential reversal points
- **Ichimoku Cloud**: Comprehensive trend-following system with support/resistance zones

### Momentum Indicators
- **RSI (Relative Strength Index)**: Measures speed and magnitude of price changes (0–100)
- **Stochastic Oscillator**: Compares closing price to price range over time
- **CCI (Commodity Channel Index)**: Measures deviation from statistical mean
- **Williams %R**: Momentum indicator showing overbought/oversold levels
- **MFI (Money Flow Index)**: Volume-weighted RSI

### Volatility Indicators
- **ATR (Average True Range)**: Measures market volatility
- **Bollinger Bands**: Volatility bands around a moving average

### Volume Indicators
- **OBV (On-Balance Volume)**: Cumulative volume indicator based on price direction

## Installation

```bash
# Install from NuGet
dotnet add package Santel.TradeSharp.Indicators --version 2.0.0

# Clone or download the repository
git clone https://github.com/yourusername/Santel.TradeSharp.Indicators.git
cd Santel.TradeSharp.Indicators

# Build the project
dotnet build

# Reference in your project
dotnet add reference path/to/Core.csproj
```

## Quick Start

### Using the built-in `Candle` type

```csharp
using Santel.TradeSharp.Indicators;

// Create candle data
var candles = new List<Candle>
{
    new(DateTime.UtcNow.AddDays(-4), 100, 105, 95, 102, 1000),
    new(DateTime.UtcNow.AddDays(-3), 102, 106, 101, 104, 1200),
    new(DateTime.UtcNow.AddDays(-2), 104, 108, 103, 107, 1100),
    new(DateTime.UtcNow.AddDays(-1), 107, 109, 105, 106, 1300),
    new(DateTime.UtcNow, 106, 110, 104, 109, 1500)
};

// Initialize context with the built-in Candle type
var context = new IndicatorContext(candles);

// Calculate indicators
var ema12 = context.GetEma(12);
var rsi14 = context.GetRsi(14);
var macd = context.GetMacd(12, 26, 9);
var bb = context.GetBollingerBands(20, 2.0);

// Access values (aligned by index with source data)
var currentEma = ema12[^1];
var currentRsi = rsi14[^1];
var currentMacd = macd.Macd[^1];
var upperBand = bb.Upper[^1];

// Point-in-time queries — O(1) if full array is already cached
double emaYesterday = context.GetEma(12, offset: 1);
double rsiNow       = context.GetRsi(14, offset: 0);

// External-data overload — compute over an arbitrary slice
IReadOnlyList<Candle> slice = candles.TakeLast(50).ToList();
double sliceEma = context.GetEma(slice, period: 12, offset: 0);
```

### Using a custom OHLCV model

```csharp
using Santel.TradeSharp.Indicators;

// Use any custom model by providing selector functions
var context = new IndicatorContext<MyBar>(
    bars,
    b => b.Timestamp,
    b => b.Open,
    b => b.High,
    b => b.Low,
    b => b.Close,
    b => b.Volume   // optional; pass null to default to 0
);
```

## API Reference

### `Candle`

Built-in OHLCV candle record in the `Santel.TradeSharp.Indicators` namespace.

```csharp
var candle = new Candle(DateTime time, double open, double high, double low, double close, double volume = 0);
```

### `IndicatorContext` / `IndicatorContext<T>`

The main class for calculating indicators. All calculations are cached automatically.

- **`IndicatorContext(IReadOnlyList<Candle> data)`** — convenience constructor for the built-in `Candle` type.
- **`IndicatorContext<T>(...)`** — generic constructor for custom OHLCV models (see Quick Start above).

#### Accessor Methods

```csharp
DateTime Time(int i)
double   Open(int i)
double   High(int i)
double   Low(int i)
double   Close(int i)
double   Volume(int i)
```

#### Cache Properties

```csharp
IReadOnlyList<EmaIndicator>            Emas
IReadOnlyList<SmaIndicator>            Smas
IReadOnlyList<RsiIndicator>            Rsis
IReadOnlyList<AtrIndicator>            Atrs
IReadOnlyList<AdxIndicator>            Adxs
IReadOnlyList<CciIndicator>            Ccis
IReadOnlyList<WilliamsRIndicator>      WilliamsRs
IReadOnlyList<ObvIndicator>            Obvs
IReadOnlyList<MfiIndicator>            Mfis
IReadOnlyList<ParabolicSarIndicator>   ParabolicSars
IReadOnlyList<IchimokuIndicator>       Ichimokus
IReadOnlyList<BollingerBandsIndicator> BollingerBands
IReadOnlyList<StochasticIndicator>     Stochastics
IReadOnlyList<MacdIndicator>           Macds
```

---

### Methods

Every indicator exposes **three overloads**:

| Overload | Returns | Description |
|---|---|---|
| `GetXxx(params)` | full array / series | Calculates and **caches** the complete result |
| `GetXxx(params, int offset)` | single value / tuple | Point-in-time query — O(1) cache hit, otherwise computes only up to the target bar |
| `GetXxx(IReadOnlyList<T> data, params, int offset)` | single value / tuple | Same point-in-time query over an **external data list** (no caching) |

`offset` counts back from the latest bar: `0` = current bar, `1` = one bar ago, etc.

---

#### Trend Indicators

```csharp
// EMA
double[]  GetEma(int period)
double    GetEma(int period, int offset)
double    GetEma(IReadOnlyList<T> data, int period, int offset)

// SMA
double[]  GetSma(int period)
double    GetSma(int period, int offset)
double    GetSma(IReadOnlyList<T> data, int period, int offset)

// MACD — fastPeriod must be less than slowPeriod
MacdSeries                          GetMacd(int fastPeriod, int slowPeriod, int signalPeriod)
(double Macd, double Signal,
 double Histogram)                  GetMacd(int fastPeriod, int slowPeriod, int signalPeriod, int offset)
(double Macd, double Signal,
 double Histogram)                  GetMacd(IReadOnlyList<T> data, int fastPeriod, int slowPeriod, int signalPeriod, int offset)

// ADX
Adx                                 GetAdx(int period)
(double Adx, double PlusDi,
 double MinusDi)                    GetAdx(int period, int offset)
(double Adx, double PlusDi,
 double MinusDi)                    GetAdx(IReadOnlyList<T> data, int period, int offset)

// Parabolic SAR
double[]  GetParabolicSar(double accelerationFactor = 0.02, double maxAcceleration = 0.2)
double    GetParabolicSar(int offset, double accelerationFactor = 0.02, double maxAcceleration = 0.2)
double    GetParabolicSar(IReadOnlyList<T> data, int offset, double accelerationFactor = 0.02, double maxAcceleration = 0.2)

// Ichimoku Cloud
Ichimoku                                        GetIchimoku(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
(double TenkanSen, double KijunSen,
 double SenkouSpanA, double SenkouSpanB,
 double ChikouSpan)                             GetIchimoku(int offset, int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
(double TenkanSen, double KijunSen,
 double SenkouSpanA, double SenkouSpanB,
 double ChikouSpan)                             GetIchimoku(IReadOnlyList<T> data, int offset, int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
```

#### Momentum Indicators

```csharp
// RSI
double[]  GetRsi(int period)
double    GetRsi(int period, int offset)
double    GetRsi(IReadOnlyList<T> data, int period, int offset)

// Stochastic
Stochastic                    GetStochastic(int kPeriod, int dPeriod)
(double K, double D)          GetStochastic(int kPeriod, int dPeriod, int offset)
(double K, double D)          GetStochastic(IReadOnlyList<T> data, int kPeriod, int dPeriod, int offset)

// CCI
double[]  GetCci(int period)
double    GetCci(int period, int offset)
double    GetCci(IReadOnlyList<T> data, int period, int offset)

// Williams %R
double[]  GetWilliamsR(int period)
double    GetWilliamsR(int period, int offset)
double    GetWilliamsR(IReadOnlyList<T> data, int period, int offset)

// MFI
double[]  GetMfi(int period)
double    GetMfi(int period, int offset)
double    GetMfi(IReadOnlyList<T> data, int period, int offset)
```

#### Volatility Indicators

```csharp
// ATR
double[]  GetAtr(int period)
double    GetAtr(int period, int offset)
double    GetAtr(IReadOnlyList<T> data, int period, int offset)

// Bollinger Bands
BollingerBands                            GetBollingerBands(int period, double standardDeviation = 2.0)
(double Upper, double Middle,
 double Lower)                            GetBollingerBands(int period, int offset, double standardDeviation = 2.0)
(double Upper, double Middle,
 double Lower)                            GetBollingerBands(IReadOnlyList<T> data, int period, int offset, double standardDeviation = 2.0)
```

#### Volume Indicators

```csharp
// OBV
double[]  GetObv()
double    GetObv(int offset)
double    GetObv(IReadOnlyList<T> data, int offset)
```

---

## Data Alignment

### Index Alignment
All indicators maintain direct index alignment with the source data:
```csharp
indicator[i]  // corresponds to data[i] at the same timestamp
```

### Ichimoku Cloud Exception
Ichimoku components have intentional offsets:
- **TenkanSen, KijunSen**: Aligned with `data[i]`
- **SenkouSpanA, SenkouSpanB**: Shifted **forward** by `kijunPeriod` (default: 26)
- **ChikouSpan**: Shifted **backward** by `kijunPeriod` (default: 26)

This is by design — the cloud projects future support/resistance zones.

---

## Caching Strategy

The `IndicatorContext` automatically caches all full-array calculations.

```csharp
var ema1 = context.GetEma(12);  // Calculates and caches
var ema2 = context.GetEma(12);  // Returns cached — zero recomputation
```

### Point-in-Time Query Behaviour

When calling an offset overload (e.g. `GetEma(12, offset: 3)`):

1. **Fast path** — if the full array for that period is already cached, the result is a plain array index lookup: **O(1)**.
2. **Slow path** — if not cached, computation stops at the target bar. Window-based indicators (SMA, CCI, Williams %R, MFI) only iterate their rolling window, so the slow path is **O(period)**. State-dependent indicators (EMA, RSI, ATR, OBV, Parabolic SAR) iterate from bar 0 to the target bar, so it is **O(targetIndex)** — still never more than the full O(n) path.

```csharp
// Slow path — iterates bars 0..targetIndex only
double rsi = context.GetRsi(14, offset: 5);

// Fast path — O(1) index lookup, full array was computed above
var _ = context.GetRsi(14);          // caches full array
double rsiAgain = context.GetRsi(14, offset: 5);
```

### Dependency Reuse
Indicators that depend on others automatically reuse cached calculations:

```csharp
var macd  = context.GetMacd(12, 26, 9);  // Calculates and caches EMA(12) and EMA(26)
var ema12 = context.GetEma(12);          // Returns already-cached EMA(12) — O(1)
```

---

## Examples

### Basic Usage

```csharp
var context = new IndicatorContext(candles);

// Simple moving average
var sma20 = context.GetSma(20);
Console.WriteLine($"Current SMA(20): {sma20[^1]:F2}");

// Relative Strength Index
var rsi14 = context.GetRsi(14);
if (rsi14[^1] > 70)      Console.WriteLine("Overbought");
else if (rsi14[^1] < 30) Console.WriteLine("Oversold");
```

### Point-in-Time Queries

```csharp
// Ask for a single value without building the full array
double currentRsi  = context.GetRsi(14, offset: 0);
double previousRsi = context.GetRsi(14, offset: 1);

// Multi-value indicators return named tuples
var (adx, plusDi, minusDi) = context.GetAdx(14, offset: 0);
var (upper, mid, lower)    = context.GetBollingerBands(20, offset: 0);
var (macd, signal, hist)   = context.GetMacd(12, 26, 9, offset: 0);
var (k, d)                 = context.GetStochastic(14, 3, offset: 0);
var (tenkan, kijun, spanA, spanB, chikou) = context.GetIchimoku(offset: 0);
```

### External Data Overload

```csharp
// Compute an indicator over a custom slice without a separate context
IReadOnlyList<Candle> recentBars = candles.TakeLast(100).ToList();

double ema    = context.GetEma(recentBars, period: 20, offset: 0);
double rsi    = context.GetRsi(recentBars, period: 14, offset: 0);
double atr    = context.GetAtr(recentBars, period: 14, offset: 0);
double obv    = context.GetObv(recentBars, offset: 0);
double psar   = context.GetParabolicSar(recentBars, offset: 0);
```

### MACD Crossover Strategy

```csharp
var macd = context.GetMacd(12, 26, 9);

bool bullishCross = macd.Macd[^1] > macd.Signal[^1] && macd.Macd[^2] <= macd.Signal[^2];
bool bearishCross = macd.Macd[^1] < macd.Signal[^1] && macd.Macd[^2] >= macd.Signal[^2];

if (bullishCross) Console.WriteLine("Bullish MACD crossover");
if (bearishCross) Console.WriteLine("Bearish MACD crossover");
```

### Bollinger Bands Breakout

```csharp
var bb           = context.GetBollingerBands(20, 2.0);
var currentPrice = candles[^1].Close;

if (currentPrice > bb.Upper[^1])  Console.WriteLine("Above upper band — potential overbought");
if (currentPrice < bb.Lower[^1])  Console.WriteLine("Below lower band — potential oversold");
```

### Multiple Indicator Confluence

```csharp
var rsi  = context.GetRsi(14);
var macd = context.GetMacd(12, 26, 9);
var adx  = context.GetAdx(14);

bool strongTrend     = adx.Values[^1] > 25;
bool bullishMomentum = rsi[^1] > 50 && macd.Histogram[^1] > 0;

if (strongTrend && bullishMomentum)
    Console.WriteLine("Strong bullish signal");
```

### Accessing Cached Indicators

```csharp
context.GetEma(12);
context.GetEma(26);
context.GetRsi(14);
context.GetMacd(12, 26, 9);

Console.WriteLine($"EMAs cached:  {context.Emas.Count}");
Console.WriteLine($"RSIs cached:  {context.Rsis.Count}");
Console.WriteLine($"MACDs cached: {context.Macds.Count}");

foreach (var ema in context.Emas)
    Console.WriteLine($"EMA({ema.Period}) — {ema.Values.Length} values");
```

### Custom Model

```csharp
record MyBar(DateTime Ts, double O, double H, double L, double C, double V);

var bars    = FetchBars(); // IReadOnlyList<MyBar>
var context = new IndicatorContext<MyBar>(bars,
    b => b.Ts, b => b.O, b => b.H, b => b.L, b => b.C, b => b.V);

var rsi = context.GetRsi(14);
```

---

## Performance Considerations

### Memory Usage
- Each cached indicator stores a `double[]` of length equal to the data count
- Cached results persist for the lifetime of the `IndicatorContext`
- For very long series (10 000+ bars), consider multiple contexts for different time ranges

### Calculation Efficiency

| Scenario | Cost |
|---|---|
| Full-array call (first time) | O(n) |
| Full-array call (cached) | O(1) |
| Offset call — cache hit | O(1) |
| Offset call — window indicator (SMA, CCI, Williams %R, MFI) | O(period) |
| Offset call — state indicator (EMA, RSI, ATR, OBV, Parabolic SAR) | O(targetIndex) ≤ O(n) |
| External-data overload | O(targetIndex) |

---

## Project Structure

```
Santel.TradeSharp.Indicators/
├── Core/                                # Class library (Core.csproj)
│   ├── Models/
│   │   └── Candle.cs
│   ├── Indicators/
│   │   ├── EmaIndicator.cs
│   │   ├── SmaIndicator.cs
│   │   ├── RsiIndicator.cs
│   │   ├── AtrIndicator.cs
│   │   ├── AdxIndicator.cs  /  Adx.cs
│   │   ├── CciIndicator.cs
│   │   ├── WilliamsRIndicator.cs
│   │   ├── ObvIndicator.cs
│   │   ├── MfiIndicator.cs
│   │   ├── ParabolicSarIndicator.cs
│   │   ├── IchimokuIndicator.cs  /  Ichimoku.cs
│   │   ├── BollingerBandsIndicator.cs  /  BollingerBands.cs
│   │   ├── StochasticIndicator.cs  /  Stochastic.cs
│   │   └── MacdIndicator.cs  /  MacdSeries.cs
│   ├── IndicatorContext.cs              # Core context + Candle convenience class
│   ├── IndicatorContext.Ema.cs
│   ├── IndicatorContext.Sma.cs
│   ├── IndicatorContext.Rsi.cs
│   ├── IndicatorContext.Atr.cs
│   ├── IndicatorContext.Adx.cs
│   ├── IndicatorContext.Cci.cs
│   ├── IndicatorContext.WilliamsR.cs
│   ├── IndicatorContext.Obv.cs
│   ├── IndicatorContext.Mfi.cs
│   ├── IndicatorContext.ParabolicSar.cs
│   ├── IndicatorContext.Ichimoku.cs
│   ├── IndicatorContext.BollingerBands.cs
│   ├── IndicatorContext.Stochastic.cs
│   └── IndicatorContext.Macd.cs
└── ConsoleApp/                          # Demo console application
    └── Program.cs
```

---

## Requirements

- .NET 10.0 or higher
- No external dependencies

## License

MIT License — free to use commercially, modify, distribute, and use privately.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues, questions, or feature requests, please open an issue on GitHub.
