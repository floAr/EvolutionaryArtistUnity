using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3OneFromNEvolutionaryValue : EvolutionalValue<Vector3>
{
    public List<Color> Options;
    public override int GeneCount() => 1;

    protected override Vector3 Map(ref float[] values, int index)
    {
        var pick =  Mathf.FloorToInt(Mathf.Abs(values[index]) * Options.Count);
        if (pick < 0)
            pick = 0;
        var c = Options[pick % Options.Count];
        return new Vector3(c.r,c.g,c.b);
    }
}