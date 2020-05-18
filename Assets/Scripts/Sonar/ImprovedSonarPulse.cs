using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ImprovedSonarPulse : MonoBehaviour {

    public GameObject[] sonarOrigins;
    
    public Transform sonarEffectOrigin;
    public GameObject playerSonarCollider;
    public GameObject pSonarColliderOrigin;
    public GameObject sonarSweepEmitter;
    public Transform playerSonarOriginSpawner;

    public Rigidbody rock;
    public Transform rockSpawn;

    public Material playerPingEffect;
    
    public float pulseDistance;
    public float maxPulseDistance = 30f;
    public float fadingScanWidth;


    private Camera _camera;
    public Material effectMaterial;
    private Vector3 lastPing;

    bool stopRunningTheFuckingPingScript;

    bool _pinging;

    MonsterSig[] _monsterSigs;
    

    void Start () {

        _monsterSigs = FindObjectsOfType<MonsterSig>();

	}
	
	
	void Update () {

        sonarOrigins = GameObject.FindGameObjectsWithTag("SoundOrigin");
        
        
        Debug.Log("origins currently in play: " + sonarOrigins.Length);


        //Press F to sonar
        if (Input.GetKeyDown(KeyCode.F))
        {
            effectMaterial = playerPingEffect;

            // Spawn Sonar Collider Sphere. This will tell the game if anything "hears" the
            // player or any object thrown by the player
            Instantiate(playerSonarCollider, playerSonarOriginSpawner.position, playerSonarOriginSpawner.rotation);

            // spawn emitter object for reverb particles (Yeah yeah I know sound is a wave
            // not a particle, but it was a hell of a lot easier to use a shit-ton of
            // particles with bounce physics to represent reverb than trying to wrap my brain
            // around how to make an image shader bounce off geometry, so lick me.)
            Instantiate(sonarSweepEmitter, playerSonarOriginSpawner.position, playerSonarOriginSpawner.rotation);
            
            //Get the coordinates from the very last location that the player emitted a sound
            lastPing.Set(playerSonarOriginSpawner.transform.position.x, 
                         playerSonarOriginSpawner.transform.position.y,
                         playerSonarOriginSpawner.transform.position.z);

            // make the actual sweep effect happen
            MakeSonarPing(lastPing, effectMaterial);
            
        }

        //This is the old Mouse click functionality. It used a ray cast to make sonar pings
        //happen at different locations on the map. I'm hanging on to this as a reference
        //in case I need it later.

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;


        //    _pinging = true;
        //    pulseDistance = 0;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        //ScannerOrigin.position = hit.point;
        //    }
        //}

    }

    //this should probably take a position coordinate as an argument
    //maybe pass in a sound type as well, so it knows what color to make the sweep

    //maybe make this so that it runs asyncronously? I think IEnumerator does that.
    public void MakeSonarPing(Vector3 pingCoord, Material pingColor)
    {
        //get the object type as well (probably object Tag, actually) and set effectMaterial to the material/color associated with that type of sound.
        //Player sounds should be GOLD, Monsters sounds should be RED, water sounds should be BLUE, and so on.
        effectMaterial = pingColor;

        _pinging = true;
        pulseDistance = 0;
        fadingScanWidth = 25f;
        stopRunningTheFuckingPingScript = false;

        lastPing.Set(pingCoord.x, pingCoord.y, pingCoord.z);
        Debug.Log("new ping coords:" + lastPing);
        StartCoroutine(Emit());
    }

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    public IEnumerator Emit()
    {

        //Pulses are currently interruptable and, while pulses can happen in multiple locations,
        //they cannot happen in multiple locations SIMULTANEOUSLY. If a pulse effect is currently
        //in progress and another pulse is created, that first pulse is interrupted to create a
        //new one at it's own point of origin. When this happens, the while loop does not
        //technically break (because _pinging would still technically be true), and the pulse
        //speed continues getting faster until the pulse reaches it's max size, at which point it
        //resets back to it's default speed. Basically, at some point, I will probably have to
        //rewrite this while loop, in order to correct this problem.
        while (_pinging == true)
        {
            //Pulse movement 
            pulseDistance += Time.deltaTime * 60;

            //Light up any monster hit by sonar pulse
            //
            //Honestly, this probably won't make it to the final game because of two reasons:
            //
            // 1. The monsters already create their own noises and sonar signatures so the player
            // would already be able to see them as long as the monster makes noise.
            //
            // 2. If the players sonar passes over a monster, the monster will react accordingly
            // having "heard" the player. Monster makes an alert sound and a sonar pulse is cast
            // which the player can see. Thus there's no real need for the monster to produce an
            // afterimage when it's detected by the player. They'll already know it's there. 
            //
            // Again, I'll still hang on to the code, as I may still use it to mark objective in
            // the future. Maybe.
            //
            //foreach (MonsterSig MSig in _monsterSigs)
            //{

            //    //stops the fucking Ping() script from running infinitely
            //    if (stopRunningTheFuckingPingScript == false)
            //    {
            //        //if the sonarPulse "hits" an object that would return a "signature" (i.e. a monster or objective item)
            //        if (Vector3.Distance(sonarEffectOrigin.transform.position, MSig.transform.position) <= pulseDistance)
            //        {
            //            //var lastPing = PlayerSonarOrigin.transform; 

            //            stopRunningTheFuckingPingScript = true;
            //            //_alertMonster.FollowTargetWithRotation(lastPing, 1, 5);
            //            MSig.Ping();
            //        }
            //    }
            //}

            //Why couldn't I fade the opacity on this again? Why was diminishing the width to zero the better option? -Past Matt
            //
            //Because, as of now, we still suck at writing shaders - Future Matt
            if (pulseDistance >= maxPulseDistance)
            {

                fadingScanWidth -= Time.deltaTime * 60;

                if (fadingScanWidth <= 0)
                {
                    _pinging = false;
                }
            }

            effectMaterial.SetFloat("_ScanWidth", fadingScanWidth);

            yield return null;
        }

        //if (_pinging == false && SonarEffectOrigin != null)
        //{
        //    Destroy(SonarEffectOrigin);
        //}

    }

    //ImageEffectOpaque allows the sonar sweep to render after opaque geometry, but before transparent geometry.
    //This allows the effect to appear obscured by other transparent effects such as fog. 
    //sonar should not be obscured by fog, however. That's the whole reason we're even using sonar in the first place
    //instead of something practical like Infrared or naked eye vision.
    //[ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        effectMaterial.SetVector("_WorldSpaceScannerPos", lastPing);

        effectMaterial.SetFloat("_ScanDistance", pulseDistance);
        RaycastCornerBlit(source, destination, effectMaterial);

    }

    //complicated computer magic that, at 12:30 in the morning, I can only say has something to do with how the sonar pulse renders to the screen.
    public void RaycastCornerBlit(RenderTexture source, RenderTexture destination, Material mat)
    {

        //compute frustum corners. yeah those are words.
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

        RenderTexture.active = destination;

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
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

}
