using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogDangerSystem : MonoBehaviour
{
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private DogDanger dogPrefab;

    [SerializeField] private float spawnInterval = 120f;
    [SerializeField] private float dogLifeTime = 120f;

    private float timer;
    private DogDanger currentDog;

    private void Start()
    {
        timer = spawnInterval;

        if (graphManager == null)
            graphManager = FindObjectOfType<GraphManager>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnDog();
            timer = spawnInterval;
        }
    }

    private void SpawnDog()
    {
        if (currentDog != null)
            currentDog.Remove();

        GraphNode node = GetRandomDogNode();

        if (node == null)
            return;

        currentDog = Instantiate(
            dogPrefab,
            node.Position,
            Quaternion.Euler(0f, 180f, 0f)
        );

        currentDog.Init(node);

        Destroy(currentDog.gameObject, dogLifeTime);
    }

    private GraphNode GetRandomDogNode()
    {
        List<GraphNode> candidates = new List<GraphNode>();

        foreach (GraphNode node in graphManager.Nodes)
        {
            if (node == null)
                continue;

            if (node.IsBlocked)
                continue;

            if (node.Type == NodeType.Normal)
                continue;

            if (node.Type == NodeType.Crossroad)
                continue;

            candidates.Add(node);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    public void SpawnDogOnNode(GraphNode node)
    {
        if (node == null || dogPrefab == null)
            return;

        DogDanger dog = Instantiate(
            dogPrefab,
            node.Position,
            Quaternion.identity
        );

        dog.Init(node);
    }
}
