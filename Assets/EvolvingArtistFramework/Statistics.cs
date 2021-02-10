using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Statistics 
{
    private List<int> _buckets;
    private int _bucketCount = 100;



    public Statistics()
    {
        _buckets = new List<int>(Enumerable.Repeat<int>(0,_bucketCount));
    }

    public void LogValue(float value)
    {
        var b = Mathf.FloorToInt(value * (_bucketCount-1));
        if (b < 0 || b >= _bucketCount)
            Debug.Log("error");
        _buckets[b] += 1;
    }







    private static Statistics _instance;

    public static Statistics Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Statistics();
            }
            return _instance;
        }
    }

    internal string GetBuckets()
    {
        return  string.Join(",", _buckets);
    }
    internal void ClearBuckets()
    {
        _buckets = new List<int>(Enumerable.Repeat<int>(0, _bucketCount));
    }

}
