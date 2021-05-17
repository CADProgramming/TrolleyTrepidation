using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private const int MIN_CARS = 20;
    private const int MAX_CARS = 30;
    private const int MIN_TIME_OUT = 15;
    private const int MAX_TIME_OUT = 30;
    private const int MIN_TIME_IN = 20;
    private const int MAX_TIME_IN = 35;

    public GameObject[] carPrefabs;
    public GameObject spawnNodes;
    public GameObject parkNodes;
    public GameObject[] exitNodes;
    public List<GameObject> cars;

    // Start is called before the first frame update
    void Start()
    {
        cars = new List<GameObject>();

        PopulateCarPark();
        InvokeRepeating("LeaveCarPark", Random.Range(MIN_TIME_OUT, MAX_TIME_OUT), Random.Range(MIN_TIME_OUT, MAX_TIME_OUT));
        //InvokeRepeating("EnterCarPark", Random.Range(MIN_TIME_IN, MAX_TIME_IN), Random.Range(MIN_TIME_IN, MAX_TIME_IN));
    }

    private void PopulateCarPark()
    {
        int carCount = Random.Range(MIN_CARS, MAX_CARS);

        for (int c = 0; c < carCount; c++)
        {
            SpawnParkedCar();
        }
        
    }

    private void SpawnParkedCar()
    {
        int spawnPointIndex;
        bool spaceFree;
        Transform spawnPoint;
        ParkNodeController parkingSpace;

        do
        {
            spawnPointIndex = Random.Range(0, parkNodes.transform.childCount);
            spawnPoint = parkNodes.transform.GetChild(spawnPointIndex);
            parkingSpace = spawnPoint.GetComponent<ParkNodeController>();
            spaceFree = parkingSpace.isEmpty;

        } while (!spaceFree);

        parkingSpace.isEmpty = false;

        GameObject carSpawn = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPoint);
        carSpawn.transform.parent = transform;
        CarMovement carMovement = carSpawn.GetComponent<CarMovement>();
        carMovement.enteringState = CarEnteringState.STOPPED;
        carMovement.leavingState = CarLeavingState.STOPPED;
        cars.Add(carSpawn);
    }

    private void LeaveCarPark()
    {
        GameObject carToLeave;
        CarMovement carMovement;
        bool carIsParked;
        int parkedCount = 0;

        foreach (GameObject car in cars)
        {
            carMovement = car.GetComponent<CarMovement>();
            carIsParked = carMovement.enteringState == CarEnteringState.STOPPED &&
                carMovement.leavingState == CarLeavingState.STOPPED;
            if (carIsParked)
            {
                parkedCount++;
            }
        }

        if (parkedCount > 0)
        {
            do
            {
                carToLeave = cars[Random.Range(0, cars.Count)];
                carMovement = carToLeave.GetComponent<CarMovement>();
                carIsParked = carMovement.enteringState == CarEnteringState.STOPPED &&
                    carMovement.leavingState == CarLeavingState.STOPPED;

            } while (!carIsParked);

            carMovement.goal = exitNodes[Random.Range(0, exitNodes.Length)].transform;
            carMovement.leavingState = CarLeavingState.REVERSING;
        }
    }

    private void EnterCarPark()
    {
        GameObject carToEnter = Instantiate(
            carPrefabs[Random.Range(0, carPrefabs.Length)], 
            spawnNodes.transform.GetChild(Random.Range(0, spawnNodes.transform.childCount))
        );
        CarMovement carMovement;
        GameObject park;
        carMovement = carToEnter.GetComponent<CarMovement>();
        bool parkIsFull;

        do
        {
            park = parkNodes.transform.GetChild(Random.Range(0, parkNodes.transform.childCount)).gameObject;
            ParkNodeController parkInfo = park.GetComponent<ParkNodeController>();

            parkIsFull = !parkInfo.isEmpty;

        } while (parkIsFull);

        carMovement.goal = park.transform;
        carMovement.enteringState = CarEnteringState.AUTO;
    }

    public void CarHasLeft(GameObject car)
    {
        cars.Remove(car);
    }
}
