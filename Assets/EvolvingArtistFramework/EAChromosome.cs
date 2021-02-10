using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// We save position x,y, rotation and color
/// /// </summary>
public class EAChromosome : IChromosome, IBinaryChromosome
{
    #region Fields
    private Gene[] m_genes;
    private int _numberOfStrokes;
    private int _genesPerStroke;
    #endregion

    public double? Fitness { get; set; }

    public int Length { get; private set; }

    public EAChromosome(int numberOfStrokes, int genesPerStroke)
    {
        _numberOfStrokes = numberOfStrokes;
        _genesPerStroke = genesPerStroke;

        Length = _numberOfStrokes * _genesPerStroke;
        m_genes = new Gene[Length];
        for (int i = 0; i < Length; i++)
        {
            m_genes[i]=GenerateGene(i);
        }
    }
     
    public IChromosome Clone()
    {
        var clone = CreateNew();
        clone.ReplaceGenes(0, GetGenes());
        clone.Fitness = Fitness;

        return clone;
    }


    public int CompareTo(IChromosome other)
    {
        if (other == null)
        {
            return -1;
        }

        var otherFitness = other.Fitness;

        if (Fitness == otherFitness)
        {
            return 0;
        }

        return Fitness > otherFitness ? 1 : -1;
    }
    public IChromosome CreateNew()
    {
        return new EAChromosome(_numberOfStrokes, _genesPerStroke);
    }

    public void FlipGene(int index)
    {
        //var bitToFlip = Mathf.FloorToInt(UnityEngine.Random.Range(0, sizeof(float)*8));
        var bytes = BitConverter.GetBytes((ushort)m_genes[index].Value);
        var byteToFlipIn = Mathf.FloorToInt(UnityEngine.Random.Range(0, bytes.Length));
        var bitToFlip = Mathf.FloorToInt(UnityEngine.Random.Range(0, 8));
        bytes[byteToFlipIn] ^= (byte)(1 << bitToFlip);
        var v = (ushort)BitConverter.ToUInt16(bytes, 0); // this produces negative values
        ReplaceGene(index, new Gene(v));
    }

    public Gene GenerateGene(int geneIndex)
    {
        return new Gene((ushort)(UnityEngine.Random.value*ushort.MaxValue));
    }

    public Gene GetGene(int index)
    {
        return m_genes[index];
    } 

    public Gene[] GetGenes()
    {
        return m_genes;
    }

    public void ReplaceGene(int index, Gene gene)
    {
        m_genes[index] = gene;
        Fitness = null;
    }

    public void ReplaceGenes(int startIndex, Gene[] genes)
    {
        Array.Copy(genes, 0, m_genes, startIndex, genes.Length);

        Fitness = null;
    }

    public void Resize(int newLength)
    {
        Array.Resize(ref m_genes, newLength);
        Length = newLength;
    }

    public float[] ToFloatingPoints(){
        var result = new float[m_genes.Length];
        for (int i = 0; i < m_genes.Length; i++)
        {
            var v = (ushort)m_genes[i].Value / (float)ushort.MaxValue;
          //  Statistics.Instance.LogValue(v);
            result[i] = v;
            
        }
        return result;
    }

    public void LoadFloatingPoints(float[] values)
    {
        for (int i = 0; i < m_genes.Length; i++)
        {
            ReplaceGene(i, new Gene(values[i]*ushort.MaxValue));
        }
    }
}