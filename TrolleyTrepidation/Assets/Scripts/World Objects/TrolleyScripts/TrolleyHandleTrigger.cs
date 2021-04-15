using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrolleyHandleTrigger : MonoBehaviour
{
   

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Robot")
        {
            transform.parent = other.transform;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < beginSize - 1; i++)
        {

            AddBodyPart();

        }
    }

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
    

    // Update is called once per frame
    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Q))
            AddBodyPart();
    }

    public void Move()
    {

        float curspeed = speed;

        if (Input.GetKey(KeyCode.W))
            curspeed *= 2;

        bodyParts[0].Translate(bodyParts[0].forward * curspeed * Time.smoothDeltaTime, Space.World);

        if (Input.GetAxis("Horizontal") != 0)
            bodyParts[0].Rotate(Vector3.up * rotationSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));

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
 /*       if (bodyParts.Count == 1)
        {
            Transform newpart = (Instantiate(bodyprefabs, transform.position, transform.rotation) as GameObject).transform;
            newpart.SetParent(transform);

            bodyParts.Add(newpart);
        }
        else */
        {
            Transform newpart = (Instantiate(bodyprefabs, bodyParts[bodyParts.Count - 1].position, bodyParts[bodyParts.Count - 1].rotation) as GameObject).transform;
            newpart.SetParent(transform);
            bodyParts.Add(newpart);
        }
        
    }
}
