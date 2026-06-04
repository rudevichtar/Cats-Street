using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    [Header("Objects")]
    //[SerializeField] private Transform spawnPoint;
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private CatUIList catUIList;

    [Header("Cat Prefabs")]
    [SerializeField] private List<GameObject> catPrefabs = new();

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Auto Spawn")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float spawnInterval = 60f;
    [SerializeField] private int maxCats = 10;

    private float timer;
    private int catsAlive;

    private void Start()
    {
        timer = spawnInterval;

        if (spawnOnStart)
            SpawnCat();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnCat();
            timer = spawnInterval;
        }
    }

    public void SpawnCat()
    {
        if (catsAlive >= maxCats)
            return;

        if (catPrefabs == null || catPrefabs.Count == 0)
        {
            Debug.LogWarning("Нет префабов котов");
            return;
        }

        GameObject randomPrefab =
            catPrefabs[Random.Range(0, catPrefabs.Count)];

        GameObject catObject = Instantiate(
            randomPrefab,
            GetRandomSpawnPoint(spawnPoints).position,
            GetRandomSpawnPoint(spawnPoints).rotation
        );

        catsAlive++;

        CatProfile profile = catObject.GetComponent<CatProfile>();
        if (profile != null)
            profile.CatName = RandomName.GetRandomCatName();

        CatBrain brain = catObject.GetComponent<CatBrain>();
        if (brain != null)
            brain.GraphManager = graphManager;

        CatNeeds needs = catObject.GetComponent<CatNeeds>();
        if (needs != null)
        {
            catUIList.AddCat(needs);
            needs.OnCatDied += HandleCatDied;
        }
    }

    private void HandleCatDied(CatNeeds catNeeds)
    {
        catsAlive--;

        if (catNeeds != null)
            catNeeds.OnCatDied -= HandleCatDied;
    }

    private Transform GetRandomSpawnPoint(List<Transform> spawnPoints)
    {
        Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];

        return randomSpawn;
    }
}
