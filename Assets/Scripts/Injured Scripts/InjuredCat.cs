using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuredCat : MonoBehaviour
{
    [SerializeField] private CatNeeds catNeeds;
    [SerializeField] private CatMover catMover;
    [SerializeField] private CatBrain catBrain;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider catCollider;

    [SerializeField] private string injuredTriggerName = "Die";

    [SerializeField] private GraphManager graphManager;
    [SerializeField] private NodeType escapeNodeType = NodeType.ShelterExit;
    //[SerializeField] private float escapeDangerWeight = 0f;

    [Header("Dog Escape")]
    [SerializeField] private Transform escapePoint;
    [SerializeField] private float escapeSpeed = 5f;
    [SerializeField] private float escapeReachDistance = 0.2f;
    [SerializeField] private string runTriggerName = "Run";

    private bool waitingForAmbulance;
    private bool escaping;
    public bool IsWaitingForAmbulance => waitingForAmbulance;
    public bool IsEscaping => escaping;

    private void Awake()
    {
        if (catNeeds == null)
            catNeeds = GetComponent<CatNeeds>();

        if (catMover == null)
            catMover = GetComponent<CatMover>();

        if (catBrain == null)
            catBrain = GetComponent<CatBrain>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (catCollider == null)
            catCollider = GetComponentInChildren<Collider>();
    }

    public void HitByCar()
    {
        Debug.Log("Столкновение");

        if (waitingForAmbulance)
            return;

        waitingForAmbulance = true;

        if (catNeeds != null)
            catNeeds.Kill();

        if (catMover != null)
            catMover.StopMoving();

        if (catBrain != null)
            catBrain.enabled = false;

        /*if (catCollider != null)
            catCollider.enabled = false;*/

        if (animator != null)
            animator.SetTrigger(injuredTriggerName);

        if (AmbulanceSystem.Instance != null)
            AmbulanceSystem.Instance.CallAmbulance(this);
    }

    public void PickUp()
    {
        if (catNeeds != null)
            catNeeds.Kill();

        Destroy(gameObject);
    }

    public void EscapeAfterDog()
    {
        if (waitingForAmbulance || escaping)
            return;

        escaping = true;

        if (catNeeds != null)
            catNeeds.Kill();

        if (catBrain != null)
            catBrain.enabled = false;

        if (catMover != null)
            catMover.StopMoving();

        if (graphManager == null)
            graphManager = FindObjectOfType<GraphManager>();

        catCollider.enabled = false;

        GraphNode startNode = graphManager.GetClosestNode(transform.position, true);
        GraphNode escapeNode = graphManager.GetClosestNodeOfType(
            transform.position,
            escapeNodeType,
            false
        );

        if (startNode == null || escapeNode == null)
        {
            Destroy(gameObject);
            return;
        }

        PathFinderAStar pathfinder = new PathFinderAStar();
        List<GraphNode> path = pathfinder.FindPath(startNode, escapeNode);

        if (path == null || path.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        catMover.SetPath(path);
        StartCoroutine(WaitUntilEscaped());
    }

    /*private IEnumerator RunAwayRoutine()
    {
        Vector3 targetPosition = GetEscapePosition();

        while (Vector3.Distance(transform.position, targetPosition) > escapeReachDistance)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime
                );
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                escapeSpeed * Time.deltaTime
            );

            yield return null;
        }

        Destroy(gameObject);
    }

    private Vector3 GetEscapePosition()
    {
        if (escapePoint != null)
            return escapePoint.position;

        Vector3 directionFromCenter = transform.position.normalized;
        directionFromCenter.y = 0f;

        if (directionFromCenter.sqrMagnitude < 0.001f)
            directionFromCenter = transform.forward;

        return transform.position + directionFromCenter.normalized * 12f;
    }*/

    private IEnumerator WaitUntilEscaped()
    {
        yield return null;

        while (catMover != null && catMover.IsMoving)
            yield return null;

        Destroy(gameObject);
    }
}
