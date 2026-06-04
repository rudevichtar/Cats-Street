using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatPathDebugDrawer : MonoBehaviour
{
    public static CatPathDebugDrawer Instance { get; private set; }

    private readonly Dictionary<CatMover, List<GraphNode>> paths = new();

    [SerializeField] private float yOffset = 0.3f;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPath(CatMover cat, List<GraphNode> path)
    {
        if (cat == null)
            return;

        paths[cat] = path != null
            ? new List<GraphNode>(path)
            : new List<GraphNode>();
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = Vector3.up * yOffset;

        foreach (var pair in paths)
        {
            List<GraphNode> path = pair.Value;

            if (path == null || path.Count == 0)
                continue;

            Gizmos.color = Color.magenta;

            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] == null || path[i + 1] == null)
                    continue;

                Gizmos.DrawLine(path[i].Position + offset, path[i + 1].Position + offset);
                Gizmos.DrawSphere(path[i].Position + offset, 0.12f);
            }

            GraphNode last = path[path.Count - 1];

            if (last != null)
                Gizmos.DrawSphere(last.Position + offset, 0.15f);
        }
    }
}
