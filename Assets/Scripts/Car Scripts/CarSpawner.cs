using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private CarAgent carPrefab;
    [SerializeField] private RoadNode spawnNode;

    [Header("Spawn Timing")]
    [SerializeField] private float minDelay = 4f;
    [SerializeField] private float maxDelay = 8f;

    [Header("Spawn Rules")]
    [SerializeField] private int maxCarsAlive = 12;
    [SerializeField] private bool spawnOnStart = true;

    private float timer;
    private float currentDelay;

    public RoadNode SpawnNode => spawnNode;

    private void Start()
    {
        ScheduleNext();

        if (spawnOnStart)
            TrySpawn();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentDelay)
        {
            TrySpawn();
            ScheduleNext();
        }
    }

    private void ScheduleNext()
    {
        timer = 0f;
        currentDelay = Random.Range(minDelay, maxDelay);
    }

    private void TrySpawn()
    {
        if (carPrefab == null || spawnNode == null || TrafficSystem.Instance == null)
        {
            return;
        }

        if (TrafficSystem.Instance.ActiveCars.Count >= maxCarsAlive)
            return;

        CarAgent car = Instantiate(carPrefab, spawnNode.Position, Quaternion.identity);
        car.Initialize(spawnNode);
    }
}
