using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePlacementHighlight : MonoBehaviour
{
    [SerializeField] private GameObject visualObject;
    [SerializeField] private Renderer visualRenderer;

    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;

    [SerializeField] private float blinkInterval = 0.4f;

    private bool isActive;
    private bool transparentState;
    private float timer;

    private void Awake()
    {
        if (visualRenderer == null && visualObject != null)
            visualRenderer = visualObject.GetComponent<Renderer>();

        SetVisible(false);
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer += Time.deltaTime;

        if (timer >= blinkInterval)
        {
            timer = 0f;
            transparentState = !transparentState;

            if (visualRenderer != null)
            {
                visualRenderer.material = transparentState
                    ? transparentMaterial
                    : opaqueMaterial;
            }
        }
    }

    public void SetVisible(bool visible)
    {
        isActive = visible;
        timer = 0f;
        transparentState = false;

        if (visualObject != null)
            visualObject.SetActive(visible);

        if (visualRenderer != null && opaqueMaterial != null)
            visualRenderer.material = opaqueMaterial;
    }
}
