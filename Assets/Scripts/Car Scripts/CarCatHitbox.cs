using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCatHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask catLayerMask;

    private void OnTriggerEnter(Collider other)
    {
        TryHitCat(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHitCat(other);
    }

    private void TryHitCat(Collider other)
    {
        if ((catLayerMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        InjuredCat cat = other.GetComponentInParent<InjuredCat>();

        if (cat == null)
            return;

        if (cat.IsWaitingForAmbulance)
            return;

        cat.HitByCar();
    }
}
