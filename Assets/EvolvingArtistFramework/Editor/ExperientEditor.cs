using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Experiment))]
public class ExperientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Save Experiment"))
        {
            var expPath = Path.Combine(Application.dataPath, "Experiments");
            var exp = target as Experiment;
            var prefabPath = expPath + $"/{exp.Name}.prefab";
          
          PrefabUtility.SaveAsPrefabAsset(exp.gameObject, prefabPath);
        }
    }
}
