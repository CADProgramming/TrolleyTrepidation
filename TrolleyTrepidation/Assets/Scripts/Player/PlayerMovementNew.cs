using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    private const float MOVE_SPEED = 5.0f;
    
    Vector3 movementVector;
    private Rigidbody playerBody;
    public List<Transform> bodyParts = new List<Transform>();

    public float minDistance = 0.25f;
    public float distance = 1.3F;

    public int beginSize;

    public float speed = 0;
    public float rotationSpeed = 50;

    public GameObject bodyprefabs;

    private float dis;
    private Transform curBodyPart;
    private Transform PrevBodyPart;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    public void OnTriggerEnter(Collider other)
    {
        if(bodyParts.Contains(other.transform) == false)
        {
            if (other.name.Contains("Trolley"))
            {                
                AddBodyPart(other.gameObject);
            }
        }              
    }

    private void MovePlayer()
    {
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 1, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 1, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.forward);
        moveTrolley();
    }
    private void moveTrolley()
    {
        float curspeed = speed;

        for(int i = 1; i < (bodyParts.Count); i++)
        {
            bodyParts[i].GetComponent<Rigidbody>().velocity = (MOVE_SPEED * Vector3.forward);
        }
    }
    public void AddBodyPart(GameObject trolley)
    {
        //Transform newpart = (Instantiate(bodyprefabs, bodyParts[bodyParts.Count - 1].transform.position + new Vector3(0,0,distance+bodyParts.Count - 1), bodyParts[bodyParts.Count -1 ].rotation) as GameObject).transform;
        bodyParts.Add(trolley.transform);
        trolley.transform.parent = transform;
        trolley.transform.localPosition = new Vector3(0, 0, distance + bodyParts.Count - 1);
        trolley.transform.localRotation = Quaternion.identity;
        
    }
}
