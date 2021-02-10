using UnityEngine;

public class OneFromNEvolutionaryValue : EvolutionalValue<int>
{
    public Texture2DArray SetTextureToCalcOptions;
    public int NumberOfOptions;
    public override int GeneCount() => 1;

    private void OnValidate()
    {
        if (SetTextureToCalcOptions != null)
        {
            NumberOfOptions = SetTextureToCalcOptions.depth;
            SetTextureToCalcOptions = null;
        }
    }

    protected override int Map(ref float[] values, int index)
    {
        return Mathf.RoundToInt(values[index] * NumberOfOptions);
    }
}