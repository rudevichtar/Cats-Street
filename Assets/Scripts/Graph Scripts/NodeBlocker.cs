using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBlocker : MonoBehaviour
{
    [SerializeField] private GameObject blockVisual;

    [Header("Auto Unlock")]
    [SerializeField] private float blockDuration = 10f;

    private GraphNode node;
    private Coroutine unblockCoroutine;

    private void Awake()
    {
        node = GetComponent<GraphNode>();

        if (blockVisual != null)
            blockVisual.SetActive(false);
    }

    public void SetBlocked(bool blocked)
    {
        if (node == null)
            return;

        node.IsBlocked = blocked;

        if (blockVisual != null)
            blockVisual.SetActive(blocked);

        if (blocked)
        {
            if (unblockCoroutine != null)
                StopCoroutine(unblockCoroutine);

            unblockCoroutine = StartCoroutine(AutoUnblock());
        }
        else
        {
            if (unblockCoroutine != null)
            {
                StopCoroutine(unblockCoroutine);
                unblockCoroutine = null;
            }
        }
    }

    public void ToggleBlocked()
    {
        SetBlocked(!node.IsBlocked);
    }

    private IEnumerator AutoUnblock()
    {
        yield return new WaitForSeconds(blockDuration);

        SetBlocked(false);

        unblockCoroutine = null;
    }
}
