using UnityEngine;

public class FloatEvolutionaryValue : EvolutionalValue<float>
{
    public float MinValue = 0;
    public float MaxValie = 1;
    public override int GeneCount() => 1;

    protected override float Map(ref float[] values, int index)
    {
        return Mathf.Lerp(MinValue, MaxValie, values[index]);
    }
}