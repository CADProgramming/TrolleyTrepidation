using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public GameObject myPrefab;
    public Transform location;
    public GameObject[] arrayTrolleys;
    // Start is called before the first frame update
    void Start()
    {
        arrayTrolleys = new GameObject[51];
        for(int i = 0; i < 10; i++)
        {
            arrayTrolleys[i] = Instantiate(myPrefab, SpawnRandomLocation(), Quaternion.identity);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    Vector3 SpawnRandomLocation()
    {
        return new Vector3(Random.Range(location.position.x - (location.localScale.x / 2), location.position.x + (location.localScale.x /2)), 0, Random.Range(location.position.z - (location.localScale.z /2), location.position.z + (location.localScale.z /2)));
    }
}
