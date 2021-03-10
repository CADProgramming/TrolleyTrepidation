using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float MAX_SPEED = 6.0f;   // Player top speed
    private const float ROTATION = 120.0f;
    private const float ACCEL = 0.15f;
    private const float DEACCEL = 0.02f;

    public float speed;

    private Animator playerAnimator;
    private Rigidbody playerRigidBody;

    private bool isPushingTrolley;  // Is the player attached to a trolley
    private bool hasCrashed;

    // Start is called before the first frame update
    void Start()
    {
        isPushingTrolley = false;
        hasCrashed = false;

        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePosition();
    }

    // Updates the player's position
    private void UpdatePosition()
    {
        if (isPushingTrolley)
        {
            MoveWithTrolley();
        }
        else
        {
            MovePlayer();
        }
    }

    // Moves the player and the trolley/trolley chain
    private void MoveWithTrolley()
    {

    }

    // Moves the player without a trolley
    private void MovePlayer()
    {
        float motor = ACCEL * Input.GetAxis("Vertical");
        float steering = ROTATION * Input.GetAxis("Horizontal");
        
        if (motor != 0)
        {
            speed += motor;
        }
        else if ((speed > 0 && speed < DEACCEL) ||
            (speed < 0 && speed > -DEACCEL))
        {
            speed = 0;
        }
        else if (speed < 0)
        {
            speed += DEACCEL;
        }
        else if (speed > 0)
        {
            speed -= DEACCEL;
        }

        if (speed > MAX_SPEED)
        {
            speed = MAX_SPEED;
        }
        else if (speed < -MAX_SPEED)
        {
            speed = -MAX_SPEED;
        }

        if (!hasCrashed)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        transform.Rotate(Vector3.up * steering * Time.deltaTime);

        if (playerAnimator)
        {
            bool isForwardPressed = motor > 0 || motor < 0;
            bool isMoving = speed > 0.1f || speed < -0.1f;

            playerAnimator.SetBool("isForwardPressed", isForwardPressed);
            playerAnimator.SetBool("isMoving", isMoving);
        }

        AntiRoll();
    }

    private void AntiRoll()
    {
        if (transform.rotation.eulerAngles.x > 0)
        {
            transform.rotation = Quaternion.Euler(Quaternion.identity.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        if (transform.rotation.eulerAngles.z > 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Quaternion.identity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "FixedObstacle")
        {
            hasCrashed = true;
            speed = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        hasCrashed = false;
    }
}
