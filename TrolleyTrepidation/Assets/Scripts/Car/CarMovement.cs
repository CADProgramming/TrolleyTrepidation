using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    private const float FORWARD_SPEED = 2.0f;
    private const float REVERSE_SPEED = 1.0f;
    private const float REVERSE_DISTANCE = 2.5f;
    private const float ROTATION_SPEED = 20.0f;
    private const float VECTOR_MATCH = 0.8f;
    private const float NODE_TOLERANCE = 0.2f;

    public Transform goal;
    public GameObject path;

    private float degreesOfRotation;
    private bool atNextNode;
    private Vector3 forwardVector;
    private Vector3 reverseVector;
    private Vector3 reverseLocation;
    private CarState movementState;

    private GameObject nextNode;
    private NavMeshAgent agent;

    void Start()
    {
        degreesOfRotation = 0;
        atNextNode = true;
        nextNode = null;

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
            case CarState.AUTO:
                FollowPath();
                break;
        }
    }

    // Car moves backward out of a car park
    private void ReverseOutOfPark()
    {
        // Reverse a fixed distance to a location
        if (Mathf.Abs(transform.position.magnitude - reverseLocation.magnitude) > 0.1f)
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
        }
    }

    private void FollowPath()
    {
        if (atNextNode)
        {
            Transform nextLocation;

            if (nextNode)
            {
                nextLocation = FindNextNode();
            }
            else
            {
                nextLocation = FindNearestNode();
            }

            agent.destination = nextLocation.position;
        }

        atNextNode = IsAtNextNode();
    }

    private Transform FindNextNode()
    {
        NodeController nodeController = nextNode.GetComponent<NodeController>();
        GameObject newNode = nodeController.nextNodes[0];
        float minDistanceToGoal = Vector3.Distance(newNode.transform.position, goal.position);

        foreach (GameObject node in nodeController.nextNodes)
        {
            float distanceToGoal = Vector3.Distance(node.transform.position, goal.position);

            if (distanceToGoal < minDistanceToGoal)
            {
                minDistanceToGoal = distanceToGoal;
                newNode = node;
            }
        }

        nextNode = newNode;

        return newNode.transform;
    }

    private Transform FindNearestNode()
    {
        float minDistance = 0;
        GameObject closestNode = null;

        foreach (Transform node in path.transform)
        {
            float distanceToNode = Vector3.Distance(node.position, transform.position);
            float similarity = Vector3.Dot((node.position - transform.position).normalized, (transform.rotation * Vector3.forward).normalized);
            float test = Vector3.Dot(Vector3.forward.normalized, Vector3.back.normalized);

            if ((!closestNode || distanceToNode < minDistance) &&
                similarity > VECTOR_MATCH)
            {
                minDistance = distanceToNode;
                closestNode = node.gameObject;
            }
        }

        nextNode = closestNode;

        return closestNode.transform;
    }

    private bool IsAtNextNode()
    {
        float distanceToNextNode = Mathf.Abs(Vector3.Distance(transform.position, nextNode.transform.position));
        return distanceToNextNode < NODE_TOLERANCE;
    }
}
