using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrolleyHandleTrigger : MonoBehaviour
{
    public GameObject robot;
    private GameObject trolleyBasket;
    private Rigidbody trolleyBody;

    // Start is called before the first frame update
    void Start()
    {
        trolleyBasket = transform.root.gameObject;
        trolleyBody = trolleyBasket.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.root.gameObject.name == robot.name &&
            trolleyBasket.transform.parent == null)
        {
            PlayerController robotController = other.GetComponentInParent<PlayerController>();

            if (robotController && robotController.GetColliders().Contains(gameObject))
            {
                transform.root.transform.SetParent(other.transform.root);
                Destroy(trolleyBody.GetComponent<Rigidbody>());
            }
        }
    }
}
