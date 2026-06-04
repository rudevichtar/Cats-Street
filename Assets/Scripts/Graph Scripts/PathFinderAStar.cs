using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// јлгоритм ј* - поиск оптимального пути между двум€ узлами графа
public class PathFinderAStar
{
    // »нформаци€ о перемещени€х
    private class PathRecord
    {
        public GraphNode Node; // узел
        public GraphNode CameFrom; // из какого узла пришли
        public float GCost; // стоимость пути от старта до узла
        public float HCost; // оценка до цели (сколько осталось)
        public float FCost => GCost + HCost; // перспективность узла (ј* выбирает узел с минимальным F)

        public PathRecord(GraphNode node, GraphNode cameFrom, float gCost, float hCost)
        {
            Node = node;
            CameFrom = cameFrom;
            GCost = gCost;
            HCost = hCost;
        }
    }

    public List<GraphNode> FindPath(GraphNode start, GraphNode goal) // ћетод поиска пути
    {
        List<GraphNode> empty = new List<GraphNode>();

        if (start == null || goal == null)
            return empty;

        if (start == goal)
        {
            empty.Add(start);
            return empty;
        }

        List<PathRecord> openList = new List<PathRecord>(); // узлы, которые надо рассмотреть
        HashSet<GraphNode> closedSet = new HashSet<GraphNode>(); // проверенные узлы

        Dictionary<GraphNode, PathRecord> allRecords = new Dictionary<GraphNode, PathRecord>(); // записи по всем узлам, которые встретились в поиске

        PathRecord startRecord = new PathRecord(start, null, 0f, Heuristic(start, goal)); // стартовый узел
        openList.Add(startRecord);
        allRecords[start] = startRecord;

        while (openList.Count > 0)
        {
            PathRecord current = GetLowestFCost(openList);

            if (current.Node == goal)
                return ReconstructPath(allRecords, goal);

            openList.Remove(current);
            closedSet.Add(current.Node);

            for (int i = 0; i < current.Node.Connections.Count; i++)
            {
                GraphConnection connection = current.Node.Connections[i];

                if (connection == null || connection.Target == null)
                    continue;

                GraphNode neighbor = connection.Target;

                if (neighbor.IsBlocked || connection.IsBlocked)
                    continue;

                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost =
                    current.GCost +
                    Vector3.Distance(current.Node.Position, neighbor.Position) +
                    connection.BaseCost;

                if (!allRecords.TryGetValue(neighbor, out PathRecord neighborRecord))
                {
                    neighborRecord = new PathRecord(
                        neighbor,
                        current.Node,
                        tentativeGCost,
                        Heuristic(neighbor, goal)
                    );

                    allRecords[neighbor] = neighborRecord;
                    openList.Add(neighborRecord);
                }
                else if (tentativeGCost < neighborRecord.GCost)
                {
                    neighborRecord.GCost = tentativeGCost;
                    neighborRecord.CameFrom = current.Node;

                    if (!openList.Contains(neighborRecord))
                        openList.Add(neighborRecord);
                }
            }
        }

        return empty;
    }

    private float Heuristic(GraphNode a, GraphNode b)
    {
        return Vector3.Distance(a.Position, b.Position);
    }

    private PathRecord GetLowestFCost(List<PathRecord> openList)
    {
        PathRecord best = openList[0];

        for (int i = 1; i < openList.Count; i++)
        {
            PathRecord candidate = openList[i];

            if (candidate.FCost < best.FCost)
            {
                best = candidate;
            }
            else if (Mathf.Approximately(candidate.FCost, best.FCost) && candidate.HCost < best.HCost)
            {
                best = candidate;
            }
        }

        return best;
    }

    private List<GraphNode> ReconstructPath(Dictionary<GraphNode, PathRecord> allRecords, GraphNode goal)
    {
        List<GraphNode> path = new List<GraphNode>();
        GraphNode current = goal;

        while (current != null)
        {
            path.Add(current);

            PathRecord record = allRecords[current];
            current = record.CameFrom;
        }

        path.Reverse();
        return path;
    }
}
