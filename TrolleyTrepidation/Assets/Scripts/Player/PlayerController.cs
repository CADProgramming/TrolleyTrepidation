using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject item;
    public GameObject tempParent;
    public Transform guide;
    public bool carrying;
    

    // Use this for initialization
    void Start()
    {
        item.GetComponent<Rigidbody>().useGravity = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (carrying == false)
        {
            if (Input.GetKeyDown(KeyCode.K) && (guide.transform.position - item.transform.position).sqrMagnitude < 2)
            {
                pickup();
                carrying = true;
            }
        }
        else if (carrying == true)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                drop();
                carrying = false;
            }
        }
    }
    void pickup()
    {
        item.GetComponent<Rigidbody>().useGravity = false;
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.transform.position = guide.transform.position;
        item.transform.rotation = guide.transform.rotation;
        item.transform.parent = tempParent.transform;
    }
    void drop()
    {
        item.GetComponent<Rigidbody>().useGravity = true;
        item.GetComponent<Rigidbody>().isKinematic = false;
        item.transform.parent = null;
        item.transform.position = guide.transform.position;
    }
}
