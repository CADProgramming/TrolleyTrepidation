using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrolleyHandleTrigger : MonoBehaviour
{
   

    public void OnTriggerEnter(Collider other)
    {
        transform.parent = other.gameObject.transform.parent;
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
