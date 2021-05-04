using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    private const float FORWARD_SPEED = 2.0f;
    private const float REVERSE_SPEED = 1.0f;
    private const float REVERSE_DISTANCE = 3.0f;
    private const float ROTATION_SPEED = 3.0f;
    private const float PARK_ROTATION_SPEED = 24.5f;
    private const float VECTOR_MATCH = 0.8f;
    private const float PARK_VECTOR_MATCH = 0.9f;
    private const float NODE_TOLERANCE = 0.2f;
    private const float MIN_PARK_DIST = 13.0f;
    private const float SPEED = 4.0f;


    public Transform goal;
    public GameObject path;
    public CarEnteringState enteringState;
    public CarLeavingState leavingState;

    private float degreesOfRotation;
    private bool atNextNode;
    private bool readyToPark;
    private bool isParking;
    private bool parkClose;
    private Vector3 forwardVector;
    private Vector3 reverseVector;
    private Vector3 reverseLocation;
    private Vector3 parkEntranceLocation;

    private GameObject nextNode;

    void Start()
    {
        degreesOfRotation = 0;
        atNextNode = true;
        readyToPark = false;
        parkClose = true;
        isParking = false;
        nextNode = null;

        reverseLocation = transform.position + REVERSE_DISTANCE * (transform.rotation * Vector3.back);
        forwardVector = FORWARD_SPEED * Vector3.forward;
        reverseVector = REVERSE_SPEED * Vector3.back;
    }

    private void Update()
    {
        if (enteringState != CarEnteringState.STOPPED)
        {
            switch (enteringState)
            {
                // Car navigating car park
                case CarEnteringState.AUTO:
                    FollowPathIn();
                    break;
                // Car moving forward and turning to enter park
                case CarEnteringState.FORWARD_TURN:
                    EnterPark();
                    break;
            }
        }
        else if (leavingState != CarLeavingState.STOPPED)
        {
            switch (leavingState)
            {
                // Car reversing out of park
                case CarLeavingState.REVERSING:
                    ReverseOutOfPark();
                    break;
                // Car reversing and turning out of park
                case CarLeavingState.REVERSE_TURN:
                    ReverseAndTurnOutOfPark();
                    break;
                // Car moving forward and turning after exiting park
                case CarLeavingState.FORWARD_TURN:
                    AlignWithPath();
                    break;
                case CarLeavingState.AUTO:
                    FollowPathOut();
                    break;
            }
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
            leavingState = CarLeavingState.REVERSE_TURN;
        }
    }

    // Car reverses and turns to pull out of a car park
    private void ReverseAndTurnOutOfPark()
    {
        // Gradually reverse and turn until a degree of rotation is achieved
        if (degreesOfRotation < 60)
        {
            transform.Rotate(Vector3.up, PARK_ROTATION_SPEED * Time.deltaTime);
            transform.position += transform.rotation * reverseVector * Time.deltaTime;
            degreesOfRotation += PARK_ROTATION_SPEED * Time.deltaTime;
        }
        else
        {
            leavingState = CarLeavingState.FORWARD_TURN;
        }
    }

    // After leaving the park, car turns onto path
    private void AlignWithPath()
    {
        // Gradually move forward and turn until a degree of rotation is achieved
        if (degreesOfRotation < 90)
        {
            transform.Rotate(Vector3.up, PARK_ROTATION_SPEED * Time.deltaTime);
            transform.position += transform.rotation * forwardVector * Time.deltaTime;
            degreesOfRotation += PARK_ROTATION_SPEED * Time.deltaTime;
        }
        else
        {
            leavingState = CarLeavingState.AUTO;
        }
    }

    private void EnterPark()
    {
        // Gradually move forward and turn until a degree of rotation is achieved
        if (degreesOfRotation < 90)
        {
            if (parkClose)
            {
                transform.Rotate(Vector3.up, -PARK_ROTATION_SPEED * Time.deltaTime);
                transform.position += transform.rotation * forwardVector * Time.deltaTime;
            }
            else
            {
                transform.Rotate(Vector3.up, PARK_ROTATION_SPEED * Time.deltaTime);
                transform.position += transform.rotation * (forwardVector * 1.65f) * Time.deltaTime;
            }
            degreesOfRotation += PARK_ROTATION_SPEED * Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(Vector3.Distance(transform.position, goal.position)) < NODE_TOLERANCE)
            {
                enteringState = CarEnteringState.STOPPED;
            }
        }
    }

    private void FollowPathOut()
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

            StartCoroutine(MoveToNode(nextLocation.position));
        }

        atNextNode = IsAtNextNode();
    }

    private bool CanReachGoalEntrance(Vector3 goalEntrance)
    {
        float similarity = Vector3.Dot((goalEntrance - transform.position).normalized, (transform.rotation * Vector3.forward).normalized);

        return Mathf.Abs(Vector3.Distance(transform.position, nextNode.transform.position)) >
            Mathf.Abs(Vector3.Distance(transform.position, goalEntrance)) &&
            similarity > PARK_VECTOR_MATCH;
    }    

    private void CheckCanPark()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, goal.transform.position)) < MIN_PARK_DIST && !isParking)
        {
            ParkNodeController parkInfo = goal.GetComponent<ParkNodeController>();

            bool canEnterClose = CanReachGoalEntrance(parkInfo.closeEntrance);
            bool canEnterFar = CanReachGoalEntrance(parkInfo.farEntrance);
            readyToPark = canEnterClose || canEnterFar;
            parkClose = canEnterClose &&
                Mathf.Abs(Vector3.Distance(transform.position, parkInfo.closeEntrance)) <
                Mathf.Abs(Vector3.Distance(transform.position, parkInfo.farEntrance));

            if (parkClose)
            {
                parkEntranceLocation = parkInfo.closeEntrance;
            }
            else if (canEnterFar)
            {
                parkEntranceLocation = parkInfo.farEntrance;
            }

            if (readyToPark && !isParking && (parkClose || canEnterFar))
            {
                isParking = true;
                StartCoroutine(MoveToPark(parkEntranceLocation));
            }
        }
    }

    private void FollowPathIn()
    {
        CheckCanPark();

        if (readyToPark && IsAtParkEntrance())
        {
            enteringState = CarEnteringState.FORWARD_TURN;
        }
        else
        {
            if (atNextNode)
            {
                Vector3 nextLocation;

                if (nextNode)
                {
                    nextLocation = FindNextNode().position;
                }
                else
                {
                    nextLocation = FindNearestNode().position;
                }
                StartCoroutine(MoveToNode(nextLocation));
            }

            atNextNode = IsAtNextNode();
        }
    }

    IEnumerator MoveToPark(Vector3 parkEntrance)
    {
        Vector3 forwardVector = SPEED * Vector3.forward;
        Quaternion rotationBetweenPoints;

        while (Mathf.Abs(Vector3.Distance(parkEntrance, transform.position)) > NODE_TOLERANCE)
        {
            rotationBetweenPoints = Quaternion.LookRotation(parkEntrance - transform.position);
            RotateTowardsNextLocation(rotationBetweenPoints);
            MoveTowardsNextLocation(forwardVector);
            yield return null;
        }

        yield break;
    }

    IEnumerator MoveToNode(Vector3 nextLocation)
    {
        Vector3 forwardVector = SPEED * Vector3.forward;
        Quaternion rotationBetweenPoints;

        while (Mathf.Abs(Vector3.Distance(nextLocation, transform.position)) > NODE_TOLERANCE && !isParking)
        {
            rotationBetweenPoints = Quaternion.LookRotation(nextLocation - transform.position);
            RotateTowardsNextLocation(rotationBetweenPoints);
            MoveTowardsNextLocation(forwardVector);
            yield return null;
        }

        yield break;
    }

    private void MoveTowardsNextLocation(Vector3 moveVector)
    {
        transform.position += transform.rotation * moveVector * Time.deltaTime;
    }

    private void RotateTowardsNextLocation(Quaternion rotationAmount)
    {
        float angleOfRotation = Quaternion.Angle(transform.rotation, rotationAmount);

        if (Mathf.Abs(angleOfRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationAmount, Time.deltaTime * ROTATION_SPEED);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
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

    private bool IsAtParkEntrance()
    {
        float distanceToNextNode = Mathf.Abs(Vector3.Distance(transform.position, parkEntranceLocation));
        return distanceToNextNode < NODE_TOLERANCE;
    }
}
