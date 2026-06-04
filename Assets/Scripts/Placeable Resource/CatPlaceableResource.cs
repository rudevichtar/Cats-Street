using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatPlaceableResource : MonoBehaviour
{
    [SerializeField] private CatResourceType resourceType;
    [SerializeField] private int maxUses = 3;

    private int usesLeft;
    private GraphNode node;
    private NodeType originalType;

    public CatResourceType ResourceType => resourceType;
    public int UsesLeft => usesLeft;

    public void Init(GraphNode targetNode, NodeType newType, int uses)
    {
        node = targetNode;
        originalType = targetNode.Type;

        usesLeft = uses;
        maxUses = uses;

        node.Type = newType;
    }

    public void InitLoaded(GraphNode targetNode, NodeType newType, int loadedUsesLeft)
    {
        node = targetNode;
        originalType = NodeType.CatsZone;

        usesLeft = loadedUsesLeft;
        node.Type = newType;
    }

    public void Use()
    {
        usesLeft--;

        if (usesLeft <= 0)
            Remove();
    }

    private void Remove()
    {
        if (node != null)
        {
            node.Type = originalType;
            node.ClearResource(this);
        }

        Destroy(gameObject);
    }
}
