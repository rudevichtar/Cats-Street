using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Переключатель для открытия/закрытия дороги
[Serializable]
public class RoadSwitch : MonoBehaviour
{
    [Header("Road")]
    public string DisplayName = "Дорога";
    public GraphNode From;
    public GraphNode To;

    [Header("Visual")]
    [SerializeField] private RoadHighlight highlight;

    private void Awake()
    {
        if (highlight == null)
            highlight = GetComponent<RoadHighlight>();
    }

    private void OnMouseDown()
    {
        RoadSelectionManager.Instance.SelectRoad(this);
    }

    public void SetSelected(bool selected)
    {
        if (highlight != null)
            highlight.SetSelected(selected);
    }

    public void Toggle()
    {
        RoadBlockSystem.Instance.ToggleRoad(From, To);
    }

    public bool IsBlocked()
    {
        if (From == null || To == null)
            return false;

        GraphConnection connection = From.Connections.Find(c => c.Target == To);
        return connection != null && connection.IsBlocked;
    }
}
