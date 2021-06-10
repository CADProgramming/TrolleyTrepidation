using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    private const float FORWARD_SPEED = 3.5f;
    private const float REVERSE_SPEED = 1.0f;
    private const float REVERSE_DISTANCE = 3.0f;
    private const float ROTATION_SPEED = 4.0f;
    private const float PARK_ROTATION_SPEED = 24.5f;
    private const float VECTOR_MATCH = 0.98f;
    private const float PARK_VECTOR_MATCH = 0.98f;
    private const float NODE_TOLERANCE = 0.3f;
    private const float MIN_PARK_DIST = 13.0f;
    private const float MIN_START_DIST = 0.3f;
    private const float SPEED = 4.0f;

    private const float STOPPING_DISTANCE = 11.0f;
    private const float STOP_BACK_DISTANCE = 5.0f;
    private const float STOPPING_ANGLE = 100.0f;

    public bool isWaiting;
    public Transform goal;
    public GameObject path;
    public ParkNodeController park;
    public CarLeavingState leavingState;

    private float degreesOfRotation;
    private float distanceReversed;
    private bool atNextNode;
    private bool isParking;
    private Vector3 forwardVector;
    private Vector3 reverseVector;

    private GameObject nextNode;

    void Start()
    {
        degreesOfRotation = 0;
        distanceReversed = 0;
        atNextNode = true;
        isParking = false;
        isWaiting = false;
        nextNode = null;

        forwardVector = FORWARD_SPEED * Vector3.forward;
        reverseVector = REVERSE_SPEED * Vector3.back;
    }

    private void Update()
    {
        if (leavingState != CarLeavingState.STOPPED)
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
        Ray rayBehind = new Ray(transform.position + Vector3.up, transform.rotation * Vector3.back * STOP_BACK_DISTANCE);
        Debug.DrawRay(transform.position + Vector3.up, transform.rotation * Vector3.back * STOP_BACK_DISTANCE, Color.red, 0.5f);

        bool allOtherCarsWaiting = true;
        isWaiting = CheckGiveway(rayBehind, ref allOtherCarsWaiting, STOP_BACK_DISTANCE);

        if (!isWaiting)
        {
            // Reverse a fixed distance to a location
            if (distanceReversed < REVERSE_DISTANCE)
            {
                transform.position += transform.rotation * reverseVector * Time.deltaTime;
                distanceReversed += reverseVector.magnitude * Time.deltaTime;
            }
            else
            {
                leavingState = CarLeavingState.REVERSE_TURN;
            }
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

    private bool CheckPathIsBlocked(Ray checkRay, float distanceToNode)
    {
        RaycastHit collision;

        if (Physics.Raycast(checkRay, out collision))
        {
            CarMovement car;
            bool isCar = collision.transform.gameObject.TryGetComponent(out car);

            if (isCar && collision.distance <= distanceToNode &&
                car.leavingState == CarLeavingState.STOPPED)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool CheckGiveway(Ray checkRay, ref bool allOtherWaiting, float stoppingDistance)
    {
        RaycastHit collision;

        if (Physics.Raycast(checkRay, out collision))
        {
            CarMovement car;
            bool isCar = collision.transform.gameObject.TryGetComponent(out car);

            if (isCar && car != this)
            {
                float angleBetweenCars = Quaternion.Angle(collision.transform.rotation, transform.rotation);
                allOtherWaiting = car.isWaiting && allOtherWaiting;

                if ((angleBetweenCars < STOPPING_ANGLE || (angleBetweenCars >= STOPPING_ANGLE && !allOtherWaiting)) && 
                    collision.distance < stoppingDistance && 
                    car.leavingState != CarLeavingState.STOPPED)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        else
        {
            return false;
        }
    }

    private void FollowPathOut()
    {
        Ray rayForward = new Ray(transform.position + Vector3.up, transform.rotation * Vector3.forward * STOPPING_DISTANCE);
        Ray rayLeft = new Ray(transform.position + Vector3.up, transform.rotation * Quaternion.Euler(0, -30.0f, 0) * Vector3.forward * STOPPING_DISTANCE);
        Debug.DrawRay(transform.position + Vector3.up, transform.rotation * Vector3.forward * STOPPING_DISTANCE, Color.red, 0.5f);
        Debug.DrawRay(transform.position + Vector3.up, transform.rotation * Quaternion.Euler(0, -20.0f, 0) * Vector3.forward * STOPPING_DISTANCE, Color.red, 0.5f);

        bool allOtherCarsWaiting = true;
        isWaiting = CheckGiveway(rayForward, ref allOtherCarsWaiting, STOPPING_DISTANCE) || 
            CheckGiveway(rayLeft, ref allOtherCarsWaiting, STOPPING_DISTANCE);

        if (!isWaiting)
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
    }


    IEnumerator MoveToNode(Vector3 nextLocation)
    {
        Vector3 forwardVector = SPEED * Vector3.forward;
        Quaternion rotationBetweenPoints;

        while (Mathf.Abs(Vector3.Distance(nextLocation, transform.position)) > NODE_TOLERANCE && !isParking)
        {
            if (!isWaiting)
            {
                rotationBetweenPoints = Quaternion.LookRotation(nextLocation - transform.position);
                RotateTowardsNextLocation(rotationBetweenPoints);
                MoveTowardsNextLocation(forwardVector);
            }
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
        float bestMinDistance = 0;
        GameObject closestNode = null;
        GameObject bestNode = null;

        foreach (Transform node in path.transform)
        {
            float distanceToNode = Vector3.Distance(node.position, transform.position);
            float similarity = Vector3.Dot((node.position - transform.position).normalized, (transform.rotation * Vector3.forward).normalized);
            Ray rayForward = new Ray(transform.position + Vector3.up, node.position - transform.position);
            Debug.DrawRay(transform.position + Vector3.up, node.position - transform.position, Color.red, 0.1f);

            if (!CheckPathIsBlocked(rayForward, distanceToNode))
            {
                if ((!bestNode || distanceToNode < bestMinDistance) &&
                    similarity > VECTOR_MATCH)
                {
                    bestMinDistance = distanceToNode;
                    bestNode = node.gameObject;
                }
                if (!closestNode || distanceToNode < minDistance)
                {
                    minDistance = distanceToNode;
                    closestNode = node.gameObject;
                }
            }
        }


        if (bestNode == null)
        {
            float distanceToNode = Vector3.Distance(transform.position, closestNode.transform.position);
            if (distanceToNode < MIN_START_DIST)
            {
                transform.position = closestNode.transform.position;
            }
            nextNode = closestNode;
            return closestNode.transform;
        }
        else
        {
            float distanceToNode = Vector3.Distance(transform.position, bestNode.transform.position);
            if (distanceToNode < MIN_START_DIST)
            {
                transform.position = bestNode.transform.position;
            }
            nextNode = bestNode;
            return bestNode.transform;
        }
    }

    private bool IsAtNextNode()
    {
        float distanceToNextNode = Mathf.Abs(Vector3.Distance(transform.position, nextNode.transform.position));
        return distanceToNextNode < NODE_TOLERANCE;
    }

    public void DeSpawn()
    {
        GameObject carSpawner = GameObject.Find("CarSpawner");
        CarSpawner spawner = carSpawner.GetComponent<CarSpawner>();
        spawner.CarHasLeft(gameObject);
        park.isEmpty = true;
        Destroy(gameObject);
    }
}
