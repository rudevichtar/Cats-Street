using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPathFinder : MonoBehaviour
{
    public static List<RoadNode> FindPath(RoadNode start, RoadNode goal)
    {
        List<RoadNode> empty = new List<RoadNode>();

        if (start == null || goal == null)
            return empty;

        Queue<RoadNode> open = new Queue<RoadNode>();
        HashSet<RoadNode> visited = new HashSet<RoadNode>();
        Dictionary<RoadNode, RoadNode> cameFrom = new Dictionary<RoadNode, RoadNode>();

        open.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = null;

        while (open.Count > 0)
        {
            RoadNode current = open.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, goal);

            foreach (RoadNode next in current.NextNodes)
            {
                if (next == null || visited.Contains(next))
                    continue;

                visited.Add(next);
                cameFrom[next] = current;
                open.Enqueue(next);
            }
        }

        return empty;
    }

    private static List<RoadNode> ReconstructPath(Dictionary<RoadNode, RoadNode> cameFrom, RoadNode goal)
    {
        List<RoadNode> path = new List<RoadNode>();

        RoadNode current = goal;

        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }
}
