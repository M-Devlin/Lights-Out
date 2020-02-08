using UnityEngine;
using System.Collections;

public class MonsterSig : MonoBehaviour
{
    //get monster transform data
    public Transform monster;

    public GameObject monsterSonarSig;

    public void Ping()
    {
        //instantiate duplicate of monster mesh at monsters ping location with red mesh color
        Instantiate(monsterSonarSig, monster.transform.position, monster.rotation);
    }
}