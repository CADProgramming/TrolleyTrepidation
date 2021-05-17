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
        //Temporary code to test dropping trolleys, may be used in later development
        if(Input.GetKeyDown(KeyCode.Q))
        {
            bodyParts[bodyParts.Count - 1].parent = null;
            bodyParts.RemoveAt(bodyParts.Count - 1);
            player.GetComponent<BoxCollider>().center = player.GetComponent<BoxCollider>().center + Vector3.back;
        }
        //code for turning left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //checks if carrying trolleys or if the speed is below a threshold
            if((bodyParts.Count <= 0) || speed < 3f)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 0.2f, 0);
            }
            //rotates the trolleys and robots around the middle point of the amount of trolleys carried
            else
            {
                transform.RotateAround(bodyParts[bodyParts.Count / 2].position, Vector3.down, 0.2f);
            }
            //transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.left);

        }
        //code for turning right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //checks if carrying trolleys or if the speed is below a threshold
            if((bodyParts.Count <= 0) || speed < 3f)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 0.2f, 0);
            }
            //roates the trolleys and robots around the middle point of the amount of trolleys carried
            else
            {
                transform.RotateAround(bodyParts[bodyParts.Count / 2].position, Vector3.up, 0.2f);
            }
            
            //transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.right);
        }
        //applies forward velocity to the player
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.forward);            
        }
        //applies backwards velocity to the player
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.GetComponent<Rigidbody>().velocity = transform.rotation * (MOVE_SPEED * Vector3.back);
        }
        //moves the trolleys with the player
        moveTrolley();
        //used to calculate the speed the player is moving
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
        //adds the trolley transform to the array
        bodyParts.Add(trolley.transform);
        //sets the trolleys parent to the players
        trolley.transform.parent = transform;
        // updates the trolleys postion to infront of the player
        trolley.transform.localPosition = new Vector3(0, 0, distance + bodyParts.Count - 1);
        trolley.transform.localRotation = Quaternion.identity;
        //update the collision box to allow for picking up another trolley
        player.GetComponent<BoxCollider>().center = player.GetComponent<BoxCollider>().center + Vector3.forward;
        //TEMP CODE, MIGHT BE REMOVED
        trolley.GetComponent<Rigidbody>().useGravity = false;
        
    }
}
