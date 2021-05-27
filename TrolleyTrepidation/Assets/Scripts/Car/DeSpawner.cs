using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeSpawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CarMovement car;
        if (other.TryGetComponent(out car))
        {
            car.DeSpawn();
        }
    }
}
