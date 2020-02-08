using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockEcho : MonoBehaviour
{

    public GameObject rockSonarEmitter;
    public Transform rSonarEmitCoords;
    public Material pingColor;
    
    MonsterSig[] _monsterSigs;
    // Use this for initialization
    void Start()
    {
        _monsterSigs = FindObjectsOfType<MonsterSig>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {


        GameObject camera = GameObject.Find("FirstPersonCharacter");
        ImprovedSonarPulse improvedSonarPulse = camera.GetComponent<ImprovedSonarPulse>();
        
        Instantiate(rockSonarEmitter, gameObject.transform.position, gameObject.transform.rotation);
        if (collision != null)
        {
            ContactPoint contact = collision.contacts[0];

            Debug.Log(collision.contacts.Length);

            improvedSonarPulse.MakeSonarPing(contact.point, pingColor);
        }
        
    }
}
    