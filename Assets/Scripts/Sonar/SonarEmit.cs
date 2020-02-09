using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarEmit : MonoBehaviour
{
    //Should probably pass in a volume variable to influence the max size of a sonar pulse
    public GameObject sonarPulse;
    public Transform spawnPoint;
    public GameObject SonarOrigin;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(SonarOrigin, spawnPoint.position, spawnPoint.rotation);
            Instantiate(sonarPulse, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
