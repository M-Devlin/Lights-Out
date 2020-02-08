using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteInEditMode]
public class MonsterSonar : MonoBehaviour
{


    public Transform PlayerSonarOrigin;
    //public Transform MonsterSonarOrigin;
    //public Transform RockSonarOrigin;

    public Material EffectMaterial;
    public float PulseDistance;
    public float MaxPulseDistance = 50f;

    public ParticleSystem sonarParticles;

    private Camera _camera;
    //public MonsterInvestigate _alertMonster;
    private float timer = 0f;

    bool stopRunningTheFuckingPingScript;

    bool _pinging;


    void Start()
    {
        
    }


    void Update()
    {

        timer += Time.deltaTime * 1;

        if (timer >= 15)
        {
            _pinging = true;
            Emit();
        }

    }

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    public IEnumerator Emit()
    {


        while (_pinging == true)
        {
            //Pulse movement
            PulseDistance += Time.deltaTime * 120;


            if (PulseDistance >= MaxPulseDistance)
            {
                PulseDistance = 0;
                _pinging = false;
            }

            yield return null;
        }

    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        EffectMaterial.SetVector("_WorldSpaceScannerPos", PlayerSonarOrigin.position);
        EffectMaterial.SetFloat("_ScanDistance", PulseDistance);
        RaycastCornerBlit(src, dst, EffectMaterial);
    }

    //complicated computer magic that, at 12:30 in the morning, I can only say has something to do with how the sonar pulse works.
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
