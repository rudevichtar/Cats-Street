using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Описание одного ребра графа (связь от одной точки к другой). Параметры
[Serializable]
public class GraphConnection
{
    public GraphNode Target;
    public float BaseCost = 1f;
    public float DangerCost = 0f;
    public bool IsBlocked = false;

    public bool VisualBidirectional = true;

    public RoadBlockVisual BlockVisual;

    public void SetBlocked(bool blocked)
    {
        IsBlocked = blocked;

        if (BlockVisual != null)
            BlockVisual.Play(blocked);
    }
}
