using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseValue<T> : MonoBehaviour
{
    public string Name;

    public virtual void Register(Experiment parent) { }

    public abstract T Get(ref float[] values, ref int startIndex);
    public abstract int GeneCount();
}


public abstract class ConstantValue<T> : BaseValue<T>
{
    public T Value;
    public override int GeneCount() => 0;
    public override T Get(ref float[] values, ref int startIndex)
    {
        return Value;
    }
}

public abstract class EvolutionalValue<T> : BaseValue<T>
{
    public override T Get(ref float[] values, ref int startIndex)
    {
        var value = Map(ref values, startIndex);
        startIndex += GeneCount();
        return value;
    }

    protected abstract T Map(ref float[] values, int index);
}


public abstract class ChangingPerIterationValue<T> : BaseValue<T>
{
    private T _value;
    public override int GeneCount() => 0;
    public override T Get(ref float[] values, ref int startIndex)
    {
        return _value;
    }
    public override void Register(Experiment parent)
    {
        parent.NewIterationStarted += ChangeValue;
        base.Register(parent);
    }

    private void ChangeValue(int current, int max)
    {
        _value = CalcNewValue(current, max);
    }

    protected abstract T CalcNewValue(int current, int max);
}