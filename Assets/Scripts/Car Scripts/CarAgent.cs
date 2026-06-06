using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAgent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float reachDistance = 0.08f;

    [Header("Route")]
    [SerializeField] private int lookAheadNodes = 8;
    [SerializeField] private int maxVisitedNodes = 40;

    [Header("Debug")]
    [SerializeField] private RoadNode currentNode;
    [SerializeField] private RoadNode nextNode;
    [SerializeField] private RoadNode previousNode;
    [SerializeField] private List<RoadNode> plannedRoute = new();

    private float baseMoveSpeed;

    private int visitedCount;
    private bool initialized;

    public float MoveSpeed => moveSpeed;
    public RoadNode CurrentNode => currentNode;
    public RoadNode NextNode => nextNode;
    public bool IsInitialized => initialized;

    public void Initialize(RoadNode startNode)
    {
        if (startNode == null)
        {
            Destroy(gameObject);
            return;
        }

        currentNode = startNode;
        previousNode = null;
        visitedCount = 0;
        transform.position = startNode.Position;

        BuildPlannedRoute();
        AdvanceNextNodeFromPlan();

        initialized = nextNode != null;

        if (TrafficSystem.Instance != null)
            TrafficSystem.Instance.RegisterCar(this);
    }

    private void OnDestroy()
    {
        if (TrafficSystem.Instance != null)
            TrafficSystem.Instance.UnregisterCar(this);
    }

    private void Awake()
    {
        baseMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (!initialized || nextNode == null)
        {
            Destroy(gameObject);
            return;
        }

        MoveAlongPath();
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        moveSpeed = baseMoveSpeed * multiplier;
    }

    private void MoveAlongPath()
    {
        Vector3 target = nextNode.Position;
        Vector3 flatDir = target - transform.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDir.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= reachDistance)
        {
            previousNode = currentNode;
            currentNode = nextNode;
            visitedCount++;

            if (visitedCount >= maxVisitedNodes)
            {
                Destroy(gameObject);
                return;
            }

            if (plannedRoute.Count == 0)
                BuildPlannedRoute();

            AdvanceNextNodeFromPlan();

            if (nextNode == null)
                Destroy(gameObject);
        }
    }

    private void BuildPlannedRoute()
    {
        plannedRoute.Clear();

        RoadNode tempCurrent = currentNode;
        RoadNode tempPrevious = previousNode;

        for (int i = 0; i < lookAheadNodes; i++)
        {
            RoadNode chosen = ChooseNextNode(tempCurrent, tempPrevious);
            if (chosen == null)
                break;

            plannedRoute.Add(chosen);
            tempPrevious = tempCurrent;
            tempCurrent = chosen;
        }
    }

    private void AdvanceNextNodeFromPlan()
    {
        if (plannedRoute.Count == 0)
        {
            nextNode = null;
            return;
        }

        nextNode = plannedRoute[0];
        plannedRoute.RemoveAt(0);

        if (plannedRoute.Count < 2)
            BuildPlannedRouteFromFuture();
    }

    private void BuildPlannedRouteFromFuture()
    {
        RoadNode tempPrevious;
        RoadNode tempCurrent;

        if (plannedRoute.Count > 0)
        {
            tempCurrent = plannedRoute[plannedRoute.Count - 1];

            if (plannedRoute.Count > 1)
                tempPrevious = plannedRoute[plannedRoute.Count - 2];
            else
                tempPrevious = nextNode;
        }
        else
        {
            tempCurrent = nextNode;
            tempPrevious = currentNode;
        }

        int missingCount = Mathf.Max(0, lookAheadNodes - plannedRoute.Count);

        for (int i = 0; i < missingCount; i++)
        {
            RoadNode chosen = ChooseNextNode(tempCurrent, tempPrevious);
            if (chosen == null)
                break;

            plannedRoute.Add(chosen);
            tempPrevious = tempCurrent;
            tempCurrent = chosen;
        }
    }

    private RoadNode ChooseNextNode(RoadNode from, RoadNode previous)
    {
        if (from == null || from.NextNodes == null || from.NextNodes.Count == 0)
            return null;

        List<RoadNode> candidates = new();

        foreach (RoadNode node in from.NextNodes)
        {
            if (node == null)
                continue;

            // Ќе возвращаемс€ назад, если есть другие варианты
            if (previous != null && node == previous && from.NextNodes.Count > 1)
                continue;

            candidates.Add(node);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    public float EstimateTimeToPoint(Vector3 point, float tolerance = 0.6f)
    {
        if (!initialized || nextNode == null || moveSpeed <= 0.01f)
            return float.PositiveInfinity;

        float totalDistance = 0f;

        Vector3 currentPos = transform.position;

        if (IsPointNearSegment(currentPos, nextNode.Position, point, tolerance))
        {
            totalDistance += DistanceOnSegment(currentPos, nextNode.Position, point);
            return totalDistance / moveSpeed;
        }

        totalDistance += Vector3.Distance(currentPos, nextNode.Position);

        RoadNode segStart = nextNode;
        RoadNode segEnd = null;

        for (int i = 0; i < plannedRoute.Count; i++)
        {
            segEnd = plannedRoute[i];

            if (segStart == null || segEnd == null)
                break;

            if (IsPointNearSegment(segStart.Position, segEnd.Position, point, tolerance))
            {
                totalDistance += DistanceOnSegment(segStart.Position, segEnd.Position, point);
                return totalDistance / moveSpeed;
            }

            totalDistance += Vector3.Distance(segStart.Position, segEnd.Position);
            segStart = segEnd;
        }

        return float.PositiveInfinity;
    }

    private bool IsPointNearSegment(Vector3 a, Vector3 b, Vector3 point, float tolerance)
    {
        Vector3 projected = ProjectPointOnSegment(a, b, point);
        return Vector3.Distance(projected, point) <= tolerance;
    }

    private float DistanceOnSegment(Vector3 segmentStart, Vector3 segmentEnd, Vector3 point)
    {
        Vector3 projected = ProjectPointOnSegment(segmentStart, segmentEnd, point);
        return Vector3.Distance(segmentStart, projected);
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

    private void OnDrawGizmosSelected()
    {
        if (nextNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, nextNode.Position);
        }

        if (plannedRoute == null || plannedRoute.Count == 0)
            return;

        Gizmos.color = Color.magenta;
        Vector3 from = nextNode != null ? nextNode.Position : transform.position;

        foreach (RoadNode node in plannedRoute)
        {
            if (node == null)
                continue;

            Gizmos.DrawLine(from, node.Position);
            from = node.Position;
        }
    }
}
