using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosswalkTrigger : MonoBehaviour
{
    [SerializeField] private TrafficLightController trafficLight;
    [SerializeField] private Transform crossingPoint;

    [Header("Settings")]
    [SerializeField] private float redLeadTime = 3f;
    [SerializeField] private float activeCrossingDistance = 2.5f;

    private readonly List<CarAgent> trackedCars = new();

    private void Update()
    {
        UpdateTrackedCars();

        float nearestArrival = float.PositiveInfinity;
        bool anyCarPassingNow = false;

        foreach (var car in trackedCars)
        {
            if (car == null) continue;

            float time = car.EstimateTimeToPoint(crossingPoint.position);
            if (time < nearestArrival)
                nearestArrival = time;

            if (Vector3.Distance(car.transform.position, crossingPoint.position) <= activeCrossingDistance)
                anyCarPassingNow = true;
        }

        bool shouldBeRed = anyCarPassingNow || nearestArrival <= redLeadTime;

        if (shouldBeRed)
            trafficLight.SetRedBlinking();
        else
            trafficLight.SetGreen();
    }

    private void UpdateTrackedCars()
    {
        trackedCars.Clear();

        var allCars = FindObjectsByType<CarAgent>(FindObjectsSortMode.None);
        foreach (var car in allCars)
        {
            if (car == null || car.CurrentNode == null || car.NextNode == null)
                continue;

            if (SegmentIntersectsCrosswalk(car.CurrentNode.Position, car.NextNode.Position, crossingPoint.position, 1.25f))
                trackedCars.Add(car);
        }
    }

    private bool SegmentIntersectsCrosswalk(Vector3 a, Vector3 b, Vector3 point, float tolerance)
    {
        Vector3 projected = ProjectPointOnSegment(a, b, point);
        return Vector3.Distance(projected, point) <= tolerance;
    }

    private Vector3 ProjectPointOnSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float sqr = ab.sqrMagnitude;
        if (sqr < 0.0001f) return a;

        float t = Vector3.Dot(p - a, ab) / sqr;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }
}
