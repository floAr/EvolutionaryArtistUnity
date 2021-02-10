

#if UNITY_EDITOR
using System;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoadAttribute]
public static class BatchProcessor
{
    private static ManualResetEvent eventBlock = new ManualResetEvent(false);
    static BatchProcessor()
    {
        if (Application.isBatchMode)
        {
            EditorApplication.playModeStateChanged += LoadDefaultScene;
            EditorSceneManager.sceneLoaded += SceneLoaded;
        }
    }

  

    [MenuItem("EvolvingArtist/Try me")]
    public static void ExecuteBatchEA()
    {
        EditorApplication.EnterPlaymode();
        var arguments = System.Environment.GetCommandLineArgs();
        Console.WriteLine("Hello");
        
    }

    static void LoadDefaultScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorSceneManager.LoadScene(0);
        }
    }

    private static void SceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
    {
        if (arg0.name.Equals("EA2"))
        {
            var EaController = GameObject.FindObjectOfType<EAController2>();
            EaController.Finished += GenerationFinished;
            EaController.AutoStart = true;
            Debug.Log($"C: {EaController}");
           // EaController.Run();
        }
    }

    private static void GenerationFinished(string obj)
    {
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(0);
    }
}
#endif
