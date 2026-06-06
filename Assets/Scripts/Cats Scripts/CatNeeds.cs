using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Параметры (хотелки) кошки
public class CatNeeds : MonoBehaviour
{
    public event Action<CatNeeds> OnCatDied;

    [Header("Needs")]
    [Range(0f, 100f)] public float hunger = 20f;
    [Range(0f, 100f)] public float sleepiness = 10f;
    [Range(0f, 100f)] public float happiness = 80f;
    [Range(0f, 100f)] public float health = 100f;

    [Header("Change per second")]
    [SerializeField] private float hungerIncreasePerSecond = 0.35f;
    [SerializeField] private float sleepinessIncreasePerSecond = 0.25f;
    [SerializeField] private float happinessDecreasePerSecond = 0.12f;

    [Header("Health penalties")]
    [SerializeField] private float criticalHungerThreshold = 95f;
    [SerializeField] private float criticalSleepThreshold = 95f;
    [SerializeField] private float lowHappinessThreshold = 10f;

    [SerializeField] private float hungerHealthDamagePerSecond = 0.25f;
    [SerializeField] private float sleepHealthDamagePerSecond = 0.15f;
    [SerializeField] private float sadnessHealthDamagePerSecond = 0.08f;

    public bool IsDead { get; private set; }

    private void Update()
    {
        if (IsDead)
            return;

        hunger = Mathf.Clamp(
            hunger + hungerIncreasePerSecond * Time.deltaTime,
            0f,
            100f
        );

        sleepiness = Mathf.Clamp(
            sleepiness + sleepinessIncreasePerSecond * Time.deltaTime,
            0f,
            100f
        );

        happiness = Mathf.Clamp(
            happiness - happinessDecreasePerSecond * Time.deltaTime,
            0f,
            100f
        );

        ApplyHealthPenalties();
    }

    private void ApplyHealthPenalties()
    {
        float damage = 0f;

        if (hunger >= criticalHungerThreshold)
            damage += hungerHealthDamagePerSecond;

        if (sleepiness >= criticalSleepThreshold)
            damage += sleepHealthDamagePerSecond;

        if (happiness <= lowHappinessThreshold)
            damage += sadnessHealthDamagePerSecond;

        if (damage > 0f)
            Damage(damage * Time.deltaTime);
    }

    public void Eat(float amount)
    {
        if (IsDead)
            return;

        hunger = Mathf.Clamp(hunger - amount, 0f, 100f);
        happiness = Mathf.Clamp(happiness + amount * 0.2f, 0f, 100f);
    }

    public void Sleep(float amount)
    {
        if (IsDead)
            return;

        sleepiness = Mathf.Clamp(sleepiness - amount, 0f, 100f);
        health = Mathf.Clamp(health + amount * 0.05f, 0f, 100f);
        happiness = Mathf.Clamp(happiness + amount * 0.1f, 0f, 100f);
    }

    public void Scare(float amount)
    {
        if (IsDead)
            return;

        happiness = Mathf.Clamp(happiness - amount, 0f, 100f);
    }

    public void Damage(float amount)
    {
        if (IsDead)
            return;

        health = Mathf.Clamp(health - amount, 0f, 100f);

        if (health <= 0f)
            Kill();
    }

    public void Kill()
    {
        if (IsDead)
            return;

        health = 0f;
        IsDead = true;

        OnCatDied?.Invoke(this);
    }
}
