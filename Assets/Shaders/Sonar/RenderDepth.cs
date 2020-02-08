using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderDepth : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {

        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;

	}
}
