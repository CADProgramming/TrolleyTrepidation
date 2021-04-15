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
    public List<GameObject> carrying;
    public float distance = 1.3F;

    // Use this for initialization
    void Start()
    {
        trolleySpawner = spawnController.GetComponent<SpawnController>();
        item.GetComponent<Rigidbody>().useGravity = true;
        carrying = new List<GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        foreach(GameObject trolley in trolleySpawner.arrayTrolleys)
        {
            if (Input.GetKeyDown(KeyCode.K) && (guide.transform.position - trolley.transform.position).sqrMagnitude < 2)
                {
                    pickup(trolley);
                    
                }         
                
            
        }        
        if (Input.GetKeyDown(KeyCode.J))
        {
            drop(carrying[carrying.Count - 1]);
        }
        
        

    }
    void pickup(GameObject trolley)
    {
        carrying.Add(trolley);
        trolley.transform.position = guide.transform.localPosition + guide.transform.rotation * new Vector3 (0,0,distance+carrying.Count - 1);
        trolley.transform.rotation = guide.transform.localRotation;
        trolley.transform.parent = guide.transform;
    }
    void drop(GameObject trolley)
    {
        carrying.Remove(trolley);
        trolley.transform.parent = null;
        trolley.transform.position = guide.transform.position;
    }
}
