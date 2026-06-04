using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Система блокировки пути
public class RoadBlockSystem : MonoBehaviour
{
    public static RoadBlockSystem Instance { get; private set; }

    [SerializeField] private float maxBlockTime = 5f;

    private readonly Dictionary<string, Coroutine> unblockTimers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ToggleRoad(GraphNode from, GraphNode to)
    {
        GraphConnection connection = FindConnection(from, to);

        if (connection == null)
        {
            Debug.LogWarning($"Связь не найдена: {from.name} -> {to.name}");
            return;
        }

        bool shouldBeBlocked = !connection.IsBlocked;
        SetBlockedBidirectional(from, to, shouldBeBlocked);
    }

    public void SetBlockedBidirectional(GraphNode from, GraphNode to, bool blocked)
    {
        GraphConnection forward = FindConnection(from, to);
        GraphConnection backward = FindConnection(to, from);

        if (forward != null)
            forward.SetBlocked(blocked);

        if (backward != null)
            backward.SetBlocked(blocked);

        string key = GetRoadKey(from, to);

        if (unblockTimers.TryGetValue(key, out Coroutine oldTimer))
        {
            StopCoroutine(oldTimer);
            unblockTimers.Remove(key);
        }

        if (blocked)
        {
            Coroutine timer = StartCoroutine(AutoUnblockAfterDelay(from, to, key));
            unblockTimers.Add(key, timer);
        }
    }

    private IEnumerator AutoUnblockAfterDelay(GraphNode from, GraphNode to, string key)
    {
        yield return new WaitForSeconds(maxBlockTime);

        GraphConnection forward = FindConnection(from, to);

        if (forward != null && forward.IsBlocked)
            SetBlockedBidirectional(from, to, false);

        unblockTimers.Remove(key);
    }

    private GraphConnection FindConnection(GraphNode from, GraphNode to)
    {
        if (from == null || to == null)
            return null;

        return from.Connections.Find(c => c.Target == to);
    }

    private string GetRoadKey(GraphNode a, GraphNode b)
    {
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();

        return idA < idB
            ? $"{idA}_{idB}"
            : $"{idB}_{idA}";
    }
}
