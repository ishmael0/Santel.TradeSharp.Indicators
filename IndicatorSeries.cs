namespace Santel.TradeSharp.Indicators;

public sealed class IndicatorSeries<T>
{
    public IndicatorSeries(string name, T[] values)
    {
        Name = name;
        Values = values;
    }

    public string Name { get; }
    public T[] Values { get; }
}
