using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosswalkController : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private Transform crossingPoint;
    [SerializeField] private TrafficLightController trafficLight;

    [Header("Logic")]
    [SerializeField] private float warningLeadTime = 3f;
    [SerializeField] private float crossingTolerance = 0.75f;
    [SerializeField] private float activePassingRadius = 2.2f;

    [Header("State")]
    [SerializeField] private bool hasPlacedTrafficLight;

    public bool HasPlacedTrafficLight => hasPlacedTrafficLight;

    private void OnEnable()
    {
        if (TrafficSystem.Instance != null)
            TrafficSystem.Instance.RegisterCrosswalk(this);
    }

    private void OnDisable()
    {
        if (TrafficSystem.Instance != null)
            TrafficSystem.Instance.UnregisterCrosswalk(this);
    }

    private void Update()
    {
        if (!hasPlacedTrafficLight || trafficLight == null || crossingPoint == null || TrafficSystem.Instance == null)
            return;

        EvaluateTraffic();
    }

    public bool CanAttachTrafficLight()
    {
        return !hasPlacedTrafficLight;
    }

    public void AttachTrafficLight(TrafficLightController controller)
    {
        if (controller == null || hasPlacedTrafficLight)
            return;

        trafficLight = controller;
        hasPlacedTrafficLight = true;
        trafficLight.SetGreen();
    }

    private void EvaluateTraffic()
    {
        float nearestArrival = float.PositiveInfinity;
        bool carPassingNow = false;

        var cars = TrafficSystem.Instance.ActiveCars;
        for (int i = 0; i < cars.Count; i++)
        {
            CarAgent car = cars[i];
            if (car == null || !car.IsInitialized)
                continue;

            float eta = car.EstimateTimeToPoint(crossingPoint.position, crossingTolerance);
            if (!float.IsInfinity(eta) && eta < nearestArrival)
                nearestArrival = eta;

            float directDistance = Vector3.Distance(car.transform.position, crossingPoint.position);
            if (directDistance <= activePassingRadius)
                carPassingNow = true;
        }

        bool shouldBeRed = carPassingNow || nearestArrival <= warningLeadTime;

        if (shouldBeRed)
            trafficLight.SetRedBlinking();
        else
            trafficLight.SetGreen();
    }

    private void OnDrawGizmosSelected()
    {
        if (crossingPoint == null)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(crossingPoint.position, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(crossingPoint.position, activePassingRadius);
    }
}
