using Santel.TradeSharp.Indicators;

var candles = new List<Candle>
{
	new(DateTime.UtcNow.AddMinutes(-4), 100, 105, 95, 102),
	new(DateTime.UtcNow.AddMinutes(-3), 102, 106, 101, 104),
	new(DateTime.UtcNow.AddMinutes(-2), 104, 108, 103, 107),
	new(DateTime.UtcNow.AddMinutes(-1), 107, 109, 105, 106),
	new(DateTime.UtcNow, 106, 110, 104, 109)
};

var context = new IndicatorContext(candles);

var ema12 = context.CalculateEma(12);
var macd = context.CalculateMacd(12, 26, 9);

Console.WriteLine($"{ema12.Name}: {ema12.Values[^1]:F2}");
Console.WriteLine($"{macd.Macd.Name}: {macd.Macd.Values[^1]:F2}");
