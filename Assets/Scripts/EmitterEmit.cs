using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterEmit : MonoBehaviour
{
    public Transform sonarEffectOrigin;
    public Transform playerSonarOriginSpawner;

    public Material EffectMaterial;
    public float PulseDistance;
    public float MaxPulseDistance = 30f;
    public float fadingScanWidth;

    public ParticleSystem sonarParticles;

    private Camera _camera;


    bool stopRunningTheFuckingPingScript;

    bool _pinging;

    private MonsterSig[] _monsterSigs; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Pulse movement
        PulseDistance += Time.deltaTime * 60;

        //Light up any monster hit by sonar pulse
        foreach (MonsterSig MSig in _monsterSigs)
        {

            //stops the fucking .Ping() script from running infinitely
            if (stopRunningTheFuckingPingScript == false)
            {
                //if the sonarPulse "hits" an object that would return a "signature" (i.e. a monster or objective item)
                if (Vector3.Distance(sonarEffectOrigin.position, MSig.transform.position) <= PulseDistance)
                {
                    //var lastPing = PlayerSonarOrigin.transform; 

                    stopRunningTheFuckingPingScript = true;
                    //_alertMonster.FollowTargetWithRotation(lastPing, 1, 5);
                    MSig.Ping();
                }
            }
        }

        if (PulseDistance >= MaxPulseDistance)
        {

            fadingScanWidth -= Time.deltaTime * 100;

            if (fadingScanWidth <= 0)
            {
                _pinging = false;
            }
        }

        EffectMaterial.SetFloat("_ScanWidth", fadingScanWidth);
    }

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        EffectMaterial.SetVector("_WorldSpaceScannerPos", sonarEffectOrigin.position);
        EffectMaterial.SetFloat("_ScanDistance", PulseDistance);
        RaycastCornerBlit(src, dst, EffectMaterial);
    }

    //complicated computer magic that, at 12:30 in the morning, I can only say has something to do with how the sonar pulse renders to the screen.
    public void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        float camFar = _camera.farClipPlane;
        float camFov = _camera.fieldOfView;
        float camAspect = _camera.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = _camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = _camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (_camera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (_camera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (_camera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (_camera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
