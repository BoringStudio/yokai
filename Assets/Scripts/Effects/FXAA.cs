using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent( typeof( Camera ) )]
[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : MonoBehaviour
{
    private Shader shader;
    private Material material;

    void Start()
    {
        shader = Shader.Find("Hidden/FXAA");
        CreateMaterials();

        if (!SystemInfo.supportsImageEffects)
        {
            this.enabled = false;
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CreateMaterials();

        float rcpWidth = 1.0f / Screen.width;
        float rcpHeight = 1.0f / Screen.height;

        material.SetVector("_rcpFrame", new Vector4(rcpWidth, rcpHeight, 0, 0));
        material.SetVector("_rcpFrameOpt", new Vector4(rcpWidth * 2, rcpHeight * 2, rcpWidth * 0.5f, rcpHeight * 0.5f));

        Graphics.Blit(source, destination, material);
    }

    public void CreateMaterials()
    {
        if (material == null)
        {
            if (!shader)
            {
                Debug.Log("Missing shader in " + this.ToString());
                this.enabled = false;
            }

            if (shader.isSupported)
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.DontSave;
            }
            else
            {
                this.enabled = false;
                Debug.LogError("The shader " + shader.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
            }
        }
	}
}
