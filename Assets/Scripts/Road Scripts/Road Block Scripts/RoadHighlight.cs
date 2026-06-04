using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Подсветка выбранной дороги
public class RoadHighlight : MonoBehaviour
{
    [SerializeField] private GameObject roadHighlight;

    private void Start()
    {
        if (roadHighlight != null)
            roadHighlight.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (roadHighlight == null)
            return;

        roadHighlight.SetActive(selected);
    }
}
