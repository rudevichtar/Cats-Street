using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbulanceSystem : MonoBehaviour
{
    public static AmbulanceSystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AmbulanceAgent ambulancePrefab;
    [SerializeField] private List<CarSpawner> carSpawners = new List<CarSpawner>();

    [Header("Settings")]
    [SerializeField] private float catRoadSearchRadius = 3f;
    [SerializeField] private int afterPickupNodes = 6;

    private void Awake()
    {
        Instance = this;
    }

    public void CallAmbulance(InjuredCat cat)
    {
        if (cat == null || ambulancePrefab == null)
            return;

        RoadNode catNode = FindRoadNodeAfterCat(cat.transform.position);
        if (catNode == null)
        {
            Debug.LogWarning("Не нашла RoadNode рядом с котом");
            return;
        }

        CarSpawner spawner = FindBestSpawner(catNode);
        if (spawner == null || spawner.SpawnNode == null)
        {
            Debug.LogWarning("Не нашла подходящий спавнер для скорой");
            return;
        }

        List<RoadNode> pathToCat = RoadPathFinder.FindPath(spawner.SpawnNode, catNode);

        if (pathToCat == null || pathToCat.Count == 0)
        {
            Debug.LogWarning("Не удалось построить путь скорой до кота");
            return;
        }

        ExtendRouteAfterCat(pathToCat);

        AmbulanceAgent ambulance = Instantiate(
            ambulancePrefab,
            spawner.SpawnNode.Position,
            Quaternion.identity
        );

        ambulance.Initialize(pathToCat, cat);
    }

    /*private RoadNode FindClosestRoadNode(Vector3 position)
    {
        RoadNode[] nodes = FindObjectsByType<RoadNode>(FindObjectsSortMode.None);

        RoadNode closest = null;
        float bestDistance = float.MaxValue;

        foreach (RoadNode node in nodes)
        {
            if (node == null)
                continue;

            float distance = Vector3.Distance(position, node.Position);

            if (distance < bestDistance && distance <= catRoadSearchRadius)
            {
                bestDistance = distance;
                closest = node;
            }
        }

        return closest;
    }*/

    private CarSpawner FindBestSpawner(RoadNode targetNode)
    {
        CarSpawner bestSpawner = null;
        int bestPathLength = int.MaxValue;

        foreach (CarSpawner spawner in carSpawners)
        {
            if (spawner == null || spawner.SpawnNode == null)
                continue;

            List<RoadNode> path = RoadPathFinder.FindPath(spawner.SpawnNode, targetNode);

            if (path == null || path.Count == 0)
                continue;

            if (path.Count < bestPathLength)
            {
                bestPathLength = path.Count;
                bestSpawner = spawner;
            }
        }

        return bestSpawner;
    }

    private void ExtendRouteAfterCat(List<RoadNode> route)
    {
        if (route == null || route.Count == 0)
            return;

        RoadNode current = route[route.Count - 1];
        RoadNode previous = route.Count >= 2 ? route[route.Count - 2] : null;

        for (int i = 0; i < afterPickupNodes; i++)
        {
            RoadNode next = ChooseNextNode(current, previous);

            if (next == null)
                break;

            route.Add(next);

            previous = current;
            current = next;
        }
    }

    private RoadNode ChooseNextNode(RoadNode current, RoadNode previous)
    {
        if (current == null || current.NextNodes == null || current.NextNodes.Count == 0)
            return null;

        List<RoadNode> candidates = new List<RoadNode>();

        foreach (RoadNode next in current.NextNodes)
        {
            if (next == null)
                continue;

            if (previous != null && next == previous && current.NextNodes.Count > 1)
                continue;

            candidates.Add(next);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private RoadNode FindRoadNodeAfterCat(Vector3 catPosition)
    {
        RoadNode[] nodes = FindObjectsByType<RoadNode>(FindObjectsSortMode.None);

        RoadNode bestTargetNode = null;
        float bestDistance = float.MaxValue;

        foreach (RoadNode from in nodes)
        {
            if (from == null || from.NextNodes == null)
                continue;

            foreach (RoadNode to in from.NextNodes)
            {
                if (to == null)
                    continue;

                Vector3 projected = ProjectPointOnSegment(from.Position, to.Position, catPosition);
                float distance = Vector3.Distance(projected, catPosition);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTargetNode = to;
                }
            }
        }

        if (bestDistance > catRoadSearchRadius)
        {
            Debug.LogWarning($"Кот слишком далеко от дороги машин. Расстояние: {bestDistance}");
            return null;
        }

        Debug.Log($"Точка для скорой найдена: RoadNode {bestTargetNode.name}, расстояние до дороги: {bestDistance}");

        return bestTargetNode;
    }

    private Vector3 ProjectPointOnSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float sqrLength = ab.sqrMagnitude;

        if (sqrLength < 0.0001f)
            return a;

        float t = Vector3.Dot(point - a, ab) / sqrLength;
        t = Mathf.Clamp01(t);

        return a + ab * t;
    }
}
