using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrolleyHandleTrigger : MonoBehaviour
{

    public GameObject robot;
    private PlayerController script;
    


    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.name == robot.name)
        {
  //          if(gameObject.Refe)
 //           if (script.GetColliders().Contains(gameObject))
 //           {
                transform.SetParent(other.transform.root);
 //           }
        }
        
        
    }
    // Start is called before the first frame update
    void Start()
    {
        script = robot.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
