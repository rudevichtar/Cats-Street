using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Менеджер графа (карта города). Собирает все узлы, хранит их список, помогает искать нужные узлы.
public class GraphManager : MonoBehaviour
{
    [Header("Auto collect nodes from scene")]
    public bool AutoCollectOnAwake = true; // Автоматический сбор всех узлов на карте

    [Header("Graph nodes")]
    public List<GraphNode> Nodes = new List<GraphNode>(); // Список всех узлов

    private void Awake()
    {
        if (AutoCollectOnAwake)
            CollectNodesFromScene();

        //DebugGraphConnections();
    }

    [ContextMenu("Debug Graph Connections")]
    public void DebugGraphConnections()
    {
        foreach (GraphNode node in Nodes)
        {
            foreach (GraphConnection connection in node.Connections)
            {
                if (connection.Target == null)
                    continue;

                Debug.Log($"{node.Id} -> {connection.Target.Id}, blocked: {connection.IsBlocked}");
            }
        }
    }

    [ContextMenu("Collect Nodes From Scene")]
    public void CollectNodesFromScene() //Собирает узлы со сцены
    {
        GraphNode[] foundNodes = FindObjectsOfType<GraphNode>();
        Nodes = new List<GraphNode>(foundNodes);

        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Id = i;
        }
    }

    public GraphNode GetClosestNode(Vector3 position, bool ignoreBlocked = true) // Возвращает ближайший к позиции узел
    {
        GraphNode closest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < Nodes.Count; i++)
        {
            GraphNode node = Nodes[i];

            if (node == null)
                continue;

            if (ignoreBlocked && node.IsBlocked)
                continue;

            float distance = Vector3.Distance(position, node.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = node;
            }
        }

        return closest;
    }

    public List<GraphNode> GetNodesByType(NodeType type, bool ignoreBlocked = true) // Возвращает все узлы конкретного типа (для поиска цели в CatBrain)
    {
        List<GraphNode> result = new List<GraphNode>();

        for (int i = 0; i < Nodes.Count; i++)
        {
            GraphNode node = Nodes[i];

            if (node == null)
                continue;

            if (node.Type != type)
                continue;

            if (ignoreBlocked && node.IsBlocked)
                continue;

            result.Add(node);
        }

        return result;
    }

    public GraphNode GetClosestNodeOfType(Vector3 position, NodeType type, bool ignoreBlocked = true) // Выбор ближайшего узла из выбранных узлов конкретного типа
    {
        List<GraphNode> typedNodes = GetNodesByType(type, ignoreBlocked);

        GraphNode closest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < typedNodes.Count; i++)
        {
            GraphNode node = typedNodes[i];
            float distance = Vector3.Distance(position, node.Position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = node;
            }
        }

        return closest;
    }

    public GraphNode GetSafestNode(Vector3 position, bool ignoreBlocked = true) // Поиск самого безопасного узла
    {
        GraphNode safest = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < Nodes.Count; i++)
        {
            GraphNode node = Nodes[i];

            if (node == null)
                continue;

            if (ignoreBlocked && node.IsBlocked)
                continue;

            float distance = Vector3.Distance(position, node.Position);
            float score = node.DangerLevel * 10f + distance;

            if (score < bestScore)
            {
                bestScore = score;
                safest = node;
            }
        }

        return safest;
    }
}
