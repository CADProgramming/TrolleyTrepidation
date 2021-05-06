using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private const int MIN_CARS = 100;
    private const int MAX_CARS = 100;
    private const int MIN_TIME = 5;
    private const int MAX_TIME = 5;

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
        InvokeRepeating("LeaveCarPark", Random.Range(MIN_TIME, MAX_TIME), Random.Range(MIN_TIME, MAX_TIME));
        InvokeRepeating("EnterCarPark", Random.Range(MIN_TIME, MAX_TIME), Random.Range(MIN_TIME, MAX_TIME));
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

    private void EnterCarPark()
    {

    }

    public void CarHasLeft(GameObject car)
    {
        cars.Remove(car);
    }
}
