using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    private const float MOVE_SPEED = 5.0f;
    
    Vector3 movementVector;
    private Rigidbody playerBody;

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
    }
}
