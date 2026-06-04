using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatProfile : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private string catName = "Кошка";

    [TextArea]
    [SerializeField] private string description;

    [Header("Visual")]
    [SerializeField] private Sprite catIcon;

    [SerializeField] private Color cardColor = Color.white;

    [Header("Stats")]
    [SerializeField] private float moveSpeedMultiplier = 1f;

    [SerializeField] private float hungerMultiplier = 1f;

    [SerializeField] private float sleepMultiplier = 1f;

    [SerializeField] private float happinessMultiplier = 1f;

    public string CatName
    {
        get => catName;
        set => catName = value;
    }
    public string Description => description;

    public Sprite CatIcon => catIcon;
    public Color CardColor => cardColor;

    public float MoveSpeedMultiplier => moveSpeedMultiplier;
    public float HungerMultiplier => hungerMultiplier;
    public float SleepMultiplier => sleepMultiplier;
    public float HappinessMultiplier => happinessMultiplier;
}
