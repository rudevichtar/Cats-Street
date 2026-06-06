using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBlocker : MonoBehaviour
{
    [SerializeField] private GameObject blockVisual;

    [Header("Auto Unlock")]
    [SerializeField] private float blockDuration = 5f;

    private GraphNode node;
    private Coroutine unblockCoroutine;

    private void Awake()
    {
        node = GetComponent<GraphNode>();

        if (blockVisual != null)
            blockVisual.SetActive(false);
    }

    public bool SetBlocked(bool blocked)
    {
        if (node == null)
            return false;

        bool wasBlocked = node.IsBlocked;

        if (blocked && !wasBlocked)
        {
            if (BlockLimitManager.Instance != null)
            {
                if (!BlockLimitManager.Instance.CanBlockNode())
                {
                    float timeLeft = BlockLimitManager.Instance.GetFirstNodeRemainingTime();

                    if (PopupHint.Instance != null)
                    {
                        PopupHint.Instance.Show(
                            $"Лимит закрытых точек. Осталось {Mathf.CeilToInt(timeLeft)} сек."
                        );
                    }

                    return false;
                }

                BlockLimitManager.Instance.RegisterNodeBlock(blockDuration);
            }
        }

        if (!blocked && wasBlocked)
        {
            if (BlockLimitManager.Instance != null)
                BlockLimitManager.Instance.UnregisterNodeBlock();
        }

        node.IsBlocked = blocked;

        if (blockVisual != null)
            blockVisual.SetActive(blocked);

        if (unblockCoroutine != null)
        {
            StopCoroutine(unblockCoroutine);
            unblockCoroutine = null;
        }

        if (blocked)
            unblockCoroutine = StartCoroutine(AutoUnblock());

        return true;
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
