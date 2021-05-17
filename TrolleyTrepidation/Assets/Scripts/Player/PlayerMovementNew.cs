using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    private const float MOVE_SPEED = 5.0f;
    
    Vector3 movementVector;
    private Rigidbody playerBody;
    public GameObject player;
    public List<Transform> bodyParts = new List<Transform>();

    
    public float distance = 3.0F;
    public float speed;
    private Vector3 oldPosition;
    public int beginSize;

    public float rotationSpeed = 50;

    public GameObject bodyprefabs;


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
        if(Input.GetKeyDown(KeyCode.Q))
        {
            bodyParts[bodyParts.Count - 1].parent = null;
            bodyParts.RemoveAt(bodyParts.Count - 1);
            player.GetComponent<BoxCollider>().center = player.GetComponent<BoxCollider>().center + Vector3.back;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if((bodyParts.Count <= 0) || speed < 3f)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 0.2f, 0);
            }
            else
            {
                transform.RotateAround(bodyParts[bodyParts.Count / 2].position, Vector3.down, 0.2f);
            }
            //transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.left);

        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if((bodyParts.Count <= 0) || speed < 3f)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 0.2f, 0);
            }
            else
            {
                transform.RotateAround(bodyParts[bodyParts.Count / 2].position, Vector3.up, 0.2f);
            }
            
            //transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.right);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.forward);            
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.back);
        }
        moveTrolley();
        
        speed = Vector3.Distance(oldPosition, transform.position) * 100f;
        Debug.Log(speed);
        oldPosition = transform.position;
    }
    private void moveTrolley()
    {
        for(int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].transform.localPosition = new Vector3(0, 0, distance + i);
            bodyParts[i].transform.GetComponent<Rigidbody>().velocity = transform.GetComponent<Rigidbody>().velocity;
        }
    }
    public void AddBodyPart(GameObject trolley)
    {
        //Transform newpart = (Instantiate(bodyprefabs, bodyParts[bodyParts.Count - 1].transform.position + new Vector3(0,0,distance+bodyParts.Count - 1), bodyParts[bodyParts.Count -1 ].rotation) as GameObject).transform;
        bodyParts.Add(trolley.transform);
        trolley.transform.parent = transform;
        trolley.transform.localPosition = new Vector3(0, 0, distance + bodyParts.Count - 1);
        trolley.transform.localRotation = Quaternion.identity;
        player.GetComponent<BoxCollider>().center = player.GetComponent<BoxCollider>().center + Vector3.forward;
        trolley.GetComponent<Rigidbody>().useGravity = false;
        
    }
}
