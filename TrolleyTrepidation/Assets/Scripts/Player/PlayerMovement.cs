using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float MAX_SPEED = 5.0f;   // Player top speed
    private const float MAX_MOTOR_TORQUE = 400;
    private const float MAX_STEERING_ANGLE = 30;

    public WheelCollider leftWheel;             // Forward motion left wheel
    public WheelCollider rightWheel;            // Forward motion right wheel
    public WheelCollider leftSteeringWheel;     // Invisible left steering wheel
    public WheelCollider rightSteeringWheel;    // Invisible right steering wheel

    private bool isPushingTrolley;  // Is the player attached to a trolley

    // Start is called before the first frame update
    void Start()
    {
        isPushingTrolley = false;
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

        leftWheel.motorTorque = motor;
        rightWheel.motorTorque = motor;
        leftSteeringWheel.steerAngle = steering;
        rightSteeringWheel.steerAngle = steering;
    }
}
