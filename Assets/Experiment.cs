using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Experiment : MonoBehaviour
{
    public string Name;
    public Texture2DArray BrushPack;

    public int Iterations = 5;
    public Color CanvasColor = Color.white;
    public Texture2D Source;
    public int StrokesPerGeneration = 10;

    public BaseValue<float> BaseSize;
    public BaseValue<float> BrushTransparency;

    private FloatEvolutionaryValue _x;
    private FloatEvolutionaryValue _y;
    public BaseValue<float> RotationValue;
    public BaseValue<float> ScaleValue;
    public BaseValue<Vector3> ColorValue;
    public BaseValue<int> BrushValue;


    public Action<int> NewGenerationStarted;
    public delegate void NewIterationDelegate(int current, int max);
    public NewIterationDelegate NewIterationStarted;

    private void Start()
    {
        _x = this.gameObject.AddComponent<FloatEvolutionaryValue>();
        _y = this.gameObject.AddComponent<FloatEvolutionaryValue>();

        _x.Register(this);
        _y.Register(this);
        RotationValue.Register(this);
        ScaleValue.Register(this);
        ColorValue.Register(this);
        BrushValue.Register(this);
        BaseSize.Register(this);
        BrushTransparency.Register(this);
    }

    public int GetGeneCount() =>
        _x.GeneCount() +
        _y.GeneCount() +
        RotationValue.GeneCount() +
        ScaleValue.GeneCount() +
        ColorValue.GeneCount() +
        BrushValue.GeneCount() +
        BaseSize.GeneCount()+
        BrushTransparency.GeneCount();

    public void StartNewGeneration(int generationNumber)
    {
        NewGenerationStarted?.Invoke(generationNumber);
    }

    public void StartNewIteration(int currentIteration)
    {
       NewIterationStarted?.Invoke(currentIteration, Iterations);
    }

    public void ParseValues(ref float[] values, ref Vector4[] shape, ref Vector3[] color, ref int[] brush)
    {
        if (values.Length % GetGeneCount() != 0)
            throw new ArgumentException($"Wrong number of genes in chromosome. Expected multiple of {GetGeneCount()}, got {values.Length}");

        for (int i = 0; i < StrokesPerGeneration; i++)
        {
            var currentIndex = i * GetGeneCount();
            shape[i] = new Vector4(
                _x.Get(ref values, ref currentIndex),
                _y.Get(ref values, ref currentIndex),    // position 
                RotationValue.Get(ref values, ref currentIndex),                     // rotation
                ScaleValue.Get(ref values, ref currentIndex));                          // scale
            color[i] = ColorValue.Get(ref values, ref currentIndex); // color
            brush[i] = BrushValue.Get(ref values, ref currentIndex);
        }
    }
}
