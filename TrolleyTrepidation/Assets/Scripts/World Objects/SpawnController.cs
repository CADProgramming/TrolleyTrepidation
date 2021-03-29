using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.other.gameObject.name.Contains("Trolley"))
        {
            Destroy(collision.other.gameObject);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
