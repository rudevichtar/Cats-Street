using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public static TrafficSystem Instance { get; private set; }

    private readonly List<CarAgent> activeCars = new();
    private readonly List<CrosswalkController> crosswalks = new();

    public IReadOnlyList<CarAgent> ActiveCars => activeCars;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterCar(CarAgent car)
    {
        if (car == null || activeCars.Contains(car))
            return;

        activeCars.Add(car);
    }

    public void UnregisterCar(CarAgent car)
    {
        if (car == null)
            return;

        activeCars.Remove(car);
    }

    public void RegisterCrosswalk(CrosswalkController crosswalk)
    {
        if (crosswalk == null || crosswalks.Contains(crosswalk))
            return;

        crosswalks.Add(crosswalk);
    }

    public void UnregisterCrosswalk(CrosswalkController crosswalk)
    {
        if (crosswalk == null)
            return;

        crosswalks.Remove(crosswalk);
    }
}
