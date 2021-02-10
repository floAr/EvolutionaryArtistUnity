using UnityEngine;

public class Vector3EvolutionaryValue : EvolutionalValue<Vector3>
{
    public override int GeneCount() => 3;

    protected override Vector3 Map(ref float[] values, int index)
    {
        return new Vector3(values[index], values[index+1], values[index+2]);
    }
}