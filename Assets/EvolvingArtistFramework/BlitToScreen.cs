using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class BlitToScreen : MonoBehaviour
{
    public Material material;
    public RenderTexture Render;

        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(Render, destination);
        }
    
}
 