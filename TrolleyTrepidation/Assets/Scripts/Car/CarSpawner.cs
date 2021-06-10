using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private const int MIN_CARS = 20;
    private const int MAX_CARS = 30;
    private const int MIN_TIME_OUT = 15;
    private const int MAX_TIME_OUT = 30;
    private const int MIN_TIME_IN = 15;
    private const int MAX_TIME_IN = 30;
    private const float SPAWN_DISTANCE = 30f;

    public GameObject[] carPrefabs;
    public GameObject spawnNodes;
    public GameObject parkNodes;
    public GameObject[] exitNodes;
    public List<GameObject> cars;

    private bool allParksFull;

    // Start is called before the first frame update
    void Start()
    {
        cars = new List<GameObject>();
        allParksFull = false;

        PopulateCarPark();
        InvokeRepeating("LeaveCarPark", Random.Range(MIN_TIME_OUT, MAX_TIME_OUT), Random.Range(MIN_TIME_OUT, MAX_TIME_OUT));
        InvokeRepeating("EnterCarPark", Random.Range(MIN_TIME_IN, MAX_TIME_IN), Random.Range(MIN_TIME_IN, MAX_TIME_IN));
    }

    private void PopulateCarPark()
    {
        int carCount = Random.Range(MIN_CARS, MAX_CARS);

        for (int c = 0; c < carCount; c++)
        {
            SpawnParkedCar();
        }
        
    }

    private void CheckAllParksFull()
    {
        bool allFull = true;

        foreach (Transform park in parkNodes.transform)
        {
            ParkNodeController parkInfo = park.GetComponent<ParkNodeController>();
            float distanceToPlayer = Vector3.Distance(park.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

            if (parkInfo.isEmpty && distanceToPlayer >= SPAWN_DISTANCE)
            {
                allFull = false;
                break;
            }
        }

        allParksFull = allFull;
    }

    private void SpawnParkedCar()
    {
        int spawnPointIndex;
        bool spaceFree;
        Transform spawnPoint;
        ParkNodeController parkingSpace;

        if (!allParksFull)
        {
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
            carMovement.park = parkingSpace;
            carMovement.leavingState = CarLeavingState.STOPPED;
            cars.Add(carSpawn);
        }

        CheckAllParksFull();
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
            carIsParked = carMovement.leavingState == CarLeavingState.STOPPED;
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
                carIsParked =  carMovement.leavingState == CarLeavingState.STOPPED;

            } while (!carIsParked);

            carMovement.goal = exitNodes[Random.Range(0, exitNodes.Length)].transform;
            carMovement.leavingState = CarLeavingState.REVERSING;
        }
    }

    private void EnterCarPark()
    {
        GameObject park;
        ParkNodeController parkInfo;

        bool parkIsFull;
        float distanceToPlayer;

        if (!allParksFull)
        {
            do
            {
                park = parkNodes.transform.GetChild(Random.Range(0, parkNodes.transform.childCount)).gameObject;
                parkInfo = park.GetComponent<ParkNodeController>();
                distanceToPlayer = Vector3.Distance(park.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

                parkIsFull = !parkInfo.isEmpty;

            } while (parkIsFull || distanceToPlayer < SPAWN_DISTANCE);

            GameObject carSpawn = Instantiate(
            carPrefabs[Random.Range(0, carPrefabs.Length)],
            park.transform
        );

            cars.Add(carSpawn);

            CarMovement carMovement = carSpawn.GetComponent<CarMovement>();
            carMovement.park = parkInfo;
            carMovement.park.isEmpty = false;
        }
        
        CheckAllParksFull();
    }

    public void CarHasLeft(GameObject car)
    {
        cars.Remove(car);
    }
}
