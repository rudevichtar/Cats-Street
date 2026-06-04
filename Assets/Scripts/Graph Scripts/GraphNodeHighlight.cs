using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNodeHighlight : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject placementObject;

    [Header("Blink")]
    [SerializeField] private Renderer placementRenderer;
    /*[SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;*/
    [SerializeField] private float blinkInterval = 0.4f;

    private bool placementActive;
    private bool transparentState;
    private float blinkTimer;

    private void Awake()
    {
        if (placementRenderer == null && placementObject != null)
            placementRenderer = placementObject.GetComponent<Renderer>();

        SetSelected(false);
        SetPlacement(false);
    }

    private void Update()
    {
        if (!placementActive)
            return;

        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            transparentState = !transparentState;

            if (placementRenderer != null)
            {
                Color color = placementRenderer.material.color;

                color.a = transparentState ? 0.2f : 1f;

                placementRenderer.material.color = color;
            }
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectedObject != null)
            selectedObject.SetActive(selected);
    }

    public void SetPlacement(bool active)
    {
        placementActive = active;
        blinkTimer = 0f;
        transparentState = false;

        if (placementObject != null)
            placementObject.SetActive(active);

        if (placementRenderer != null)
        {
            Color color = placementRenderer.material.color;
            color.a = 1f;
            placementRenderer.material.color = color;
        }
    }
}
