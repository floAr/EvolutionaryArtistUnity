using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEvolvedPhenotype<T> : MonoBehaviour, IPhenotype<T>
{
    public abstract int CalculateBitCount();

    public abstract T Get(float[] values, int startIndex);
    
}
