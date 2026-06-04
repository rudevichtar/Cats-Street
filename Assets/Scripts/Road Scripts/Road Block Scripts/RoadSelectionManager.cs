using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Менеджер выбора переходов для закрытия дороги игроком
public class RoadSelectionManager : MonoBehaviour
{
    public static RoadSelectionManager Instance { get; private set; }

    public RoadSwitch SelectedRoad { get; private set; }

    public event Action<RoadSwitch> OnRoadSelected;

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            RoadSwitch road = hit.collider.GetComponentInParent<RoadSwitch>();

            if (road != null)
                return;
        }

        ClearSelection();
    }

    public void SelectRoad(RoadSwitch road)
    {
        if (SelectedRoad == road)
            return;

        if (SelectedRoad != null)
            SelectedRoad.SetSelected(false);

        SelectedRoad = road;

        if (SelectedRoad != null)
            SelectedRoad.SetSelected(true);

        OnRoadSelected?.Invoke(SelectedRoad);
    }

    public void ClearSelection()
    {
        if (SelectedRoad != null)
            SelectedRoad.SetSelected(false);

        SelectedRoad = null;
        OnRoadSelected?.Invoke(null);
    }
}
