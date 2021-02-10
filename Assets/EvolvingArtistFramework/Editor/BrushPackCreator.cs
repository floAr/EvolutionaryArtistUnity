using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BrushPackCreator : EditorWindow
{
    [MenuItem("EvolvingArtist/BrushPack Creator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BrushPackCreator window = (BrushPackCreator)EditorWindow.GetWindow(typeof(BrushPackCreator));
        window.Show();
    }

    private string _name = "BrushPack";
    private List<Texture2D> _textures = new List<Texture2D>();
    private int _textureCount = 0;

    private Texture2D _tempTex;

    private Texture2DArray _arrayToRender;

    void OnGUI()
    {
        _name = EditorGUILayout.TextField("Name", _name);
        GUILayout.Label("Textures", EditorStyles.boldLabel);
        for (int i = 0; i < _textureCount; i++)
        {
            _textures[i] = (Texture2D)EditorGUILayout.ObjectField(_textures[i], typeof(Texture2D), false);
        }
        EditorGUILayout.BeginHorizontal();

        _tempTex = (Texture2D) EditorGUILayout.ObjectField(_tempTex, typeof(Texture2D),false);
        if (_tempTex != null)
        {
            _textures.Add(_tempTex);
            _textureCount++;
            _tempTex = null;
        }

        if (GUILayout.Button("+"))
        {
            _textures.Add(new Texture2D(1, 1));
            _textureCount++;
        }
        if (GUILayout.Button("-"))
        {
            _textures.RemoveAt(_textureCount-1);
            _textureCount--;
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Bake"))
        {

            if (_textures != null && _textures.Count > 0)
            {
                var _brushPack = new
                    Texture2DArray(_textures[0].width,
                    _textures[0].height, _textures.Count, TextureFormat.RGBA32, false);
                // Apply settings
                _brushPack.anisoLevel = _textures[0].anisoLevel;              

                _brushPack.filterMode = _textures[0].filterMode;
                _brushPack.wrapMode = _textures[0].wrapMode;
                var test = _textures[0].GetPixel(0, 0);
                Debug.Log(test);
                // Loop through ordinary textures and copy pixels to the Texture2DArray
                for (int i = 0; i < _textures.Count; i++)
                {
                    _brushPack.SetPixels(_textures[i].GetPixels(0), i, 0);
                }    
                // Apply our changes
                _brushPack.Apply();
                var test2 = _brushPack.GetPixels(0)[0];
                Debug.Log(test2);

                string path = $"Assets/Content/BrushPacks/{_name}.asset";
                AssetDatabase.CreateAsset(_brushPack, path);
                Debug.Log("Saved asset to " + path);
                AssetDatabase.Refresh();
            }
        }

        _arrayToRender = (Texture2DArray)EditorGUILayout.ObjectField(_arrayToRender, typeof(Texture2DArray), false);
        if (_arrayToRender != null)
        {
            var cols = Mathf.Min(4, _arrayToRender.depth);
            var rows = Mathf.CeilToInt(_arrayToRender.depth / 4f);
            var renderTex = new RenderTexture(512 * cols, 512 * rows, 1);
            Material m = new Material(Shader.Find("Unlit/RenderBrushPack"));
            m.SetTexture("_Brush", _arrayToRender);
            m.SetInt("_BrushCount", _arrayToRender.depth);

            Graphics.Blit(renderTex, renderTex, m);
            Texture2D tex = new Texture2D(renderTex.width, renderTex.height);

            RenderTexture.active = renderTex;

            tex.Resize(renderTex.width, renderTex.height);
            tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            RenderTexture.active = null;

            byte[] bytes;
            bytes = tex.EncodeToPNG();

            System.IO.File.WriteAllBytes($"{Application.dataPath}/{_arrayToRender.name}.png", bytes);

            _arrayToRender = null;


            //   byte[] bytes;
            //  bytes = renderTex.EncodeToPNG();

            //  System.IO.File.WriteAllBytes($"{_outputRoot}/{_folder}/final-{_maxIterations - _iterations}.png", bytes);

            _arrayToRender = null;
        }
    }
}


//[MenuItem("GameObject/Create Texture Array")]
//static void Create()
//{
//    // CHANGEME: Filepath must be under "Resources" and named appropriately. Extension is ignored.
//    // {0:000} means zero padding of 3 digits, i.e. 001, 002, 003 ... 010, 011, 012, ...
//    string filePattern = "Smoke/smoke_{0:000}";

//    // CHANGEME: Number of textures you want to add in the array
//    int slices = 24;

//    // CHANGEME: TextureFormat.RGB24 is good for PNG files with no alpha channels. Use TextureFormat.RGB32 with alpha.
//    // See Texture2DArray in unity scripting API.
//    Texture2DArray textureArray = new Texture2DArray(256, 256, slices, TextureFormat.RGB24, false);

//    // CHANGEME: If your files start at 001, use i = 1. Otherwise change to what you got.
//    for (int i = 1; i <= slices; i++)
//    {
//        string filename = string.Format(filePattern, i);
//        Debug.Log("Loading " + filename);
//        Texture2D tex = (Texture2D)Resources.Load(filename);
//        textureArray.SetPixels(tex.GetPixels(0), i, 0);
//    }
//    textureArray.Apply();

//    // CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
//    string path = "Assets/Tasmania/Resources/SmokeTextureArray.asset";
//    AssetDatabase.CreateAsset(textureArray, path);
//    Debug.Log("Saved asset to " + path);
//}