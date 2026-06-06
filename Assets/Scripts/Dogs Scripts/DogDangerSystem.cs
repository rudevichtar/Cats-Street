using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogDangerSystem : MonoBehaviour
{
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private DogDanger dogPrefab;

    [Header("Difficulty")]
    [SerializeField] private int maxDogs = 5;

    [Header("Relocation")]
    [SerializeField] private float relocationInterval = 30f;

    private readonly List<DogDanger> activeDogs = new List<DogDanger>();

    private float relocationTimer;
    private int currentDay = 1;

    private void Start()
    {
        if (graphManager == null)
            graphManager = FindObjectOfType<GraphManager>();

        relocationTimer = relocationInterval;

        ApplyDogDifficulty(1);
    }

    private void Update()
    {
        if (activeDogs.Count == 0)
            return;

        relocationTimer -= Time.deltaTime;

        if (relocationTimer <= 0f)
        {
            RelocateDogs();
            relocationTimer = relocationInterval;
        }
    }

    public void ApplyDogDifficulty(int day)
    {
        currentDay = Mathf.Max(1, day);

        int targetDogCount = Mathf.Clamp(currentDay / 2, 0, maxDogs);

        while (activeDogs.Count < targetDogCount)
        {
            SpawnDog();
        }

        while (activeDogs.Count > targetDogCount)
        {
            RemoveLastDog();
        }

        Debug.Log($"День {currentDay}. Активных собак: {activeDogs.Count}");
    }

    private void RelocateDogs()
    {
        int dogsCount = activeDogs.Count;

        RemoveAllDogs();

        for (int i = 0; i < dogsCount; i++)
        {
            SpawnDog();
        }

        Debug.Log("Собаки сменили расположение");
    }

    private void SpawnDog()
    {
        GraphNode node = GetRandomDogNode();

        if (node == null)
            return;

        DogDanger dog = Instantiate(
            dogPrefab,
            node.Position,
            Quaternion.Euler(0f, 180f, 0f)
        );

        dog.Init(node);
        activeDogs.Add(dog);
    }

    private void RemoveLastDog()
    {
        if (activeDogs.Count == 0)
            return;

        DogDanger dog = activeDogs[activeDogs.Count - 1];
        activeDogs.RemoveAt(activeDogs.Count - 1);

        if (dog != null)
            dog.Remove();
    }

    private void RemoveAllDogs()
    {
        for (int i = activeDogs.Count - 1; i >= 0; i--)
        {
            DogDanger dog = activeDogs[i];

            if (dog != null)
                dog.Remove();
        }

        activeDogs.Clear();
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

            if (IsNodeAlreadyWithDog(node))
                continue;

            candidates.Add(node);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private bool IsNodeAlreadyWithDog(GraphNode node)
    {
        foreach (DogDanger dog in activeDogs)
        {
            if (dog != null && dog.Node == node)
                return true;
        }

        return false;
    }

    public void SpawnDogOnNode(GraphNode node)
    {
        if (node == null || dogPrefab == null)
            return;

        DogDanger dog = Instantiate(
            dogPrefab,
            node.Position,
            Quaternion.Euler(0f, 180f, 0f)
        );

        dog.Init(node);
        activeDogs.Add(dog);
    }
}
