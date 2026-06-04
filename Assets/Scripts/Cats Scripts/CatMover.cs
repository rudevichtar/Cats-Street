using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Движение кошки
public class CatMover : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 2.5f;
    public float RotationSpeed = 8f;
    public float ReachDistance = 0.08f;

    [Header("Debug")]
    [SerializeField] private List<GraphNode> currentPath = new List<GraphNode>();
    [SerializeField] private int currentIndex = 0;
    [SerializeField] private bool isMoving = false;

    public bool IsMoving => isMoving;
    public GraphNode CurrentTargetNode =>
        (currentPath != null && currentIndex < currentPath.Count) ? currentPath[currentIndex] : null;

    private GraphNode lastReachedNode;
    public GraphNode LastReachedNode => lastReachedNode;

    public event Action OnPathCompleted;
    public event Action OnPathInvalid;

    // Получение нового маршрута
    public void SetPath(List<GraphNode> path)
    {
        if (CatPathDebugDrawer.Instance != null)
            CatPathDebugDrawer.Instance.SetPath(this, path);

        currentPath = path != null ? new List<GraphNode>(path) : new List<GraphNode>();

        if (currentPath.Count > 0)
        {
            currentIndex = 0;

            if (Vector3.Distance(transform.position, currentPath[0].Position) <= ReachDistance)
            {
                lastReachedNode = currentPath[0];

                if (currentPath.Count > 1)
                    currentIndex = 1;
            }

            isMoving = currentIndex < currentPath.Count;
        }
        else
        {
            currentIndex = 0;
            isMoving = false;
        }
    }

    // Сброс маршрута - остановка
    public void StopMoving()
    {
        isMoving = false;
        currentPath.Clear();
        currentIndex = 0;
    }

    // Движение
    private void Update()
    {
        if (!isMoving || currentPath == null || currentIndex >= currentPath.Count)
            return;

        if (IsCurrentStepBlocked() || HasBlockedEdgeInRemainPath())
        {
            isMoving = false;
            OnPathInvalid?.Invoke();
            return;
        }

        GraphNode targetNode = currentPath[currentIndex];

        Vector3 targetPosition = targetNode.Position;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction.normalized) * Quaternion.Euler(0f, 180f, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                RotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            MoveSpeed * Time.deltaTime
        );

        float flatDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(targetPosition.x, 0f, targetPosition.z)
        );

        if (flatDistance <= ReachDistance)
        {
            lastReachedNode = targetNode;

            DogDanger dog = FindDogOnNode(targetNode);
            if (dog != null)
            {
                CatNeeds needs = GetComponent<CatNeeds>();
                if (needs != null)
                    dog.BiteCat(needs);
            }

            currentIndex++;

            if (currentIndex >= currentPath.Count)
            {
                isMoving = false;
                OnPathCompleted?.Invoke();
            }
        }
    }

    private DogDanger FindDogOnNode(GraphNode node)
    {
        DogDanger[] dogs = FindObjectsByType<DogDanger>(FindObjectsSortMode.None);

        foreach (DogDanger dog in dogs)
        {
            if (dog != null && dog.Node == node)
                return dog;
        }

        return null;
    }

    /*private void OnDrawGizmos()
    {
        if (debugFullPath == null || debugFullPath.Count == 0)
            return;

        Gizmos.color = Color.magenta;

        for (int i = 0; i < debugFullPath.Count - 1; i++)
        {
            if (debugFullPath[i] == null || debugFullPath[i + 1] == null)
                continue;

            Gizmos.DrawLine(
                debugFullPath[i].Position,
                debugFullPath[i + 1].Position
            );

            Gizmos.DrawSphere(debugFullPath[i].Position, 0.12f);
        }

        GraphNode lastNode = debugFullPath[debugFullPath.Count - 1];
        if (lastNode != null)
            Gizmos.DrawSphere(lastNode.Position, 0.15f);
    }*/

    // Отрисовка текущего маршрута кошки
    /*private void OnDrawGizmosSelected()
    {
        if (debugFullPath == null || debugFullPath.Count == 0)
            return;

        Gizmos.color = Color.magenta;

        for (int i = 0; i < debugFullPath.Count; i++)
        {
            GraphNode node = debugFullPath[i];

            if (node == null)
                continue;

            Gizmos.DrawSphere(node.Position, 0.12f);

            if (i < debugFullPath.Count - 1 && debugFullPath[i + 1] != null)
            {
                Gizmos.DrawLine(
                    node.Position,
                    debugFullPath[i + 1].Position
                );
            }
        }

        if (CurrentTargetNode != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, CurrentTargetNode.Position);
            Gizmos.DrawSphere(CurrentTargetNode.Position, 0.18f);
        }
    }*/

    public bool HasBlockedEdgeInRemainPath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return false;

        for (int i = Mathf.Max(currentIndex - 1, 0); i < currentPath.Count - 1; i++)
        {
            GraphNode from = currentPath[i];
            GraphNode to = currentPath[i + 1];

            if (from == null || to == null)
                return true;

            if (from.IsBlocked || to.IsBlocked)
                return true;

            // Проверка, существует ли дорога между нодами сейчас
            if (!from.HasAvailableConnectionTo(to))
                return true;
        }

        return false;
    }

    private bool IsCurrentStepBlocked()
    {
        GraphNode targetNode = CurrentTargetNode;

        if (targetNode == null || targetNode.IsBlocked)
            return true;

        if (currentIndex > 0)
        {
            GraphNode fromNode = currentPath[currentIndex - 1];

            if (fromNode == null || fromNode.IsBlocked)
                return true;

            if (!fromNode.HasAvailableConnectionTo(targetNode))
                return true;
        }

        return false;
    }
}