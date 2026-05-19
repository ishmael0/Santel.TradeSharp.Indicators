# Santel.TradeSharp.Indicators

A high-performance .NET library for calculating technical analysis indicators on financial candle data.

## Features

- **Memory Efficient**: Caches calculated indicators to avoid redundant computation
- **Type Safe**: Strongly typed indicator models with clear parameter definitions
- **Time Aligned**: All indicator values are aligned by index with source candle data
- **Generic Support**: Works with any custom OHLCV model via `IndicatorContext<T>`
- **Easy to Use**: Simple, intuitive API with method chaining support

## Supported Indicators

### Trend Indicators
- **EMA (Exponential Moving Average)**: Smooths price data with exponential weighting
- **SMA (Simple Moving Average)**: Simple arithmetic mean of closing prices
- **MACD (Moving Average Convergence Divergence)**: Shows relationship between two EMAs
- **ADX (Average Directional Index)**: Measures trend strength with directional indicators
- **Parabolic SAR**: Identifies potential reversal points
- **Ichimoku Cloud**: Comprehensive trend-following system with support/resistance zones

### Momentum Indicators
- **RSI (Relative Strength Index)**: Measures speed and magnitude of price changes (0-100)
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

#### Methods

##### Trend Indicators

```csharp
double[] GetEma(int period)
// Returns: EMA values for each data point

double[] GetSma(int period)
// Returns: SMA values for each data point

MacdSeries GetMacd(int fastPeriod, int slowPeriod, int signalPeriod)
// Returns: MacdSeries with Macd, Signal, and Histogram arrays
// Constraint: fastPeriod must be less than slowPeriod

Adx GetAdx(int period)
// Returns: Adx with Values (ADX), PlusDi (+DI), and MinusDi (-DI) arrays

double[] GetParabolicSar(double accelerationFactor = 0.02, double maxAcceleration = 0.2)
// Returns: Parabolic SAR values for each data point

Ichimoku GetIchimoku(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
// Returns: Ichimoku with TenkanSen, KijunSen, SenkouSpanA, SenkouSpanB, ChikouSpan arrays
// Note: Cloud spans are shifted forward, Chikou is shifted backward
```

##### Momentum Indicators

```csharp
double[] GetRsi(int period)
// Returns: RSI values (0-100) for each data point

Stochastic GetStochastic(int kPeriod, int dPeriod)
// Returns: Stochastic with K and D arrays

double[] GetCci(int period)
// Returns: CCI values for each data point

double[] GetWilliamsR(int period)
// Returns: Williams %R values for each data point

double[] GetMfi(int period)
// Returns: MFI values (0-100) for each data point
```

##### Volatility Indicators

```csharp
double[] GetAtr(int period)
// Returns: ATR values for each data point

BollingerBands GetBollingerBands(int period, double standardDeviation = 2.0)
// Returns: BollingerBands with Upper, Middle, and Lower arrays
```

##### Volume Indicators

```csharp
double[] GetObv()
// Returns: OBV values for each data point (cumulative volume)
```

## Data Alignment

### Index Alignment
Most indicators maintain direct index alignment with the source data:
```csharp
indicator[i] corresponds to data[i] at the same timestamp
```

### Ichimoku Cloud Exception
Ichimoku components have intentional offsets:
- **TenkanSen, KijunSen**: Aligned with `data[i]`
- **SenkouSpanA, SenkouSpanB**: Shifted **forward** by `kijunPeriod` (default: 26)
- **ChikouSpan**: Shifted **backward** by `kijunPeriod` (default: 26)

This is by design — the cloud projects future support/resistance zones.

## Caching Strategy

The `IndicatorContext` automatically caches all calculated indicators. When you call a Get method:

1. **Check cache**: If indicator with same parameters exists, return cached result
2. **Calculate**: If not found, calculate the indicator
3. **Store**: Add to cache for future use
4. **Return**: Return the calculated values

```csharp
var ema1 = context.GetEma(12);  // Calculates
var ema2 = context.GetEma(12);  // Returns cached — no recalculation
```

### Dependency Reuse
Indicators that depend on others automatically reuse cached calculations:
```csharp
var macd = context.GetMacd(12, 26, 9);  // Calculates EMA(12) and EMA(26)
var ema12 = context.GetEma(12);         // Returns cached EMA(12) — already calculated by MACD
```

## Project Structure

```
Santel.TradeSharp.Indicators/
├── Core/                                # Class library (Core.csproj)
│   ├── Models/
│   │   └── Candle.cs                    # OHLCV candle data model
│   ├── Indicators/
│   │   ├── EmaIndicator.cs
│   │   ├── SmaIndicator.cs
│   │   ├── RsiIndicator.cs
│   │   ├── AtrIndicator.cs
│   │   ├── AdxIndicator.cs
│   │   ├── Adx.cs
│   │   ├── CciIndicator.cs
│   │   ├── WilliamsRIndicator.cs
│   │   ├── ObvIndicator.cs
│   │   ├── MfiIndicator.cs
│   │   ├── ParabolicSarIndicator.cs
│   │   ├── IchimokuIndicator.cs
│   │   ├── Ichimoku.cs
│   │   ├── BollingerBandsIndicator.cs
│   │   ├── BollingerBands.cs
│   │   ├── StochasticIndicator.cs
│   │   ├── Stochastic.cs
│   │   ├── MacdIndicator.cs
│   │   └── MacdSeries.cs
│   └── IndicatorContext.cs              # Main calculation context (generic + Candle convenience class)
└── ConsoleApp/                          # Demo console application (ConsoleApp.csproj)
    └── Program.cs                       # Example usage
```

## Examples

### Basic Usage

```csharp
var context = new IndicatorContext(candles);

// Simple moving average
var sma20 = context.GetSma(20);
Console.WriteLine($"Current SMA(20): {sma20[^1]:F2}");

// Relative Strength Index
var rsi14 = context.GetRsi(14);
if (rsi14[^1] > 70)
    Console.WriteLine("Overbought");
else if (rsi14[^1] < 30)
    Console.WriteLine("Oversold");
```

### MACD Crossover Strategy

```csharp
var macd = context.GetMacd(12, 26, 9);

var currentMacd   = macd.Macd[^1];
var currentSignal = macd.Signal[^1];
var previousMacd  = macd.Macd[^2];
var previousSignal = macd.Signal[^2];

if (currentMacd > currentSignal && previousMacd <= previousSignal)
    Console.WriteLine("Bullish MACD crossover");
else if (currentMacd < currentSignal && previousMacd >= previousSignal)
    Console.WriteLine("Bearish MACD crossover");
```

### Bollinger Bands Breakout

```csharp
var bb = context.GetBollingerBands(20, 2.0);

var currentPrice = candles[^1].Close;
var upperBand    = bb.Upper[^1];
var lowerBand    = bb.Lower[^1];

if (currentPrice > upperBand)
    Console.WriteLine("Price above upper band - potential overbought");
else if (currentPrice < lowerBand)
    Console.WriteLine("Price below lower band - potential oversold");
```

### Multiple Indicator Confluence

```csharp
var rsi  = context.GetRsi(14);
var macd = context.GetMacd(12, 26, 9);
var adx  = context.GetAdx(14);

var strongTrend      = adx.Values[^1] > 25;
var bullishMomentum  = rsi[^1] > 50 && macd.Histogram[^1] > 0;

if (strongTrend && bullishMomentum)
    Console.WriteLine("Strong bullish signal");
```

### Accessing Cached Indicators

```csharp
context.GetEma(12);
context.GetEma(26);
context.GetRsi(14);
context.GetMacd(12, 26, 9);

Console.WriteLine($"Total EMAs calculated:  {context.Emas.Count}");
Console.WriteLine($"Total RSIs calculated:  {context.Rsis.Count}");
Console.WriteLine($"Total MACDs calculated: {context.Macds.Count}");

foreach (var ema in context.Emas)
    Console.WriteLine($"EMA({ema.Period}) cached with {ema.Values.Length} values");
```

### Custom Model

```csharp
// Your own bar type
record MyBar(DateTime Ts, double O, double H, double L, double C, double V);

var bars = FetchBars(); // IReadOnlyList<MyBar>
var context = new IndicatorContext<MyBar>(bars,
    b => b.Ts, b => b.O, b => b.H, b => b.L, b => b.C, b => b.V);

var rsi = context.GetRsi(14);
```

## Performance Considerations

### Memory Usage
- Each indicator stores a `double[]` with length equal to the data count
- Cached indicators persist for the lifetime of the `IndicatorContext`
- For very long series (10,000+ data points), consider:
  - Using multiple contexts for different time ranges
  - Clearing and recreating context when indicator parameters change frequently

### Calculation Efficiency
- First calculation: O(n) for most indicators where n = data count
- Cached retrieval: O(1)
- Dependent indicators (e.g., MACD uses EMA internally) automatically reuse existing calculations

## Requirements

- .NET 10.0 or higher
- No external dependencies

## License

This project is licensed under the MIT License - a permissive free software license.

**You are free to:**
- ✅ Use commercially
- ✅ Modify
- ✅ Distribute
- ✅ Use privately

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues, questions, or feature requests, please open an issue on GitHub.
