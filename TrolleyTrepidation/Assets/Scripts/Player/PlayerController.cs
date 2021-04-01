using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject spawnController;
    private SpawnController trolleySpawner;
    public GameObject item;
    public GameObject tempParent;
    public Transform guide;
    public bool carrying;
    public float distance = 1.3F;

    // Use this for initialization
    void Start()
    {
        trolleySpawner = spawnController.GetComponent<SpawnController>();
        item.GetComponent<Rigidbody>().useGravity = true;
    }
    // Update is called once per frame
    void Update()
    {
        foreach(GameObject trolley in trolleySpawner.arrayTrolleys)
        {
            if (Input.GetKeyDown(KeyCode.K) && (guide.transform.position - trolley.transform.position).sqrMagnitude < 2)
                {
                    pickup(trolley);
                    carrying = true;
                }
            
            
                if (Input.GetKeyDown(KeyCode.J))
                {
                    drop(trolley);
                    carrying = false;
                }
            
        }
        
    }
    void pickup(GameObject trolley)
    {        
        trolley.transform.position = guide.transform.localPosition + guide.transform.rotation * new Vector3 (0,0,distance);
        trolley.transform.rotation = guide.transform.localRotation;
        trolley.transform.parent = tempParent.transform;
    }
    void drop(GameObject trolley)
    {        
        trolley.transform.parent = null;
        trolley.transform.position = guide.transform.position;
    }
}
