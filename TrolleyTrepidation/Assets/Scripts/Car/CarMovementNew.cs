using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementNew : MonoBehaviour
{
    private const float REVERSE_DISTANCE = 2.5f;
    private const float NODE_TOLERANCE = 0.2f;
    private const float VECTOR_MATCH = 0.8f;
    private const float SPEED = 1.0f;
    private const float EXIT_ROTATION1 = 60.0f;
    private const float EXIT_ROTATION2 = 30.0f;
    private const float TURN_SPEED = 25.0f;
    private const float ACCELERATION = 0.5f;

    public CarStates state;
    public GameObject path;
    public Transform goal;

    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        CarNavigation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CarNavigation()
    {
        switch (state)
        {
            case CarStates.ENTER:
                EnterCarPark();
                break;
            case CarStates.EXIT:
                ExitCarPark();
                break;
        }
    }

    private void EnterCarPark()
    {

    }

    private void ExitCarPark()
    {
        StartCoroutine(MoveOutOfPark());
    }

    //IEnumerator MoveIntoPark()
    //{

    //}

    // Car moves backward out of a car park
    private void ReverseOutOfPark(Vector3 reverseVector)
    {
        MoveStraight(reverseVector);
    }

    // Car reverses and turns to pull out of a car park
    private void MoveAndTurn(Quaternion endRotation, Vector3 reverseVector)
    {
        transform.rotation *= Quaternion.Euler(0, TURN_SPEED * Time.deltaTime, 0);
        MoveStraight(reverseVector);
    }

    private void MoveStraight(Vector3 moveVector)
    {
        transform.position += transform.rotation * moveVector * Time.deltaTime;
    }

    private Vector3 FindNearestNode()
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

        return closestNode.transform.position;
    }

    private Vector3 FindNextNode(GameObject nextNode)
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

        return newNode.transform.position;
    }

    private void MoveTowardsNextLocation(Vector3 moveVector, Vector3 nextLocation)
    {
        Quaternion rotationBetweenPoints = Quaternion.FromToRotation(transform.position, nextLocation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationBetweenPoints, TURN_SPEED * Time.deltaTime);
        MoveStraight(moveVector);
        
    }

    IEnumerator MoveOutOfPark()
    {
        //Reverse until point
        Vector3 reverseLocation = transform.position + REVERSE_DISTANCE * (transform.rotation * Vector3.back);
        Vector3 reverseVector;
        float speed = SPEED;

        while (Mathf.Abs(Vector3.Distance(reverseLocation, transform.position)) > NODE_TOLERANCE)
        {
            reverseVector = speed * Vector3.back;
            ReverseOutOfPark(reverseVector);
            yield return new WaitForEndOfFrame();
        }

        //Turn until degrees
        Quaternion endRotation = transform.rotation * Quaternion.Euler(0, EXIT_ROTATION1, 0);

        while (Quaternion.Angle(transform.rotation, endRotation) > 0.01f)
        {
            reverseVector = speed * Vector3.back;
            MoveAndTurn(endRotation, reverseVector);
            yield return new WaitForEndOfFrame();
        }

        while (speed > 0)
        {
            speed -= ACCELERATION * Time.deltaTime;
            reverseVector = speed * Vector3.back;
            MoveStraight(reverseVector);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
        Vector3 forwardVector;

        while (speed < SPEED)
        {
            speed += ACCELERATION * Time.deltaTime;
            forwardVector = speed * Vector3.forward;
            MoveStraight(forwardVector);
            yield return new WaitForEndOfFrame();
        }

        //Line up with path
        endRotation = transform.rotation * Quaternion.Euler(0, EXIT_ROTATION2, 0);
        forwardVector = speed * Vector3.forward;

        while (Quaternion.Angle(transform.rotation, endRotation) > 0.01f)
        {
            MoveAndTurn(endRotation, forwardVector);
            yield return new WaitForEndOfFrame();
        }

        GameObject nextNode = null;
        Vector3 nextLocation = FindNearestNode();

        while (Mathf.Abs(Vector3.Distance(goal.position, transform.position)) > NODE_TOLERANCE)
        {
            if (nextNode)
            {
                nextLocation = FindNextNode(nextNode);
            }

            StartCoroutine(MoveToNode(nextLocation));
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    IEnumerator MoveToNode(Vector3 nextLocation)
    {
        Vector3 forwardVector = SPEED * Vector3.forward;

        while (Mathf.Abs(Vector3.Distance(goal.position, transform.position)) > NODE_TOLERANCE)
        {
            MoveTowardsNextLocation(forwardVector, nextLocation);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
