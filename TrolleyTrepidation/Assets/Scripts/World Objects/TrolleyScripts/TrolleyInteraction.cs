using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrolleyInteraction : MonoBehaviour
{
    public float distance = 1.3F;
    private void OnTriggerEnter(Collider other)
    {
        /*if (!this.transform.Find(other.name))
        {
            if (other.name.Contains("Trolley"))
            {
                AddTrolley(other.gameObject);
            }
        }*/
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddTrolley(GameObject trolley)
    {
        trolley.transform.position = this.transform.localPosition + new Vector3(0, 0, distance);
        trolley.transform.rotation = this.transform.localRotation;
        trolley.transform.parent = this.transform;
    }
}
