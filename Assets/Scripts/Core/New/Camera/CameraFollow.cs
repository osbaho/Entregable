using UnityEngine;
using Core.Characters;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;

    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool useFixedUpdate = false;

    [Header("Look Ahead Settings")]
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSpeed = 2f;
    [SerializeField] private float deadZoneWidth = 1f;

    [Header("Wall Detection Settings")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallOffset = 0.5f;
    [SerializeField] private float cameraHalfWidth;
    [SerializeField] private float cameraHalfHeight;

    private Vector3 velocity = Vector3.zero;
    private float currentLookAheadX = 0f;
    private float targetLookAheadX = 0f;
    private float lastTargetPositionX;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Start()
    {
        // Calculate camera bounds
        float orthoSize = targetCamera.orthographicSize;
        cameraHalfHeight = orthoSize;
        cameraHalfWidth = orthoSize * targetCamera.aspect;

        // Try to find existing player if one wasn't assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                // Actualizar posición inmediatamente
                Vector3 targetPosition = target.position;
                targetPosition.z = transform.position.z;
                transform.position = targetPosition;
            }
        }
    }

    private void LateUpdate()
    {
        if (!useFixedUpdate)
        {
            FollowTarget();
        }
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if (target == null) return;

        // Calculate the movement direction
        float moveDirection = Mathf.Sign(target.position.x - lastTargetPositionX);
        float movementDelta = Mathf.Abs(target.position.x - lastTargetPositionX);

        // Only update look ahead if movement is significant
        if (movementDelta > 0.01f)
        {
            targetLookAheadX = moveDirection * lookAheadDistance;
        }
        else if (Mathf.Abs(targetLookAheadX) < deadZoneWidth)
        {
            targetLookAheadX = 0;
        }

        // Smoothly interpolate current look ahead
        currentLookAheadX = Mathf.Lerp(currentLookAheadX, targetLookAheadX, Time.deltaTime * lookAheadSpeed);

        // Update target position with look ahead offset
        Vector3 targetPosition = target.position;
        targetPosition.x += currentLookAheadX;
        targetPosition.z = transform.position.z;

        // Check for walls and adjust position
        targetPosition = AdjustForWalls(targetPosition);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);

        lastTargetPositionX = target.position.x;
    }

    private Vector3 AdjustForWalls(Vector3 position)
    {
        // Puntos de verificación para la pared izquierda
        Vector2[] leftChecks = new Vector2[]
        {
            new Vector2(position.x - cameraHalfWidth, position.y), // Centro
            new Vector2(position.x - cameraHalfWidth, position.y + cameraHalfHeight * 0.5f), // Arriba
            new Vector2(position.x - cameraHalfWidth, position.y - cameraHalfHeight * 0.5f)  // Abajo
        };

        // Puntos de verificación para el suelo
        Vector2[] bottomChecks = new Vector2[]
        {
            new Vector2(position.x, position.y - cameraHalfHeight), // Centro
            new Vector2(position.x - cameraHalfWidth * 0.5f, position.y - cameraHalfHeight), // Izquierda
            new Vector2(position.x + cameraHalfWidth * 0.5f, position.y - cameraHalfHeight)  // Derecha
        };

        float maxX = float.MinValue;
        float maxY = float.MinValue;

        // Verificar paredes izquierdas
        foreach (var checkPoint in leftChecks)
        {
            RaycastHit2D hit = Physics2D.Raycast(checkPoint, Vector2.left, 1f, wallLayer);
            Debug.DrawRay(checkPoint, Vector2.left * 1f, Color.red);

            if (hit.collider != null)
            {
                float newX = hit.point.x + cameraHalfWidth + wallOffset;
                maxX = Mathf.Max(maxX, newX);
            }
        }

        // Verificar suelo
        foreach (var checkPoint in bottomChecks)
        {
            RaycastHit2D hit = Physics2D.Raycast(checkPoint, Vector2.down, 1f, wallLayer);
            Debug.DrawRay(checkPoint, Vector2.down * 1f, Color.blue);

            if (hit.collider != null)
            {
                float newY = hit.point.y + cameraHalfHeight + wallOffset;
                maxY = Mathf.Max(maxY, newY);
            }
        }

        // Aplicar ajustes si se detectaron colisiones
        if (maxX != float.MinValue)
        {
            position.x = maxX;
        }

        if (maxY != float.MinValue)
        {
            position.y = maxY;
        }

        return position;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Vector3 position = transform.position;
        
        // Dibujar límites de la cámara
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(position, new Vector3(cameraHalfWidth * 2, cameraHalfHeight * 2, 0.1f));
        
        // Dibujar puntos de verificación
        Gizmos.color = Color.red;
        DrawCheckPoints(position, true);
        
        Gizmos.color = Color.blue;
        DrawCheckPoints(position, false);
    }

    private void DrawCheckPoints(Vector3 position, bool isHorizontal)
    {
        if (isHorizontal)
        {
            Vector3[] points = new Vector3[]
            {
                new Vector3(position.x - cameraHalfWidth, position.y, position.z),
                new Vector3(position.x - cameraHalfWidth, position.y + cameraHalfHeight * 0.5f, position.z),
                new Vector3(position.x - cameraHalfWidth, position.y - cameraHalfHeight * 0.5f, position.z)
            };

            foreach (var point in points)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
        else
        {
            Vector3[] points = new Vector3[]
            {
                new Vector3(position.x, position.y - cameraHalfHeight, position.z),
                new Vector3(position.x - cameraHalfWidth * 0.5f, position.y - cameraHalfHeight, position.z),
                new Vector3(position.x + cameraHalfWidth * 0.5f, position.y - cameraHalfHeight, position.z)
            };

            foreach (var point in points)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("Attempting to set null target for camera!");
            return;
        }

        target = newTarget;
        lastTargetPositionX = target.position.x;
        currentLookAheadX = 0f;
        targetLookAheadX = 0f;
        velocity = Vector3.zero;

        // Actualizar inmediatamente la posición de la cámara al nuevo target
        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;

        Debug.Log($"Camera target set to: {target.name}");
    }
}
