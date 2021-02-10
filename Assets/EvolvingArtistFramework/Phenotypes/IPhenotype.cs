using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPhenotype <T>
{
    int CalculateBitCount();
     T Get(float[] values, int startIndex);
}
