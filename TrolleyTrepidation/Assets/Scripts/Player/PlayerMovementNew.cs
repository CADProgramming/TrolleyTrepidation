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
        if (other.name.Contains("Trolley"))
        {
            Destroy(other.gameObject);
            AddBodyPart();
        }        
    }

    private void MovePlayer()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerBody.rotation = Quaternion.Euler(0, -90, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerBody.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            playerBody.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            playerBody.rotation = Quaternion.Euler(0, 180, 0);
        }

        playerBody.velocity = transform.rotation * (MOVE_SPEED * Vector3.forward);
        moveTrolley();
    }
    private void moveTrolley()
    {
        float curspeed = speed;
        for (int i = 1; i < bodyParts.Count; i++)
        {

            curBodyPart = bodyParts[i];
            PrevBodyPart = bodyParts[i - 1];

            dis = Vector3.Distance(PrevBodyPart.position, curBodyPart.position);

            Vector3 newpos = PrevBodyPart.position;

            newpos.y = bodyParts[0].position.y;

            float T = Time.deltaTime * dis / minDistance * curspeed;

            if (T > 0.5f)
                T = 0.5f;
            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
            curBodyPart.rotation = Quaternion.Slerp(curBodyPart.rotation, PrevBodyPart.rotation, T);

        }
    }
    public void AddBodyPart()
    {
        if (bodyParts.Count == 1)
        {
            Transform newpart = (Instantiate(bodyprefabs, transform.position., transform.rotation) as GameObject).transform;
            newpart.SetParent(transform);

            bodyParts.Add(newpart);
        }
        else 
        {
            Transform newpart = (Instantiate(bodyprefabs, bodyParts[bodyParts.Count - 1].position, bodyParts[bodyParts.Count - 1].rotation) as GameObject).transform;
            newpart.SetParent(transform);
            bodyParts.Add(newpart);
        }

    }


}
