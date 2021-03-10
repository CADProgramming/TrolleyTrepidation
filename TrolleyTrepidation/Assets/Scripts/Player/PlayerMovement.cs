using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float MAX_SPEED = 6.0f;   // Player top speed
    private const float ROTATION = 120.0f;  // Player rotation deg
    private const float ACCEL = 0.15f;      // Player acceleration
    private const float DEACCEL = 0.02f;    // Player coasting friction

    public float speed;     // Player movement speed

    private Animator playerAnimator;    // Player animations

    private bool isPushingTrolley;  // Is the player attached to a trolley
    private bool hasCrashed;        // Player has crashed into a fixed obstacle

    // Start is called before the first frame update
    void Start()
    {
        isPushingTrolley = false;
        hasCrashed = false;

        playerAnimator = GetComponent<Animator>();
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
            // Player movement with a trolley
            MoveWithTrolley();
        }
        else
        {
            // Player movement without a trolley
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
        // Calculate acceleration and steering based on inputs
        float motor = ACCEL * Input.GetAxis("Vertical");
        float steering = ROTATION * Input.GetAxis("Horizontal");

        // Player is moving forward or backwards        
        if (motor != 0)
        {
            speed += motor;
        }
        // Player is moving less than the rate of deacceleration
        else if ((speed > 0 && speed < DEACCEL) ||
            (speed < 0 && speed > -DEACCEL))
        {
            speed = 0;
        }
        // Player is rolling backwards
        else if (speed < 0)
        {
            speed += DEACCEL;
        }
        // Player is rolling forwards
        else if (speed > 0)
        {
            speed -= DEACCEL;
        }

        // Forward & backword speed limiter
        if (speed > MAX_SPEED)
        {
            speed = MAX_SPEED;
        }
        else if (speed < -MAX_SPEED)
        {
            speed = -MAX_SPEED;
        }

        // Only move if not crashed
        if (!hasCrashed)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        transform.Rotate(Vector3.up * steering * Time.deltaTime);

        // Update animation variables
        if (playerAnimator)
        {
            bool isForwardPressed = motor > 0 || motor < 0;     // Player is accelerating
            bool isMoving = speed > 0.1f || speed < -0.1f;      // Player is moving

            // Update variables
            playerAnimator.SetBool("isForwardPressed", isForwardPressed);
            playerAnimator.SetBool("isMoving", isMoving);
        }

        // Prevent rolling
        AntiRoll();
    }

    // Stops the player rotating on euler axis x or z
    private void AntiRoll()
    {
        if (transform.rotation.eulerAngles.x > 0)
        {
            // Set rotation to the same rotation but reset x
            transform.rotation = Quaternion.Euler(Quaternion.identity.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        if (transform.rotation.eulerAngles.z > 0)
        {
            // Set rotation to the same rotation but reset z
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Quaternion.identity.z);
        }
    }

    // When player enters into a collision
    private void OnCollisionEnter(Collision collision)
    {
        // Player hits fixed obstacle
        if (collision.gameObject.tag == "FixedObstacle")
        {
            // Stop moving
            hasCrashed = true;
            speed = 0;
        }
    }

    // When player exits a collision
    private void OnCollisionExit(Collision collision)
    {
        hasCrashed = false;
    }
}
