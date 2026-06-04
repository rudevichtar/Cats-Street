using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Узел графа. Его свойства, отрисовка на карте и подсчёт стоимости перехода
public class GraphNode : MonoBehaviour
{
    [Header("Node settings")]
    public int Id;
    public NodeType Type = NodeType.Normal;
    public float DangerLevel = 0f;
    public bool IsBlocked = false;

    public CatPlaceableResource CurrentResource { get; private set; }
    public bool HasResource => CurrentResource != null;

    //public List<NodeConnection> Connections;

    [Header("Connections")]
    public List<GraphConnection> Connections = new List<GraphConnection>();

    public Vector3 Position => transform.position;

    public float GetCostTo(GraphNode target/*, float dangerWeight = 1f*/)
    {
        if (target == null || target.IsBlocked)
            return float.PositiveInfinity;

        for (int i = 0; i < Connections.Count; i++)
        {
            GraphConnection connection = Connections[i];

            if (connection == null || connection.Target == null)
                continue;

            if (connection.IsBlocked)
                continue;

            return connection.BaseCost;

            /*if (connection.Target == target)
            {
                float distance = Vector3.Distance(Position, target.Position);

                return distance
                       + connection.BaseCost
                       + (connection.DangerCost + target.DangerLevel) * dangerWeight;
            }*/
        }

        return float.PositiveInfinity;
    }

    private void OnDrawGizmos()
    {
        Color nodeColor = Type switch
        {
            NodeType.Food => Color.green,
            NodeType.CatsZone => Color.cyan,
            NodeType.SleepSpot => Color.blue,
            NodeType.DangerZone => Color.red,
            NodeType.Crossroad => Color.yellow,
            _ => Color.white
        };

        Gizmos.color = IsBlocked ? Color.black : nodeColor;
        Gizmos.DrawSphere(transform.position, 0.22f);

        if (Connections == null)
            return;

        for (int i = 0; i < Connections.Count; i++)
        {
            GraphConnection connection = Connections[i];

            if (connection == null || connection.Target == null)
                continue;

            Gizmos.color = connection.IsBlocked ? Color.red : new Color(1f, 0.8f, 0.5f, 1f);
            Gizmos.DrawLine(transform.position, connection.Target.transform.position);
        }
    }

    public bool HasAvailableConnectionTo(GraphNode node)
    {
        if (node == null || Connections == null)
            return false;

        for (int i = 0; i < Connections.Count; i++)
        {
            GraphConnection connection = Connections[i];

            if (connection == null || connection.Target == null)
                continue;

            if (connection.Target == node && !connection.IsBlocked)
                return true;
        }

        return false;
    }

    public void SetResource(CatPlaceableResource resource)
    {
        CurrentResource = resource;
    }

    public void ClearResource(CatPlaceableResource resource)
    {
        if (CurrentResource == resource)
            CurrentResource = null;
    }
}
