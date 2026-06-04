using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GraphNodeSelectionManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask nodeLayerMask;
    [SerializeField] private Button blockButton;

    private GraphNode selectedNode;

    public GraphNode SelectedNode => selectedNode;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        UpdateBlockButton();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        TrySelectNode();
    }

    private void TrySelectNode()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, nodeLayerMask))
        {
            ClearSelection();
            return;
        }

        GraphNode node = hit.collider.GetComponentInParent<GraphNode>();

        if (node == null)
        {
            ClearSelection();
            return;
        }

        SelectNode(node);
    }

    public void SelectNode(GraphNode node)
    {
        if (selectedNode == node)
            return;

        ClearSelection();

        selectedNode = node;

        GraphNodeHighlight highlight =
            selectedNode.GetComponent<GraphNodeHighlight>();

        if (highlight != null)
            highlight.SetSelected(true);

        UpdateBlockButton();
    }

    public void ClearSelection()
    {
        if (selectedNode != null)
        {
            GraphNodeHighlight highlight =
                selectedNode.GetComponent<GraphNodeHighlight>();

            if (highlight != null)
                highlight.SetSelected(false);
        }

        selectedNode = null;

        UpdateBlockButton();
    }

    public void ToggleSelectedNodeBlock()
    {
        if (selectedNode == null)
        {
            Debug.Log("Нода не выбрана");
            return;
        }

        NodeBlocker blocker = selectedNode.GetComponent<NodeBlocker>();

        if (blocker == null)
        {
            Debug.LogWarning("На выбранной ноде нет NodeBlocker");
            return;
        }

        blocker.ToggleBlocked();
    }

    private void UpdateBlockButton()
    {
        if (blockButton != null)
            blockButton.interactable = selectedNode != null;
    }
}
