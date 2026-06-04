using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Bounds")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minY = 5f;
    [SerializeField] private float maxY = 20f;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        Move();
        Zoom();

        // Плавное изменение
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    private void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D или ←/→
        float inputZ = Input.GetAxisRaw("Vertical");   // W/S или ↑/↓

        Vector3 moveDirection = new Vector3(inputX, 0f, inputZ).normalized;

        targetPosition += moveDirection * moveSpeed * Time.deltaTime;

        // Ограничение области движения
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
    }

    private void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetPosition.y -= scroll * zoomSpeed;

            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
    }
}
