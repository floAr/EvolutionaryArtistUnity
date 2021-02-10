using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "EvolvingArtist/BrushPack", order = 1)]

public class BrushPack : ScriptableObject
{


    [SerializeField]
    public Texture2DArray BrushPackArray;

   

    public int GetBrushCount()
    {
        return BrushPackArray.depth;
    }

    public Texture2DArray GetBrushPack()
    {
        return BrushPackArray;
    }
}
