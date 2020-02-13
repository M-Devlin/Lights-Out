using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterInvestigate : MonoBehaviour
{

    public Transform target;
    public float speed = 10.0f;
    public GameObject MonsterEcho;


    public float wanderRadius;
    public float wanderTimer;

    private NavMeshAgent agent;
    private float timer;
    bool _idling;

    public Material pingColor;
    ImprovedSonarPulse sonarEmitter;


    // Use this for initialization
    void Start()
    {

        _idling = true;
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;

        StartCoroutine(MonsterIdle());
    }
       
    //Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            //sonarEmitter.Emit();
            //Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            //agent.SetDestination(newPos);
            timer = 0;
        }


    }


    IEnumerator MonsterIdle()
    {

        GameObject camera = GameObject.Find("FirstPersonCharacter");
        ImprovedSonarPulse improvedSonarPulse = camera.GetComponent<ImprovedSonarPulse>();

        while (_idling == true)
        {

            yield return new WaitForSeconds(0.13f);
            improvedSonarPulse.MakeSonarPing(gameObject.transform.position, pingColor);
            yield return new WaitForSeconds(0.13f);
            improvedSonarPulse.MakeSonarPing(gameObject.transform.position, pingColor);
            Debug.Log("Grr...");
            yield return new WaitForSeconds(3f);
        }

    }

    IEnumerator FollowTargetWithRotation(GameObject lastPing, float distanceToStop, float speed)
    {
        

        while (Vector3.Distance(transform.position, lastPing.transform.position) > distanceToStop)
        {
            if (lastPing != null)
            {
                Vector3 targetPosition = new Vector3(lastPing.transform.position.x, transform.position.y, lastPing.transform.position.z);

                transform.LookAt(targetPosition);
                //GameObject.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
                transform.position = Vector3.MoveTowards(transform.position, lastPing.transform.position, Time.deltaTime * 3);
            }
            //This will probably cause the monster to immediately break pursuit and return to idle
            //else
            //{
            //    _idling = true;
            //    MonsterIdle();
            //}

            yield return null;
        }
    }

    //public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    //{
    //    Vector3 randomDirection = Random.insideUnitSphere * distance;

    //    randomDirection += origin;

    //    NavMeshHit navHit;

    //    NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

    //    return navHit.position;
    //}


    void OnTriggerEnter(Collider sound)
    {

        GameObject[] soundOrigin = GameObject.FindGameObjectsWithTag("SoundOrigin");
        GameObject lastPing = soundOrigin[soundOrigin.Length - 1];

        if (sound.gameObject.tag.Equals("Sound") == true)
        {

            _idling = false;

            GameObject camera = GameObject.Find("FirstPersonCharacter");
            ImprovedSonarPulse improvedSonarPulse = camera.GetComponent<ImprovedSonarPulse>();

            improvedSonarPulse.MakeSonarPing(gameObject.transform.position, pingColor);
            Debug.Log("Wuzzat?");

            StartCoroutine(FollowTargetWithRotation(lastPing, 1, 5));
        }
    }
}
