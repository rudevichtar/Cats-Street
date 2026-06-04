using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CatResourceType
{
    Food,
    Bed
}

public class CatResourcePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private LayerMask nodeLayerMask;

    [Header("Prefabs")]
    [SerializeField] private CatPlaceableResource foodPrefab;
    [SerializeField] private CatPlaceableResource bedPrefab;

    [Header("Settings")]
    [SerializeField] private int usesPerResource = 3;
    [SerializeField] private float cooldown = 60f;

    private bool placementMode;
    private CatResourceType selectedType;

    private float foodCooldownTimer;
    private float bedCooldownTimer;

    private void Start()
    {
        HidePlacementHighlights();
    }

    private void Update()
    {
        if (foodCooldownTimer > 0f)
            foodCooldownTimer -= Time.deltaTime;

        if (bedCooldownTimer > 0f)
            bedCooldownTimer -= Time.deltaTime;

        if (!placementMode)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
            return;
        }

        if (Input.GetMouseButtonDown(0))
            TryPlaceFromMouse();
    }

    public void StartPlacingFood()
    {
        Debug.Log("Нажата кнопка размещения еды");

        StartPlacement(CatResourceType.Food);
    }

    public void StartPlacingBed()
    {
        Debug.Log("Нажата кнопка размещения лежанки");

        StartPlacement(CatResourceType.Bed);
    }

    private void StartPlacement(CatResourceType type)
    {
        Debug.Log($"StartPlacement: {type}");

        selectedType = type;

        if (!CanPlaceSelectedType())
        {
            Debug.Log("Нельзя поставить: cooldown");

            float timeLeft = selectedType == CatResourceType.Food
                ? foodCooldownTimer
                : bedCooldownTimer;

            Debug.Log($"Нельзя поставить. Осталось ждать: {timeLeft:F1} сек.");
            return;
        }

        placementMode = true;

        Debug.Log("Включаю подсветку");
        ShowPlacementHighlights();
    }

    private void TryPlaceFromMouse()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, nodeLayerMask))
            return;

        GraphNode node = hit.collider.GetComponentInParent<GraphNode>();

        if (node == null)
            return;

        if (!CanPlaceOnNode(node))
        {
            Debug.Log("На эту ноду нельзя поставить объект");
            return;
        }

        PlaceResource(node);
    }

    private bool CanPlaceOnNode(GraphNode node)
    {
        if (node == null)
            return false;

        if (node.Type != NodeType.CatsZone)
            return false;

        if (node.IsBlocked)
            return false;

        if (node.HasResource)
            return false;

        return true;
    }

    private void PlaceResource(GraphNode node)
    {
        CatPlaceableResource prefab = selectedType == CatResourceType.Food
            ? foodPrefab
            : bedPrefab;

        NodeType newNodeType = selectedType == CatResourceType.Food
            ? NodeType.Food
            : NodeType.SleepSpot;

        if (prefab == null)
            return;

        CatPlaceableResource resource = Instantiate(
            prefab,
            node.Position,
            Quaternion.identity
        );

        resource.Init(node, newNodeType, usesPerResource);
        node.SetResource(resource);

        if (selectedType == CatResourceType.Food)
            foodCooldownTimer = cooldown;
        else
            bedCooldownTimer = cooldown;

        placementMode = false;
        HidePlacementHighlights();
    }

    private void CancelPlacement()
    {
        placementMode = false;
        HidePlacementHighlights();
        Debug.Log("Размещение отменено");
    }

    private bool CanPlaceSelectedType()
    {
        return selectedType == CatResourceType.Food
            ? foodCooldownTimer <= 0f
            : bedCooldownTimer <= 0f;
    }

    private void ShowPlacementHighlights()
    {
        Debug.Log("ShowPlacementHighlights вызван");

        if (graphManager == null)
        {
            Debug.LogError("GraphManager не назначен");
            return;
        }

        Debug.Log($"Нод в GraphManager: {graphManager.Nodes.Count}");

        foreach (GraphNode node in graphManager.Nodes)
        {
            if (node == null)
                continue;

            GraphNodeHighlight highlight = node.GetComponent<GraphNodeHighlight>();

            if (highlight == null)
            {
                Debug.LogWarning($"У ноды {node.name} нет GraphNodeHighlight");
                continue;
            }

            bool canPlace = CanPlaceOnNode(node);
            Debug.Log($"{node.name}: canPlace = {canPlace}, type = {node.Type}");

            highlight.SetPlacement(canPlace);

            /*if (highlight != null)
                highlight.SetPlacement(CanPlaceOnNode(node));*/
        }
    }

    private void HidePlacementHighlights()
    {
        if (graphManager == null)
            return;

        foreach (GraphNode node in graphManager.Nodes)
        {
            if (node == null)
                continue;

            GraphNodeHighlight highlight = node.GetComponent<GraphNodeHighlight>();

            if (highlight != null)
                highlight.SetPlacement(false);
        }
    }
}
