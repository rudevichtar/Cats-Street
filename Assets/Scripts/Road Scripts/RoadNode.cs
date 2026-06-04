using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour
{
    [SerializeField] private List<RoadNode> nextNodes = new();

    public IReadOnlyList<RoadNode> NextNodes => nextNodes;
    public Vector3 Position => transform.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.18f);

        if (nextNodes == null)
            return;

        Gizmos.color = Color.cyan;
        foreach (RoadNode next in nextNodes)
        {
            if (next == null)
                continue;

            Gizmos.DrawLine(transform.position, next.transform.position);

            Vector3 dir = (next.transform.position - transform.position).normalized;
            Vector3 arrowPos = Vector3.Lerp(transform.position, next.transform.position, 0.75f);
            Gizmos.DrawRay(arrowPos, Quaternion.Euler(0f, 25f, 0f) * -dir * 0.35f);
            Gizmos.DrawRay(arrowPos, Quaternion.Euler(0f, -25f, 0f) * -dir * 0.35f);
        }
    }
}
