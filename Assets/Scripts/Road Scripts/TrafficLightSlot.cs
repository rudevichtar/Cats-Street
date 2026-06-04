using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightSlot : MonoBehaviour
{
    [SerializeField] private string slotId;
    [SerializeField] private CrosswalkController crosswalk;
    [SerializeField] private bool occupied;

    [Header("Visual")]
    [SerializeField] private GameObject visualObject;
    [SerializeField] private Renderer slotRenderer;
    [SerializeField] private Collider slotCollider;

    [Header("Blink Materials")]
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private float blinkInterval = 0.4f;

    private bool placementModeActive;
    private float blinkTimer;
    private bool transparentState;

    public string SlotId => slotId;
    public CrosswalkController Crosswalk => crosswalk;
    public bool IsOccupied => occupied;

    private void Awake()
    {
        if (slotRenderer == null)
            slotRenderer = GetComponentInChildren<Renderer>();

        if (slotCollider == null)
            slotCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        TrafficLightPlacer.OnPlacementModeChanged += SetPlacementModeVisual;
        SetPlacementModeVisual(false);
    }

    private void OnDisable()
    {
        TrafficLightPlacer.OnPlacementModeChanged -= SetPlacementModeVisual;
    }

    private void Update()
    {
        if (!placementModeActive || occupied)
            return;

        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            transparentState = !transparentState;

            if (slotRenderer != null)
            {
                slotRenderer.material = transparentState
                    ? transparentMaterial
                    : opaqueMaterial;
            }
        }
    }

    public bool CanPlace()
    {
        return !occupied && crosswalk != null && crosswalk.CanAttachTrafficLight();
    }

    public void MarkOccupied()
    {
        occupied = true;
        SetPlacementModeVisual(false);
    }

    private void SetPlacementModeVisual(bool active)
    {
        placementModeActive = active && !occupied;

        if (visualObject != null)
            visualObject.SetActive(placementModeActive);

        if (slotCollider != null)
            slotCollider.enabled = placementModeActive;

        blinkTimer = 0f;
        transparentState = false;

        if (slotRenderer != null && opaqueMaterial != null)
            slotRenderer.material = opaqueMaterial;
    }

    public void ResetSlot()
    {
        occupied = false;
    }
}
