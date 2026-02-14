namespace Santel.TradeSharp.Indicators;

public sealed class ParabolicSarIndicator
{
    public ParabolicSarIndicator(double accelerationFactor, double maxAcceleration, double[] values)
    {
        AccelerationFactor = accelerationFactor;
        MaxAcceleration = maxAcceleration;
        Values = values;
    }

    public double AccelerationFactor { get; }
    public double MaxAcceleration { get; }
    public double[] Values { get; }
}
