using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float MAX_SPEED = 40.0f;   // Player top speed
    private const float MAX_MOTOR_TORQUE = 800;
    private const float MAX_STEERING_ANGLE = 30;

    public WheelCollider leftWheel;             // Forward motion left wheel
    public WheelCollider rightWheel;            // Forward motion right wheel
    public WheelCollider leftSteeringWheel;     // Invisible left steering wheel
    public WheelCollider rightSteeringWheel;    // Invisible right steering wheel

    public float speed;

    private Animator playerAnimator;
    private Rigidbody playerRigidBody;

    private bool isPushingTrolley;  // Is the player attached to a trolley

    // Start is called before the first frame update
    void Start()
    {
        isPushingTrolley = false;

        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
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
        float motor = MAX_MOTOR_TORQUE * Input.GetAxis("Vertical");
        float steering = MAX_STEERING_ANGLE * Input.GetAxis("Horizontal");
        float currentSpeed = playerRigidBody.velocity.sqrMagnitude;

        if (currentSpeed < MAX_SPEED)
        {
            leftWheel.motorTorque = motor;
            rightWheel.motorTorque = motor;
        }
        else
        {
            leftWheel.motorTorque = 0;
            rightWheel.motorTorque = 0;
        }

        leftSteeringWheel.steerAngle = steering;
        rightSteeringWheel.steerAngle = steering;

        speed = currentSpeed;

        if (playerAnimator)
        {
            bool isForwardPressed = motor > 0 || motor < 0;
            bool isMoving = currentSpeed > 0.001f || currentSpeed < -0.001f;

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
}
