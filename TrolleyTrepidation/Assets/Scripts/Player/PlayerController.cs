using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HashSet<GameObject> colliders = new HashSet<GameObject>();
    public HashSet<GameObject> GetColliders() { return colliders; }

    private void OnTriggerExit(Collider other)
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        colliders.Add(other.gameObject);
       
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
