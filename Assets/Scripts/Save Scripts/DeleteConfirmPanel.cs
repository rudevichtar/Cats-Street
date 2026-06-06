using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteConfirmPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private int slotToDelete;
    private SaveSlotUI slotUIToRefresh;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Open(int slot, SaveSlotUI slotUI)
    {
        slotToDelete = slot;
        slotUIToRefresh = slotUI;

        if (panel != null)
            panel.SetActive(true);
    }

    public void ConfirmDelete()
    {
        SaveSystem.DeleteSave(slotToDelete);

        if (slotUIToRefresh != null)
            slotUIToRefresh.Refresh();

        Close();
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
