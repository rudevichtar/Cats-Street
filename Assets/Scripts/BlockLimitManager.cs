using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLimitManager : MonoBehaviour
{
    public static BlockLimitManager Instance { get; private set; }

    [SerializeField] private int currentDay = 1;

    private readonly List<float> roadUnblockTimes = new();
    private readonly List<float> nodeUnblockTimes = new();

    public int MaxRoads => 1 + (currentDay - 1) / 2;
    public int MaxNodes => 1 + (currentDay - 1) / 2;

    private void Awake()
    {
        Instance = this;
    }

    public void SetDay(int day)
    {
        currentDay = Mathf.Max(1, day);
    }

    public bool CanBlockRoad()
    {
        ClearExpired();
        return roadUnblockTimes.Count < MaxRoads;
    }

    public bool CanBlockNode()
    {
        ClearExpired();
        return nodeUnblockTimes.Count < MaxNodes;
    }

    public void RegisterRoadBlock(float duration)
    {
        roadUnblockTimes.Add(Time.time + duration);
    }

    public void RegisterNodeBlock(float duration)
    {
        nodeUnblockTimes.Add(Time.time + duration);
    }

    public void UnregisterRoadBlock()
    {
        if (roadUnblockTimes.Count > 0)
            roadUnblockTimes.RemoveAt(0);
    }

    public void UnregisterNodeBlock()
    {
        if (nodeUnblockTimes.Count > 0)
            nodeUnblockTimes.RemoveAt(0);
    }

    public float GetFirstRoadRemainingTime()
    {
        ClearExpired();
        return GetFirstRemainingTime(roadUnblockTimes);
    }

    public float GetFirstNodeRemainingTime()
    {
        ClearExpired();
        return GetFirstRemainingTime(nodeUnblockTimes);
    }

    private float GetFirstRemainingTime(List<float> times)
    {
        if (times.Count == 0)
            return 0f;

        float minTime = times[0];

        for (int i = 1; i < times.Count; i++)
        {
            if (times[i] < minTime)
                minTime = times[i];
        }

        return Mathf.Max(0f, minTime - Time.time);
    }

    private void ClearExpired()
    {
        roadUnblockTimes.RemoveAll(t => t <= Time.time);
        nodeUnblockTimes.RemoveAll(t => t <= Time.time);
    }
}
