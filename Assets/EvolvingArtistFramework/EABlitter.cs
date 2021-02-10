//using GeneticSharp.Domain.Chromosomes;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using Random = UnityEngine.Random;

//public class EABlitter : MonoBehaviour

//{ // Copies aTexture to rTex and displays it in all cameras.
//    public BlitToScreen CameraOutput;

//    public RenderTexture Target;
//    public RenderTexture Target2;

//    public Material BrushMaterial;

//    private bool flip = false;

//    private FloatingPointChromosome _activeChromosome;
//    private bool _needsRepaint;
//    public FloatingPointChromosome ActiveChromosome
//    {
//        set
//        {
//            _activeChromosome = value;
//            _needsRepaint = true;
//        }
//    }

//    public void Update()
//    {
//        if (_needsRepaint && _activeChromosome != null)
//        {
//            Clear();
//            CameraOutput.Render= BlitChromosome(_activeChromosome);
//            _needsRepaint = false;
//        }
//    }

//    public void Clear()
//    {
//        RenderTexture rt = UnityEngine.RenderTexture.active;
//        UnityEngine.RenderTexture.active = Target;
//        GL.Clear(true, true, Color.clear);

//        UnityEngine.RenderTexture.active = Target2;
//        GL.Clear(true, true, Color.clear);
//        UnityEngine.RenderTexture.active = rt;
//    }

//    public int _geneCount=8;
//    public RenderTexture BlitChromosome(FloatingPointChromosome c)
//    {
//        var values = c.ToFloatingPoints();
//        if (values.Length % _geneCount != 0)
//            throw new ArgumentException($"Wrong number of genes in chromosome. Expected multiple of 4, got {values.Length}");
//        var num_strokes = values.Length / _geneCount;
//        var shapeBuffer = new ComputeBuffer(num_strokes, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
//        var colorBuffer = new ComputeBuffer(num_strokes, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));

//        Vector4[] shapeData = new Vector4[num_strokes];
//        Vector4[] colorData = new Vector4[num_strokes];


//        for (int i = 0; i < num_strokes; i++)
//        {
//            shapeData[i] = new Vector4(
//                (float)(values[i * _geneCount]), (float)(values[i * _geneCount + 1]),     // position 
//                (float)(values[i * _geneCount + 2]) * 360,                     // rotation
//                (float)(values[i * _geneCount + 3]));                          // scale
//            colorData[i] = new Vector3(
//                (float)(values[i * _geneCount + 4]),
//                (float)(values[i * _geneCount + 5]),
//                (float)(values[i * _geneCount + 6])); // color
//            brushData[i] = Mathf.RoundToInt(values[i * _geneCount + 7] * BrushPack.depth);
//        }

//        shapeBuffer.SetData(shapeData);
//        colorBuffer.SetData(colorData);
//        brushBuffer.SetData(brushData);

//        BrushMaterial.SetBuffer("shapeBuffer", shapeBuffer);
//        BrushMaterial.SetBuffer("colorBuffer", colorBuffer);
//        BrushMaterial.SetBuffer("brushBuffer", brushBuffer);
//        BrushMaterial.SetInt("_StrokeCount", num_strokes);
//        BrushMaterial.SetFloat("_BrushBaseSize", _currentScale);


//        Graphics.Blit(Target, Target2, BrushMaterial);
//        if (flip)
//        {
//            return Target;
//        }
//        else
//        {
//            return Target2;
//        }
//    }
//}

