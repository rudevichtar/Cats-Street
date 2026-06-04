using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Параметры (хотелки) кошки
public class CatNeeds : MonoBehaviour
{
    [Range(0f, 100f)] public float Hunger = 20f;
    [Range(0f, 100f)] public float Sleepiness = 10f;
    [Range(0f, 100f)] public float Happiness = 80f;
    [Range(0f, 100f)] public float Health = 100f;

    [Header("Change per second")]
    public float HungerIncreasePerSecond = 2f;
    public float SleepinessIncreasePerSecond = 1.5f;
    public float HappinessDecreasePerSecond = 0.5f;

    public bool IsDead { get; private set; }

    public event Action<CatNeeds> OnCatDied;

    private void Update()
    {
        if (IsDead) return;

        Hunger = Mathf.Clamp(Hunger + HungerIncreasePerSecond * Time.deltaTime, 0f, 100f);
        Sleepiness = Mathf.Clamp(Sleepiness + SleepinessIncreasePerSecond * Time.deltaTime, 0f, 100f);
        Happiness = Mathf.Clamp(Happiness - HappinessDecreasePerSecond * Time.deltaTime, 0f, 100f);

        if (Health <= 0f) Die();
    }

    public void Kill()
    {
        if (IsDead)
            return;

        Health = 0f;
        Die();
    }

    public void Damage(float amount)
    {
        if (IsDead) return;

        Health = Mathf.Clamp(Health - amount, 0f, 100f);

        if (Health <= 0f) Die();
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        OnCatDied?.Invoke(this);

        //Destroy(gameObject);
    }

    public void Eat(float amount)
    {
        Hunger = Mathf.Clamp(Hunger - amount, 0f, 100f);
        Happiness = Mathf.Clamp(Happiness + amount * 0.2f, 0f, 100f);
    }

    public void Sleep(float amount)
    {
        Sleepiness = Mathf.Clamp(Sleepiness - amount, 0f, 100f);
        Health = Mathf.Clamp(Health + amount * 0.05f, 0f, 100f);
    }

    public void Scare(float amount)
    {
        Happiness = Mathf.Clamp(Happiness - amount, 0f, 100f);
    }
}
