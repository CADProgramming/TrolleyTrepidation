using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    private const float FORWARD_SPEED = 2.0f;
    private const float REVERSE_SPEED = 1.0f;
    private const float REVERSE_DISTANCE = 3.0f;
    private const float ROTATION_SPEED = 20.0f;

    public Transform goal;

    private float degreesOfRotation;
    private Vector3 forwardVector;
    private Vector3 reverseVector;
    private Vector3 reverseLocation;
    private CarState movementState;

    NavMeshAgent agent;

    void Start()
    {
        degreesOfRotation = 0;

        movementState = CarState.REVERSING;
        reverseLocation = transform.position + REVERSE_DISTANCE * (transform.rotation * Vector3.back);
        forwardVector = FORWARD_SPEED * Vector3.forward;
        reverseVector = REVERSE_SPEED * Vector3.back;
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    private void Update()
    {
        switch (movementState)
        {
            // Car reversing out of park
            case CarState.REVERSING:
                ReverseOutOfPark();
                break;
            // Car reversing and turning out of park
            case CarState.REVERSE_TURN:
                ReverseAndTurnOutOfPark();
                break;
            // Car moving forward and turning after exiting park
            case CarState.FORWARD_TURN:
                AlignWithPath();
                break;
        }
    }

    // Car moves backward out of a car park
    private void ReverseOutOfPark()
    {
        // Reverse a fixed distance to a location
        if (transform.position.magnitude - reverseLocation.magnitude > 0.1f)
        {
            transform.position += transform.rotation * reverseVector * Time.deltaTime;
        }
        else
        {
            movementState = CarState.REVERSE_TURN;
        }
    }

    // Car reverses and turns to pull out of a car park
    private void ReverseAndTurnOutOfPark()
    {
        // Gradually reverse and turn until a degree of rotation is achieved
        if (degreesOfRotation < 60)
        {
            transform.Rotate(Vector3.up, ROTATION_SPEED * Time.deltaTime);
            transform.position += transform.rotation * reverseVector * Time.deltaTime;
            degreesOfRotation += ROTATION_SPEED * Time.deltaTime;
        }
        else
        {
            movementState = CarState.FORWARD_TURN;
        }
    }

    // After leaving the park, car turns onto path
    private void AlignWithPath()
    {
        // Gradually move forward and turn until a degree of rotation is achieved
        if (degreesOfRotation < 90)
        {
            transform.Rotate(Vector3.up, ROTATION_SPEED * Time.deltaTime);
            transform.position += transform.rotation * forwardVector * Time.deltaTime;
            degreesOfRotation += ROTATION_SPEED * Time.deltaTime;
        }
        else
        {
            movementState = CarState.AUTO;
            agent.enabled = true;
            agent.destination = goal.position;
        }
    }
}
