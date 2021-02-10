using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImageComparer : MonoBehaviour
{
    public ComputeShader _cShader;


    public float maxValue;
    public int BucketSize = 1;

    public void CalcMaxValue(RenderTexture T1, RenderTexture T2)
    {
        maxValue = 1;// T1.width * T1.height * 3 * 255;
        maxValue = Compare(T1, T2);
    }

    [ContextMenu("Dispatch")]
    public float Compare(RenderTexture T1, RenderTexture T2)
    {

        var outputBuffer = new ComputeBuffer(BucketSize, sizeof(int));
        var pixelCount = (float)T1.width * T1.height;
        int kernel = _cShader.FindKernel("CSMain");
        _cShader.SetTexture(kernel, "T1", T1);
        _cShader.SetTexture(kernel, "T2", T2);

        _cShader.SetInt("BucketSize", BucketSize);

        var result = Enumerable.Repeat(0, BucketSize).ToArray();
          

        outputBuffer.SetData(result);
        _cShader.SetBuffer(kernel, "Error", outputBuffer);

        _cShader.Dispatch(kernel, Mathf.CeilToInt(T1.width / 8f), Mathf.CeilToInt(T1.height / 8f), 1);

        outputBuffer.GetData(result);
        outputBuffer.Release();
        var summed_result = result.Sum(x => x / pixelCount / maxValue);
        // return ((float)result[0]/maxValue);
        return summed_result;
    }
}
