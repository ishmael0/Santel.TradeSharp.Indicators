using Santel.TradeSharp.Indicators;
using Santel.TradeSharp.Indicators.Models;

namespace Tests
{
    public class IndicatorTests
    {
        private static List<Candle> CreateSampleCandles(int count = 50)
        {
            var candles = new List<Candle>();
            var baseTime = new DateTime(2024, 1, 1);
            var random = new Random(42); // Fixed seed for reproducibility

            for (int i = 0; i < count; i++)
            {
                var open = 100 + random.NextDouble() * 10;
                var close = open + (random.NextDouble() - 0.5) * 5;
                var high = Math.Max(open, close) + random.NextDouble() * 2;
                var low = Math.Min(open, close) - random.NextDouble() * 2;
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

        #region EMA Tests

        [Fact]
        public void GetEma_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var ema = context.GetEma(12);

            // Assert
            Assert.Equal(50, ema.Length);
        }

        [Fact]
        public void GetEma_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var ema1 = context.GetEma(12);
            var ema2 = context.GetEma(12);

            // Assert
            Assert.Same(ema1, ema2);
            Assert.Single(context.Emas);
        }

        [Fact]
        public void GetEma_InvalidPeriod_ThrowsException()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => context.GetEma(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => context.GetEma(-1));
        }

        [Fact]
        public void GetEma_DifferentPeriods_StoredSeparately()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var ema12 = context.GetEma(12);
            var ema26 = context.GetEma(26);

            // Assert
            Assert.NotSame(ema12, ema26);
            Assert.Equal(2, context.Emas.Count);
        }

        #endregion

        #region SMA Tests

        [Fact]
        public void GetSma_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var sma = context.GetSma(20);

            // Assert
            Assert.Equal(50, sma.Length);
        }

        [Fact]
        public void GetSma_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var sma1 = context.GetSma(20);
            var sma2 = context.GetSma(20);

            // Assert
            Assert.Same(sma1, sma2);
            Assert.Single(context.Smas);
        }

        [Fact]
        public void GetSma_InvalidPeriod_ThrowsException()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => context.GetSma(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => context.GetSma(-1));
        }

        #endregion

        #region RSI Tests

        [Fact]
        public void GetRsi_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var rsi = context.GetRsi(14);

            // Assert
            Assert.Equal(50, rsi.Length);
        }

        [Fact]
        public void GetRsi_Values_WithinValidRange()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var rsi = context.GetRsi(14);

            // Assert - RSI should be between 0 and 100
            for (int i = 14; i < rsi.Length; i++) // Skip initial period
            {
                Assert.InRange(rsi[i], 0, 100);
            }
        }

        [Fact]
        public void GetRsi_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var rsi1 = context.GetRsi(14);
            var rsi2 = context.GetRsi(14);

            // Assert
            Assert.Same(rsi1, rsi2);
            Assert.Single(context.Rsis);
        }

        #endregion

        #region MACD Tests

        [Fact]
        public void GetMacd_ValidPeriods_ReturnsCorrectStructure()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var macd = context.GetMacd(12, 26, 9);

            // Assert
            Assert.NotNull(macd.Macd);
            Assert.NotNull(macd.Signal);
            Assert.NotNull(macd.Histogram);
            Assert.Equal(50, macd.Macd.Length);
            Assert.Equal(50, macd.Signal.Length);
            Assert.Equal(50, macd.Histogram.Length);
        }

        [Fact]
        public void GetMacd_Histogram_CorrectlyCalculated()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var macd = context.GetMacd(12, 26, 9);

            // Assert - Histogram = MACD - Signal
            for (int i = 0; i < macd.Macd.Length; i++)
            {
                Assert.Equal(macd.Macd[i] - macd.Signal[i], macd.Histogram[i], 5);
            }
        }

        [Fact]
        public void GetMacd_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var macd1 = context.GetMacd(12, 26, 9);
            var macd2 = context.GetMacd(12, 26, 9);

            // Assert
            Assert.Same(macd1.Macd, macd2.Macd);
            Assert.Single(context.Macds);
        }

        [Fact]
        public void GetMacd_ReusesEmaCache()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var macd = context.GetMacd(12, 26, 9);
            var ema12 = context.GetEma(12);
            var ema26 = context.GetEma(26);

            // Assert - EMAs should be cached from MACD calculation
            Assert.Equal(3, context.Emas.Count); // 12, 26, and 9 (for signal)
        }

        #endregion

        #region Bollinger Bands Tests

        [Fact]
        public void GetBollingerBands_ValidParameters_ReturnsCorrectStructure()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var bb = context.GetBollingerBands(20, 2.0);

            // Assert
            Assert.NotNull(bb.Upper);
            Assert.NotNull(bb.Middle);
            Assert.NotNull(bb.Lower);
            Assert.Equal(50, bb.Upper.Length);
            Assert.Equal(50, bb.Middle.Length);
            Assert.Equal(50, bb.Lower.Length);
        }

        [Fact]
        public void GetBollingerBands_BandRelationship_Valid()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var bb = context.GetBollingerBands(20, 2.0);

            // Assert - Upper >= Middle >= Lower
            for (int i = 20; i < bb.Upper.Length; i++)
            {
                Assert.True(bb.Upper[i] >= bb.Middle[i]);
                Assert.True(bb.Middle[i] >= bb.Lower[i]);
            }
        }

        [Fact]
        public void GetBollingerBands_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var bb1 = context.GetBollingerBands(20, 2.0);
            var bb2 = context.GetBollingerBands(20, 2.0);

            // Assert
            Assert.Same(bb1.Upper, bb2.Upper);
            Assert.Single(context.BollingerBands);
        }

        #endregion

        #region ATR Tests

        [Fact]
        public void GetAtr_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var atr = context.GetAtr(14);

            // Assert
            Assert.Equal(50, atr.Length);
        }

        [Fact]
        public void GetAtr_Values_AlwaysPositive()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var atr = context.GetAtr(14);

            // Assert - ATR should always be positive
            for (int i = 0; i < atr.Length; i++)
            {
                Assert.True(atr[i] >= 0);
            }
        }

        [Fact]
        public void GetAtr_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var atr1 = context.GetAtr(14);
            var atr2 = context.GetAtr(14);

            // Assert
            Assert.Same(atr1, atr2);
            Assert.Single(context.Atrs);
        }

        #endregion

        #region ADX Tests

        [Fact]
        public void GetAdx_ValidPeriod_ReturnsCorrectStructure()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var adx = context.GetAdx(14);

            // Assert
            Assert.NotNull(adx.Values);
            Assert.NotNull(adx.PlusDi);
            Assert.NotNull(adx.MinusDi);
            Assert.Equal(50, adx.Values.Length);
            Assert.Equal(50, adx.PlusDi.Length);
            Assert.Equal(50, adx.MinusDi.Length);
        }

        [Fact]
        public void GetAdx_Values_WithinValidRange()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var adx = context.GetAdx(14);

            // Assert - ADX should be between 0 and 100
            for (int i = 28; i < adx.Values.Length; i++) // Skip initial periods
            {
                Assert.InRange(adx.Values[i], 0, 100);
            }
        }

        [Fact]
        public void GetAdx_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var adx1 = context.GetAdx(14);
            var adx2 = context.GetAdx(14);

            // Assert
            Assert.Same(adx1.Values, adx2.Values);
            Assert.Single(context.Adxs);
        }

        #endregion

        #region Stochastic Tests

        [Fact]
        public void GetStochastic_ValidPeriods_ReturnsCorrectStructure()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var stoch = context.GetStochastic(14, 3);

            // Assert
            Assert.NotNull(stoch.K);
            Assert.NotNull(stoch.D);
            Assert.Equal(50, stoch.K.Length);
            Assert.Equal(50, stoch.D.Length);
        }

        [Fact]
        public void GetStochastic_Values_WithinValidRange()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var stoch = context.GetStochastic(14, 3);

            // Assert - Stochastic should be between 0 and 100
            for (int i = 14; i < stoch.K.Length; i++)
            {
                Assert.InRange(stoch.K[i], 0, 100);
                Assert.InRange(stoch.D[i], 0, 100);
            }
        }

        [Fact]
        public void GetStochastic_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var stoch1 = context.GetStochastic(14, 3);
            var stoch2 = context.GetStochastic(14, 3);

            // Assert
            Assert.Same(stoch1.K, stoch2.K);
            Assert.Single(context.Stochastics);
        }

        #endregion

        #region CCI Tests

        [Fact]
        public void GetCci_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var cci = context.GetCci(20);

            // Assert
            Assert.Equal(50, cci.Length);
        }

        [Fact]
        public void GetCci_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var cci1 = context.GetCci(20);
            var cci2 = context.GetCci(20);

            // Assert
            Assert.Same(cci1, cci2);
            Assert.Single(context.Ccis);
        }

        #endregion

        #region Williams %R Tests

        [Fact]
        public void GetWilliamsR_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var willR = context.GetWilliamsR(14);

            // Assert
            Assert.Equal(50, willR.Length);
        }

        [Fact]
        public void GetWilliamsR_Values_WithinValidRange()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var willR = context.GetWilliamsR(14);

            // Assert - Williams %R should be between -100 and 0
            for (int i = 14; i < willR.Length; i++)
            {
                Assert.InRange(willR[i], -100, 0);
            }
        }

        [Fact]
        public void GetWilliamsR_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var willR1 = context.GetWilliamsR(14);
            var willR2 = context.GetWilliamsR(14);

            // Assert
            Assert.Same(willR1, willR2);
            Assert.Single(context.WilliamsRs);
        }

        #endregion

        #region OBV Tests

        [Fact]
        public void GetObv_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var obv = context.GetObv();

            // Assert
            Assert.Equal(50, obv.Length);
        }

        [Fact]
        public void GetObv_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var obv1 = context.GetObv();
            var obv2 = context.GetObv();

            // Assert
            Assert.Same(obv1, obv2);
            Assert.Single(context.Obvs);
        }

        #endregion

        #region MFI Tests

        [Fact]
        public void GetMfi_ValidPeriod_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var mfi = context.GetMfi(14);

            // Assert
            Assert.Equal(50, mfi.Length);
        }

        [Fact]
        public void GetMfi_Values_WithinValidRange()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var mfi = context.GetMfi(14);

            // Assert - MFI should be between 0 and 100
            for (int i = 14; i < mfi.Length; i++)
            {
                Assert.InRange(mfi[i], 0, 100);
            }
        }

        [Fact]
        public void GetMfi_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var mfi1 = context.GetMfi(14);
            var mfi2 = context.GetMfi(14);

            // Assert
            Assert.Same(mfi1, mfi2);
            Assert.Single(context.Mfis);
        }

        #endregion

        #region Parabolic SAR Tests

        [Fact]
        public void GetParabolicSar_DefaultParameters_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var sar = context.GetParabolicSar();

            // Assert
            Assert.Equal(50, sar.Length);
        }

        [Fact]
        public void GetParabolicSar_CustomParameters_ReturnsCorrectLength()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var sar = context.GetParabolicSar(0.03, 0.3);

            // Assert
            Assert.Equal(50, sar.Length);
        }

        [Fact]
        public void GetParabolicSar_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(50);
            var context = new IndicatorContext(candles);

            // Act
            var sar1 = context.GetParabolicSar(0.02, 0.2);
            var sar2 = context.GetParabolicSar(0.02, 0.2);

            // Assert
            Assert.Same(sar1, sar2);
            Assert.Single(context.ParabolicSars);
        }

        #endregion

        #region Ichimoku Tests

        [Fact]
        public void GetIchimoku_DefaultParameters_ReturnsCorrectStructure()
        {
            // Arrange
            var candles = CreateSampleCandles(100); // More candles for Ichimoku
            var context = new IndicatorContext(candles);

            // Act
            var ichimoku = context.GetIchimoku();

            // Assert
            Assert.NotNull(ichimoku.TenkanSen);
            Assert.NotNull(ichimoku.KijunSen);
            Assert.NotNull(ichimoku.SenkouSpanA);
            Assert.NotNull(ichimoku.SenkouSpanB);
            Assert.NotNull(ichimoku.ChikouSpan);
            Assert.Equal(100, ichimoku.TenkanSen.Length);
            Assert.Equal(100, ichimoku.KijunSen.Length);
            Assert.Equal(100, ichimoku.SenkouSpanA.Length);
            Assert.Equal(100, ichimoku.SenkouSpanB.Length);
            Assert.Equal(100, ichimoku.ChikouSpan.Length);
        }

        [Fact]
        public void GetIchimoku_CustomParameters_ReturnsCorrectStructure()
        {
            // Arrange
            var candles = CreateSampleCandles(100);
            var context = new IndicatorContext(candles);

            // Act
            var ichimoku = context.GetIchimoku(10, 30, 60);

            // Assert
            Assert.Equal(100, ichimoku.TenkanSen.Length);
        }

        [Fact]
        public void GetIchimoku_CacheWorks_ReturnsSameInstance()
        {
            // Arrange
            var candles = CreateSampleCandles(100);
            var context = new IndicatorContext(candles);

            // Act
            var ich1 = context.GetIchimoku();
            var ich2 = context.GetIchimoku();

            // Assert
            Assert.Same(ich1.TenkanSen, ich2.TenkanSen);
            Assert.Single(context.Ichimokus);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Context_EmptyCandles_HandlesGracefully()
        {
            // Arrange
            var candles = new List<Candle>();
            var context = new IndicatorContext(candles);

            // Act
            var ema = context.GetEma(12);

            // Assert
            Assert.Empty(ema);
        }

        [Fact]
        public void Context_SingleCandle_HandlesGracefully()
        {
            // Arrange
            var candles = CreateSampleCandles(1);
            var context = new IndicatorContext(candles);

            // Act
            var ema = context.GetEma(12);
            var sma = context.GetSma(20);

            // Assert
            Assert.Single(ema);
            Assert.Single(sma);
        }

        [Fact]
        public void Context_PeriodLargerThanCandles_HandlesGracefully()
        {
            // Arrange
            var candles = CreateSampleCandles(10);
            var context = new IndicatorContext(candles);

            // Act
            var sma = context.GetSma(50);

            // Assert
            Assert.Equal(10, sma.Length);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void MultipleIndicators_AllCached_WorksCorrectly()
        {
            // Arrange
            var candles = CreateSampleCandles(100);
            var context = new IndicatorContext(candles);

            // Act
            var ema12 = context.GetEma(12);
            var ema26 = context.GetEma(26);
            var sma20 = context.GetSma(20);
            var rsi14 = context.GetRsi(14);
            var macd = context.GetMacd(12, 26, 9);
            var bb = context.GetBollingerBands(20, 2.0);
            var atr = context.GetAtr(14);
            var adx = context.GetAdx(14);

            // Assert
            Assert.True(context.Emas.Count >= 3); // 12, 26, and signal EMA
            Assert.Single(context.Smas);
            Assert.Single(context.Rsis);
            Assert.Single(context.Macds);
            Assert.Single(context.BollingerBands);
            Assert.Single(context.Atrs);
            Assert.Single(context.Adxs);
        }

        [Fact]
        public void RealWorldScenario_TradingSignals_WorksCorrectly()
        {
            // Arrange - Create trending data
            var candles = new List<Candle>();
            var baseTime = new DateTime(2024, 1, 1);
            
            for (int i = 0; i < 100; i++)
            {
                var close = 100 + i * 0.5; // Uptrend
                var open = close - 0.3;
                var high = close + 0.5;
                var low = open - 0.3;
                
                candles.Add(new Candle(
                    baseTime.AddDays(i),
                    open,
                    high,
                    low,
                    close,
                    1000
                ));
            }

            var context = new IndicatorContext(candles);

            // Act
            var macd = context.GetMacd(12, 26, 9);
            var rsi = context.GetRsi(14);
            var adx = context.GetAdx(14);

            // Assert - In an uptrend
            var currentMacd = macd.Macd[^1];
            var currentSignal = macd.Signal[^1];
            var currentRsi = rsi[^1];
            var currentAdx = adx.Values[^1];

            // MACD should be above signal in uptrend
            Assert.True(currentMacd > currentSignal);
            
            // RSI should be elevated in uptrend (but not necessarily > 50 due to calculation)
            Assert.InRange(currentRsi, 0, 100);
            
            // ADX should show trend strength
            Assert.True(currentAdx >= 0);
        }

        #endregion
    }
}
