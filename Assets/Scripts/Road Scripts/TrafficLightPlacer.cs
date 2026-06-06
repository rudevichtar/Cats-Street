using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightPlacer : MonoBehaviour
{
    public static event Action<bool> OnPlacementModeChanged;

    [Header("Placement")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask slotLayerMask;
    [SerializeField] private TrafficLightController trafficLightPrefab;
    private int trafficLightCost = 35;

    [Header("Input")]
    [SerializeField] private bool placementModeEnabled;

    public void SetPlacementMode(bool enabled)
    {
        placementModeEnabled = enabled;
        Debug.Log("Можно размещать");
        OnPlacementModeChanged?.Invoke(enabled);
    }

    private void Update()
    {
        if (!placementModeEnabled)
            return;

        if (Input.GetMouseButtonDown(0))
            TryPlaceFromMouse();
    }

    private void TryPlaceFromMouse()
    {
        if (mainCamera == null || trafficLightPrefab == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, slotLayerMask))
            return;

        TrafficLightSlot slot = hit.collider.GetComponent<TrafficLightSlot>();
        if (slot == null || !slot.CanPlace())
            return;

        if (!CoinWallet.Instance.TrySpend(trafficLightCost))
        {
            if (PopupHint.Instance != null)
                PopupHint.Instance.Show("Недостаточно монет");

            return;
        }

        TrafficLightController light = Instantiate(
            trafficLightPrefab,
            slot.transform.position,
            slot.transform.rotation);

        slot.Crosswalk.AttachTrafficLight(light);
        slot.MarkOccupied();

        SetPlacementMode(false);
        Debug.Log("Светофор установлен и режим размещения выключен");
    }
}
