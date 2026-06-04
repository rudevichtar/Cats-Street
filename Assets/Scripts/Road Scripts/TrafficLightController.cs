using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public enum SignalState
    {
        Green,
        RedBlinking
    }

    [Header("Visuals")]
    [SerializeField] private GameObject greenVisual;
    [SerializeField] private GameObject redVisual;

    [Header("Blink")]
    [SerializeField] private float blinkInterval = 0.35f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blinkingRedClip;

    [Header("Debug")]
    [SerializeField] private SignalState currentState = SignalState.Green;

    private float blinkTimer;
    private bool redEnabled = true;

    public SignalState CurrentState => currentState;

    private void Start()
    {
        ApplyVisuals(forceRedVisible: false);
    }

    private void Update()
    {
        if (currentState != SignalState.RedBlinking)
            return;

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            redEnabled = !redEnabled;
            ApplyVisuals(forceRedVisible: redEnabled);
        }
    }

    public void SetGreen()
    {
        if (currentState == SignalState.Green)
            return;

        currentState = SignalState.Green;
        blinkTimer = 0f;
        redEnabled = false;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        ApplyVisuals(forceRedVisible: false);
    }

    public void SetRedBlinking()
    {
        if (currentState == SignalState.RedBlinking)
            return;

        currentState = SignalState.RedBlinking;
        blinkTimer = 0f;
        redEnabled = true;

        if (audioSource != null && blinkingRedClip != null)
        {
            audioSource.clip = blinkingRedClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        ApplyVisuals(forceRedVisible: true);
    }

    private void ApplyVisuals(bool forceRedVisible)
    {
        if (greenVisual != null)
            greenVisual.SetActive(currentState == SignalState.Green);

        if (redVisual != null)
            redVisual.SetActive(currentState == SignalState.RedBlinking && forceRedVisible);
    }
}
