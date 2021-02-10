using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatChangeOnIterationValue : ChangingPerIterationValue<float>
{
    public float MinValue;
    public float MaxValue;
    public AnimationCurve ChangeCurve;

    protected override float CalcNewValue(int current, int max)
    {
        return Mathf.Lerp(MinValue, MaxValue, ChangeCurve.Evaluate((float)current / (float)max));
    }
}