using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IConstantPhenotype<T> : MonoBehaviour, IPhenotype<T>
{
    public T Value;

    public int CalculateBitCount() => 0;

    public T Get(float[] values, int startIndex) => Value;

}
