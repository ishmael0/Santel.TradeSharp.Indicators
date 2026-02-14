# Santel.TradeSharp.Indicators

A high-performance .NET library for calculating technical analysis indicators on financial candle data.

## Features

- **Memory Efficient**: Caches calculated indicators to avoid redundant computation
- **Type Safe**: Strongly typed indicator models with clear parameter definitions
- **Time Aligned**: All indicator values are aligned by index with source candle data
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
dotnet add package Santel.TradeSharp.Indicators --version 1.0.1

# Clone or download the repository
git clone https://github.com/yourusername/Santel.TradeSharp.Indicators.git
cd Santel.TradeSharp.Indicators

# Build the project
dotnet build

# Reference in your project
dotnet add reference path/to/Santel.TradeSharp.Indicators.csproj
```

## Quick Start

```csharp
using Santel.TradeSharp.Indicators;
using Santel.TradeSharp.Indicators.Models;

// Create candle data
var candles = new List<Candle>
{
    new(DateTime.UtcNow.AddDays(-4), 100, 105, 95, 102, 1000),
    new(DateTime.UtcNow.AddDays(-3), 102, 106, 101, 104, 1200),
    new(DateTime.UtcNow.AddDays(-2), 104, 108, 103, 107, 1100),
    new(DateTime.UtcNow.AddDays(-1), 107, 109, 105, 106, 1300),
    new(DateTime.UtcNow, 106, 110, 104, 109, 1500)
};

// Initialize context
var context = new IndicatorContext(candles);

// Calculate indicators
var ema12 = context.GetEma(12);
var rsi14 = context.GetRsi(14);
var macd = context.GetMacd(12, 26, 9);
var bb = context.GetBollingerBands(20, 2.0);

// Access values (aligned by index)
var currentEma = ema12[^1];
var currentRsi = rsi14[^1];
var currentMacd = macd.Macd[^1];
var upperBand = bb.Upper[^1];
```

## API Reference

### IndicatorContext

The main class for calculating indicators. All calculations are cached automatically.

#### Properties

```csharp
IReadOnlyList<Candle> Candles           // Source candle data
IReadOnlyList<EmaIndicator> Emas        // Cached EMA calculations
IReadOnlyList<SmaIndicator> Smas        // Cached SMA calculations
IReadOnlyList<RsiIndicator> Rsis        // Cached RSI calculations
IReadOnlyList<AtrIndicator> Atrs        // Cached ATR calculations
IReadOnlyList<AdxIndicator> Adxs        // Cached ADX calculations
IReadOnlyList<CciIndicator> Ccis        // Cached CCI calculations
IReadOnlyList<WilliamsRIndicator> WilliamsRs    // Cached Williams %R calculations
IReadOnlyList<ObvIndicator> Obvs        // Cached OBV calculations
IReadOnlyList<MfiIndicator> Mfis        // Cached MFI calculations
IReadOnlyList<ParabolicSarIndicator> ParabolicSars  // Cached Parabolic SAR calculations
IReadOnlyList<IchimokuIndicator> Ichimokus          // Cached Ichimoku calculations
IReadOnlyList<BollingerBandsIndicator> BollingerBands   // Cached Bollinger Bands calculations
IReadOnlyList<StochasticIndicator> Stochastics         // Cached Stochastic calculations
IReadOnlyList<MacdIndicator> Macds      // Cached MACD calculations
```

#### Methods

##### Trend Indicators

```csharp
double[] GetEma(int period)
// Returns: EMA values for each candle

double[] GetSma(int period)
// Returns: SMA values for each candle

MacdSeries GetMacd(int fastPeriod, int slowPeriod, int signalPeriod)
// Returns: MacdSeries with Macd, Signal, and Histogram arrays

Adx GetAdx(int period)
// Returns: Adx with Values (ADX), PlusDi (+DI), and MinusDi (-DI) arrays

double[] GetParabolicSar(double accelerationFactor = 0.02, double maxAcceleration = 0.2)
// Returns: Parabolic SAR values for each candle

Ichimoku GetIchimoku(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouSpanBPeriod = 52)
// Returns: Ichimoku with TenkanSen, KijunSen, SenkouSpanA, SenkouSpanB, ChikouSpan arrays
// Note: Cloud spans are shifted forward, Chikou is shifted backward
```

##### Momentum Indicators

```csharp
double[] GetRsi(int period)
// Returns: RSI values (0-100) for each candle

Stochastic GetStochastic(int kPeriod, int dPeriod)
// Returns: Stochastic with K and D arrays

double[] GetCci(int period)
// Returns: CCI values for each candle

double[] GetWilliamsR(int period)
// Returns: Williams %R values for each candle

double[] GetMfi(int period)
// Returns: MFI values (0-100) for each candle
```

##### Volatility Indicators

```csharp
double[] GetAtr(int period)
// Returns: ATR values for each candle

BollingerBands GetBollingerBands(int period, double standardDeviation = 2.0)
// Returns: BollingerBands with Upper, Middle, and Lower arrays
```

##### Volume Indicators

```csharp
double[] GetObv()
// Returns: OBV values for each candle (cumulative volume)
```

## Data Alignment

### Index Alignment
Most indicators maintain direct index alignment with the source candle data:
```csharp
indicator[i] corresponds to candles[i] at the same timestamp
```

### Ichimoku Cloud Exception
Ichimoku components have intentional offsets:
- **TenkanSen, KijunSen**: Aligned with `candles[i]`
- **SenkouSpanA, SenkouSpanB**: Shifted **forward** by `kijunPeriod` (default: 26)
- **ChikouSpan**: Shifted **backward** by `kijunPeriod` (default: 26)

This is by design - the cloud projects future support/resistance zones.

## Caching Strategy

The `IndicatorContext` automatically caches all calculated indicators. When you call a Get method:

1. **Check cache**: If indicator with same parameters exists, return cached result
2. **Calculate**: If not found, calculate the indicator
3. **Store**: Add to cache for future use
4. **Return**: Return the calculated values

This means:
```csharp
var ema1 = context.GetEma(12);  // Calculates
var ema2 = context.GetEma(12);  // Returns cached - no recalculation
```

### Dependency Reuse
Indicators that depend on others automatically reuse cached calculations:
```csharp
var macd = context.GetMacd(12, 26, 9);  // Calculates EMA(12) and EMA(26)
var ema12 = context.GetEma(12);         // Returns cached EMA(12) - already calculated by MACD
```

## Project Structure

```
Santel.TradeSharp.Indicators/
├── Models/
│   └── Candle.cs                    # OHLCV candle data model
├── Indicators/
│   ├── EmaIndicator.cs
│   ├── SmaIndicator.cs
│   ├── RsiIndicator.cs
│   ├── AtrIndicator.cs
│   ├── AdxIndicator.cs
│   ├── Adx.cs
│   ├── CciIndicator.cs
│   ├── WilliamsRIndicator.cs
│   ├── ObvIndicator.cs
│   ├── MfiIndicator.cs
│   ├── ParabolicSarIndicator.cs
│   ├── IchimokuIndicator.cs
│   ├── Ichimoku.cs
│   ├── BollingerBandsIndicator.cs
│   ├── BollingerBands.cs
│   ├── StochasticIndicator.cs
│   ├── Stochastic.cs
│   ├── MacdIndicator.cs
│   └── MacdSeries.cs
├── IndicatorContext.cs              # Main calculation context
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

var currentMacd = macd.Macd[^1];
var currentSignal = macd.Signal[^1];
var previousMacd = macd.Macd[^2];
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
var upperBand = bb.Upper[^1];
var lowerBand = bb.Lower[^1];

if (currentPrice > upperBand)
    Console.WriteLine("Price above upper band - potential overbought");
else if (currentPrice < lowerBand)
    Console.WriteLine("Price below lower band - potential oversold");
```

### Multiple Indicator Confluence

```csharp
var rsi = context.GetRsi(14);
var macd = context.GetMacd(12, 26, 9);
var adx = context.GetAdx(14);

var strongTrend = adx.Values[^1] > 25;
var bullishMomentum = rsi[^1] > 50 && macd.Histogram[^1] > 0;

if (strongTrend && bullishMomentum)
    Console.WriteLine("Strong bullish signal");
```

### Accessing Cached Indicators

```csharp
// Calculate multiple indicators
context.GetEma(12);
context.GetEma(26);
context.GetRsi(14);
context.GetMacd(12, 26, 9);

// Inspect what's been calculated
Console.WriteLine($"Total EMAs calculated: {context.Emas.Count}");
Console.WriteLine($"Total RSIs calculated: {context.Rsis.Count}");
Console.WriteLine($"Total MACDs calculated: {context.Macds.Count}");

// Access cached indicator metadata
foreach (var ema in context.Emas)
{
    Console.WriteLine($"EMA({ema.Period}) cached with {ema.Values.Length} values");
}
```

## Performance Considerations

### Memory Usage
- Each indicator stores a `double[]` with length equal to candle count
- Cached indicators persist in memory for the lifetime of the `IndicatorContext`
- For very long series (10,000+ candles), consider:
  - Using multiple contexts for different time ranges
  - Clearing and recreating context when indicator parameters change frequently

### Calculation Efficiency
- First calculation: O(n) for most indicators where n = candle count
- Cached retrieval: O(1)
- Dependent indicators (e.g., MACD) automatically reuse existing calculations

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

This is a completely free and open-source package with no restrictions.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues, questions, or feature requests, please open an issue on GitHub.
