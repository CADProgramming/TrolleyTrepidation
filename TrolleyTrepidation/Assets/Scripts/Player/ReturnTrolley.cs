using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTrolley : MonoBehaviour
{
    public GameObject player;
    private Component trolleyList;
    
    private void OnTriggerEnter(Collider other)
    {
        trolleyList = player.GetComponent<PlayerMovementNew>();
        
        if (other.name.Contains("Trolley"))
        {
            Destroy(other.gameObject);
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
