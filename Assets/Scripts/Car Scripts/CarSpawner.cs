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

    [Header("Difficulty")]
    [SerializeField] private int currentDay = 1;

    [SerializeField] private float baseMinDelay = 6f;
    [SerializeField] private float baseMaxDelay = 12f;

    [SerializeField] private float minDelayLimit = 2f;
    [SerializeField] private float maxDelayLimit = 5f;

    [SerializeField] private int baseMaxCarsAlive = 5;
    [SerializeField] private int maxCarsLimit = 18;

    [SerializeField] private float carsPerDay = 2f;

    [SerializeField] private float speedMultiplierPerDay = 0.05f;
    [SerializeField] private float maxSpeedMultiplier = 1.5f;

    private float currentSpeedMultiplier = 1f;

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
        car.SetSpeedMultiplier(currentSpeedMultiplier);
        car.Initialize(spawnNode);
    }

    public void ApplyTrafficDifficulty(int day)
    {
        currentDay = Mathf.Max(1, day);

        float dayFactor = currentDay - 1;

        minDelay = Mathf.Max(minDelayLimit, baseMinDelay - dayFactor * 0.5f);
        maxDelay = Mathf.Max(maxDelayLimit, baseMaxDelay - dayFactor * 0.7f);

        maxCarsAlive = Mathf.Min(
            maxCarsLimit,
            baseMaxCarsAlive + Mathf.FloorToInt(dayFactor * carsPerDay)
        );

        currentSpeedMultiplier = Mathf.Min(
            maxSpeedMultiplier,
            1f + (currentDay - 1) * speedMultiplierPerDay
        );

        Debug.Log($"Трафик обновлён. День {currentDay}. Delay: {minDelay}-{maxDelay}, MaxCars: {maxCarsAlive}");
    }
}
