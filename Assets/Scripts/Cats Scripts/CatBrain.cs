using System.Collections.Generic;
using UnityEngine;


// Мозг кошки
[RequireComponent(typeof(CatMover))]
[RequireComponent(typeof(CatNeeds))]
public class CatBrain : MonoBehaviour
{
    [Header("References")]
    public GraphManager GraphManager; // Ссылка на граф

    [Header("State")]
    public CatState CurrentState = CatState.Idle; // Текущее состояние кошки

    [Header("Thresholds")]
    [Range(0f, 100f)] public float HungerThreshold = 70f;
    [Range(0f, 100f)] public float SleepThreshold = 75f;
    [Range(0f, 100f)] public float DangerEscapeThreshold = 7f;

    [Header("Pathfinding")]
    //[Range(0f, 5f)] public float DangerWeight = 2f;
    public float RepathInterval = 2f;

    [Header("Wander")]
    public float MinIdleTime = 1f;
    public float MaxIdleTime = 3f;

    [Header("Action values")]
    public float EatAmount = 60f;
    public float SleepAmount = 50f;

    private CatMover mover;
    private CatNeeds needs;
    private PathFinderAStar pathfinder;

    private GraphNode currentGoal; // текущая цель
    private float repathTimer;
    private float idleTimer;

    private void Awake()
    {
        mover = GetComponent<CatMover>();
        needs = GetComponent<CatNeeds>();
        pathfinder = new PathFinderAStar(); // создаёт поиск пути
    }

    private void OnEnable()
    {
        mover.OnPathCompleted += HandlePathCompleted; // кошка дошла до маршрута
        mover.OnPathInvalid += HandlePathInvalid;
    }

    private void OnDisable()
    {
        mover.OnPathCompleted -= HandlePathCompleted;
        mover.OnPathInvalid -= HandlePathInvalid;
    }

    private void Start()
    {
        if (GraphManager == null)
            GraphManager = FindObjectOfType<GraphManager>(); // поиск менеджера графа

        ChangeState(CatState.Wander); // начальное состояния кошки
    }

    private void Update()
    {
        if (GraphManager == null)
            return;

        if (needs != null && needs.IsDead)
            return;

        // уменьшение таймеров
        repathTimer -= Time.deltaTime;
        idleTimer -= Time.deltaTime;

        // проверка на смену состояни
        EvaluateState();

        // перепрокладка пути если нужно
        if (ShouldRepath())
        {
            //Debug.Log("Перепрокладка пути");
            BuildPathForCurrentState();
            repathTimer = RepathInterval;
        }

        // новый случайный маршрут если кошка стоит без дела
        if (!mover.IsMoving && CurrentState == CatState.Wander && idleTimer <= 0f)
        {
            BuildPathForCurrentState();
            idleTimer = Random.Range(MinIdleTime, MaxIdleTime);
        }
    }

    private void EvaluateState()
    {
        GraphNode nearestNode = GraphManager.GetClosestNode(transform.position);

        bool inDanger = nearestNode != null && nearestNode.DangerLevel >= DangerEscapeThreshold;
        bool hungry = needs.hunger >= HungerThreshold;
        bool sleepy = needs.sleepiness >= SleepThreshold;

        CatState desiredState;

        if (inDanger)
        {
            desiredState = CatState.EscapeDanger;
        }
        else if (hungry && sleepy)
        {
            float hungerPriority = needs.hunger - HungerThreshold;
            float sleepPriority = needs.sleepiness - SleepThreshold;

            desiredState = hungerPriority >= sleepPriority
                ? CatState.GoToFood
                : CatState.GoToSleep;
        }
        else if (hungry)
        {
            desiredState = CatState.GoToFood;
        }
        else if (sleepy)
        {
            desiredState = CatState.GoToSleep;
        }
        else
        {
            desiredState = CatState.Wander;
        }

        if (CurrentState != desiredState)
            ChangeState(desiredState);
    }

    // смена состояния и постройка маршрута под него
    private void ChangeState(CatState newState)
    {
        CurrentState = newState;

        GraphNode startNode = GraphManager.GetClosestNode(transform.position);
        if (startNode == null)
            return;

        switch (CurrentState)
        {
            case CatState.GoToFood:
                currentGoal = FindBestNodeOfType(startNode, NodeType.Food);
                break;

            case CatState.GoToSleep:
                currentGoal = FindBestNodeOfType(startNode, NodeType.SleepSpot);
                break;

            case CatState.Wander:
                currentGoal = FindRandomReachableNode(startNode);
                break;

            case CatState.Idle:
                currentGoal = null;
                mover.StopMoving();
                return;
        }

        BuildPathToCurrentGoal();
        repathTimer = RepathInterval;
    }

    private void BuildPathToCurrentGoal()
    {
        if (currentGoal == null)
        {
            mover.StopMoving();
            return;
        }

        GraphNode startNode = GraphManager.GetClosestNode(transform.position);
        if (startNode == null)
        {
            mover.StopMoving();
            return;
        }

        List<GraphNode> path = pathfinder.FindPath(startNode, currentGoal);

        if (path != null && path.Count > 0)
            mover.SetPath(path);
        else
            mover.StopMoving();
    }

    // решает, нужно ли перепроложить путь
    private bool ShouldRepath()
    {
        if (repathTimer > 0f)
            return false;

        if (currentGoal == null || currentGoal.IsBlocked)
            return true;

        if (!mover.IsMoving)
            return true;

        if (mover.HasBlockedEdgeInRemainPath())
            return true;

        return CurrentState == CatState.EscapeDanger;
    }

    // построение пути для текущего состояния
    private void BuildPathForCurrentState()
    {
        //GraphNode startNode = GraphManager.GetClosestNode(transform.position);
        GraphNode startNode = GetRepathStartNode();

        if (startNode == null)
        {
            mover.StopMoving();
            return;
        }

        if (CurrentState == CatState.GoToFood && startNode.Type == NodeType.Food)
        {
            needs.Eat(EatAmount);

            if (startNode.CurrentResource != null)
                startNode.CurrentResource.Use();

            ChangeState(CatState.Wander);
            return;
        }

        if (CurrentState == CatState.GoToSleep && startNode.Type == NodeType.SleepSpot)
        {
            needs.Sleep(SleepAmount);

            if (startNode.CurrentResource != null)
                startNode.CurrentResource.Use();

            ChangeState(CatState.Wander);
            return;
        }

        // поиск лучшего узла 
        switch (CurrentState)
        {
            case CatState.GoToFood:
                currentGoal = FindBestNodeOfType(startNode, NodeType.Food);
                break;

            case CatState.GoToSleep:
                currentGoal = FindBestNodeOfType(startNode, NodeType.SleepSpot);
                break;

            case CatState.EscapeDanger:
                {
                    GraphNode shelter = FindBestNodeOfType(startNode, NodeType.Shelter);
                    currentGoal = shelter != null ? shelter : GraphManager.GetSafestNode(transform.position);
                    break;
                }

            case CatState.Wander:
                currentGoal = FindRandomReachableNode(startNode);
                break;

            case CatState.Idle:
                mover.StopMoving();
                return;
        }

        if (currentGoal == null || currentGoal.IsBlocked)
        {
            mover.StopMoving();
            return;
        }

        List<GraphNode> path = pathfinder.FindPath(startNode, currentGoal);

        if (path != null && path.Count > 0)
        {
            //Debug.Log($"Новый путь найден от {startNode.Id} до {currentGoal.Id}. Длина: {path.Count}");
            mover.SetPath(path);
        }
        else
        {
            //Debug.LogWarning($"Путь НЕ найден от {startNode.Id} до {currentGoal.Id}");
            mover.StopMoving();
        }      
    }

    // подбор лучшего узла
    private GraphNode FindBestNodeOfType(GraphNode startNode, NodeType type)
    {
        List<GraphNode> candidates = GraphManager.GetNodesByType(type);

        GraphNode best = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            GraphNode candidate = candidates[i];

            if (candidate == null || candidate == startNode)
                continue;

            List<GraphNode> path = pathfinder.FindPath(startNode, candidate);
            if (path == null || path.Count == 0)
                continue;

            float score = CalculatePathScore(path);
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private GraphNode GetRepathStartNode()
    {
        if (mover.LastReachedNode != null && !mover.LastReachedNode.IsBlocked)
            return mover.LastReachedNode;

        GraphNode closestNode = GraphManager.GetClosestNode(transform.position);

        if (closestNode != null && !closestNode.IsBlocked)
            return closestNode;

        return null;
    }

    // поиск случайного узла для блуждания
    private GraphNode FindRandomReachableNode(GraphNode startNode)
    {
        List<GraphNode> allNodes = GraphManager.Nodes;
        List<GraphNode> candidates = new List<GraphNode>();

        for (int i = 0; i < allNodes.Count; i++)
        {
            GraphNode node = allNodes[i];
            if (node == null || node.IsBlocked || node == startNode)
                continue;

            if (node.DangerLevel > DangerEscapeThreshold)
                continue;

            candidates.Add(node);
        }

        if (candidates.Count == 0)
            return null;

        for (int attempt = 0; attempt < 12; attempt++)
        {
            GraphNode randomNode = candidates[Random.Range(0, candidates.Count)];
            List<GraphNode> path = pathfinder.FindPath(startNode, randomNode);
            if (path != null && path.Count > 0)
                return randomNode;
        }

        return null;
    }

    // суммирование стоимости переходов по маршруту
    private float CalculatePathScore(List<GraphNode> path)
    {
        float score = 0f;

        for (int i = 0; i < path.Count - 1; i++)
        {
            GraphNode from = path[i];
            GraphNode to = path[i + 1];

            score += from.GetCostTo(to);
        }

        return score;
    }

    // вызывается при завершении пути кошкой
    private void HandlePathCompleted()
    {
        if (currentGoal == null)
            return;

        // выполнение действий по состояниям
        switch (CurrentState)
        {
            case CatState.GoToFood:
                if (currentGoal.Type == NodeType.Food)
                {
                    needs.Eat(EatAmount);

                    if (currentGoal.CurrentResource != null)
                        currentGoal.CurrentResource.Use();

                    ChangeState(CatState.Wander);
                }
                break;

            case CatState.GoToSleep:
                if (currentGoal.Type == NodeType.SleepSpot)
                {
                    needs.Sleep(SleepAmount);

                    if (currentGoal.CurrentResource != null)
                        currentGoal.CurrentResource.Use();

                    ChangeState(CatState.Wander);
                }
                break;

            case CatState.EscapeDanger:
                ChangeState(CatState.Wander);
                break;

            case CatState.Wander:
                idleTimer = Random.Range(MinIdleTime, MaxIdleTime);
                break;
        }
    }

    private void HandlePathInvalid()
    {
        //Debug.Log("Маршрут стал недоступен, перепрокладываю путь");

        repathTimer = 0f;
        BuildPathForCurrentState();
    }
}