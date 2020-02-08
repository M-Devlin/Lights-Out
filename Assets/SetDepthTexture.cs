using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[ExecuteInEditMode]
public class SetDepthTexture : MonoBehaviour
{

    // Use this for initialization
    public DepthTextureMode mode = DepthTextureMode.Depth;

    public void Start()
    {

    }

    // before a camera renders this 
    public void OnWillRenderObject()
    {

        if (!enabled)
            return;

        Camera cam = Camera.current;
        if (!cam)
            return;

        cam.depthTextureMode = mode;

    } 
}
