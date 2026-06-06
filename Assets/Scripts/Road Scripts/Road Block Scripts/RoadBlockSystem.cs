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
        Instance = this;
    }

    public void ToggleRoad(GraphNode from, GraphNode to)
    {
        GraphConnection connection = FindConnection(from, to);

        if (connection == null)
            return;

        SetBlockedBidirectional(from, to, !connection.IsBlocked);
    }

    public bool SetBlockedBidirectional(GraphNode from, GraphNode to, bool blocked)
    {
        GraphConnection forward = FindConnection(from, to);
        GraphConnection backward = FindConnection(to, from);

        if (forward == null && backward == null)
            return false;

        bool wasBlocked =
            (forward != null && forward.IsBlocked) ||
            (backward != null && backward.IsBlocked);

        if (blocked && !wasBlocked)
        {
            if (BlockLimitManager.Instance != null)
            {
                if (!BlockLimitManager.Instance.CanBlockRoad())
                {
                    float timeLeft = BlockLimitManager.Instance.GetFirstRoadRemainingTime();

                    if (PopupHint.Instance != null)
                    {
                        PopupHint.Instance.Show(
                            $"Лимит закрытых дорог. Осталось {Mathf.CeilToInt(timeLeft)} сек."
                        );
                    }

                    return false;
                }

                BlockLimitManager.Instance.RegisterRoadBlock(maxBlockTime);
            }
        }

        if (!blocked && wasBlocked)
        {
            if (BlockLimitManager.Instance != null)
                BlockLimitManager.Instance.UnregisterRoadBlock();
        }

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
            Coroutine timer = StartCoroutine(AutoUnblockAfterDelay(from, to));
            unblockTimers.Add(key, timer);
        }

        return true;
    }

    private IEnumerator AutoUnblockAfterDelay(GraphNode from, GraphNode to)
    {
        yield return new WaitForSeconds(maxBlockTime);

        SetBlockedBidirectional(from, to, false);
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
