using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatDeathController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CatNeeds catNeeds;
    [SerializeField] private CatMover catMover;
    [SerializeField] private Collider catCollider;

    [SerializeField] private string deathTriggerName = "Die";
    [SerializeField] private float destroyDelay = 1f;

    private bool isDying;

    private void Awake()
    {
        if (catNeeds == null)
            catNeeds = GetComponent<CatNeeds>();

        if (catMover == null)
            catMover = GetComponent<CatMover>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (catCollider == null)
            catCollider = GetComponentInChildren<Collider>();
    }

    private void OnEnable()
    {
        if (catNeeds != null)
            catNeeds.OnCatDied += HandleCatDied;
    }

    private void OnDisable()
    {
        if (catNeeds != null)
            catNeeds.OnCatDied -= HandleCatDied;
    }

    private void HandleCatDied(CatNeeds deadCat)
    {
        if (isDying)
            return;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;

        if (catMover != null)
            catMover.StopMoving();

        if (catCollider != null)
            catCollider.enabled = false;

        if (animator != null)
            animator.SetTrigger(deathTriggerName);

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}
