using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HashSet<GameObject> colliders = new HashSet<GameObject>();
    public HashSet<GameObject> GetColliders() { return colliders; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        colliders.Add(other.gameObject);
    }
}
