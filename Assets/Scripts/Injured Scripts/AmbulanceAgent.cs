using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbulanceAgent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float reachDistance = 0.08f;
    [SerializeField] private float pickUpDistance = 2.5f;

    private List<RoadNode> route = new List<RoadNode>();
    private int currentIndex;
    private InjuredCat targetCat;
    private bool pickedUp;

    public void Initialize(List<RoadNode> roadRoute, InjuredCat cat)
    {
        route = roadRoute != null ? new List<RoadNode>(roadRoute) : new List<RoadNode>();
        targetCat = cat;
        currentIndex = 0;
        pickedUp = false;

        if (route.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = route[0].Position;

        if (route.Count > 1)
            currentIndex = 1;
    }

    private void Update()
    {
        if (route == null || route.Count == 0 || currentIndex >= route.Count)
        {
            Destroy(gameObject);
            return;
        }

        TryPickUpCat();
        MoveToCurrentNode();
    }

    private void MoveToCurrentNode()
    {
        RoadNode targetNode = route[currentIndex];

        if (targetNode == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 target = targetNode.Position;
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) <= reachDistance)
        {
            currentIndex++;

            if (currentIndex >= route.Count)
                Destroy(gameObject);
        }
    }

    private void TryPickUpCat()
    {
        if (pickedUp || targetCat == null)
            return;

        float distance = Vector3.Distance(transform.position, targetCat.transform.position);

        if (distance <= pickUpDistance)
        {
            pickedUp = true;
            targetCat.PickUp();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
