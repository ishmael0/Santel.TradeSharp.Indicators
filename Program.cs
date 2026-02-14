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

var ema12 = context.GetEma(12);
var macd = context.GetMacd(12, 26, 9);

Console.WriteLine($"EMA(12): {ema12[^1]:F2}");
Console.WriteLine($"MACD: {macd.Macd[^1]:F2}");
Console.WriteLine($"Total EMAs calculated: {context.Emas.Count}");
Console.WriteLine($"Total MACDs calculated: {context.Macds.Count}");
