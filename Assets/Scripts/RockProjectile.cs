using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProjectile : MonoBehaviour {

    public Rigidbody Rock;
    public Transform spawnPoint;


    // Use this for initialization
    void Start () {
		
	}

    Rigidbody rockInstance;
    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            
            rockInstance = Instantiate(Rock, spawnPoint.position, spawnPoint.rotation);

            rockInstance.velocity = spawnPoint.TransformDirection(Vector3.forward * 20);
            
        }
	}
}
