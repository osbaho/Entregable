using UnityEngine;
using System.Collections.Generic;
using Core.Characters.Health; // Añadir este using
using Core.Characters; // Add this using
using Core.Player; // Add this using



public class EnemyAStar : MonoBehaviour
{
    [Header("Configuración Principal")]
    public Transform target; // Changed from player to target
    public float checkRadius = 30f;
    public float moveSpeed = 20f;
    public LayerMask restrictedZoneLayer;

    [SerializeField]
    private int maxIterations = 1500; // Añadir límite de iteraciones
    [SerializeField]
    private float nodeSpacing = 0.5f; // Nueva variable para espaciado entre nodos

    private Rigidbody2D rb;

    [Header("Configuración de Salto")]
    public float jumpForce = 15f;
    public float groundCheckRadius = 3f;
    public LayerMask groundLayer;
    public float maxJumpHeight = 20f;

    [Header("Configuración de Física")]
    public float gravityMultiplier = 2.5f; // Nuevo multiplicador de gravedad

    private bool isGrounded;

    [Header("Configuración de Gizmos")]
    public bool alwaysShowRadius = true;
    public Color inRangeColor = Color.red;
    public Color outOfRangeColor = Color.green;
    public Color pathColor = Color.yellow;

    [Header("Debug")]
    public bool showJumpChecks = true;
    private bool shouldJump = false;

    [Header("Configuración de Ataque")]
    public float attackRange = 2f;
    public int attackDamage = 10; // Cambiado a int
    public float attackCooldown = 1f;
    private float nextAttackTime;

    [Header("Target Finding")]
    [SerializeField] private float targetSearchInterval = 1f;
    private float nextTargetSearchTime;

    private List<Vector2> currentPath;
    private int targetIndex;
    private bool playerInRange;
    private bool pathfindingFailed; // Nueva variable

    [SerializeField]
    private bool preserveChildScales = true; // Añadir esta variable al inicio de la clase

    private Dictionary<Transform, Vector3> originalScales; // Añadir esta variable al inicio de la clase

    [SerializeField]
    private bool mirrorChildPositions = true; // Añadir esta variable al inicio de la clase

    private Dictionary<Transform, Vector3> originalPositions; // Añadir esta variable al inicio de la clase

    private void OnEnable()
    {
        FindPlayer();
        InvokeRepeating(nameof(TryFindPlayer), 0f, targetSearchInterval);
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayerEvents();
        CancelInvoke(nameof(TryFindPlayer));
    }

    private void TryFindPlayer()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindPlayer();
            Debug.Log("[EnemyAStar] Attempting to find new player reference");
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Player not found!");
            return; // Exit early if player is not found
        }

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Found player but no Health component: {player.name}");
            return; // Exit early if no Health component is found
        }

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Found player but no PlayerStats component: {player.name}");
            return; // Exit early if no PlayerStats component is found
        }

        // ... use the playerHealth component and the playerStats component...
    }

    private void SubscribeToPlayerEvents()
    {
        if (target != null)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath += OnPlayerDeath;
                Debug.Log($"[EnemyAStar] Subscribed to player events: {target.name}");
            }
        }
    }

    private void UnsubscribeFromPlayerEvents()
    {
        if (target != null)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath -= OnPlayerDeath;
                Debug.Log($"[EnemyAStar] Unsubscribed from player events: {target.name}");
            }
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log($"[EnemyAStar] Player death detected on {gameObject.name}");
        UnsubscribeFromPlayerEvents();
        target = null;
        currentPath = null;
        playerInRange = false;
        nextTargetSearchTime = 0; // Forzar búsqueda inmediata
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindPlayer();
        
        // Guardar las escalas originales de todos los hijos
        if (preserveChildScales)
        {
            originalScales = new Dictionary<Transform, Vector3>();
            foreach (Transform child in transform)
            {
                originalScales[child] = child.localScale;
            }
        }

        // Guardar las posiciones originales de los hijos
        if (mirrorChildPositions)
        {
            originalPositions = new Dictionary<Transform, Vector3>();
            foreach (Transform child in transform)
            {
                originalPositions[child] = child.localPosition;
            }
        }
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            return;
        }

        // Si no hay target o se alcanzó el tiempo de búsqueda, buscar nuevo player
        if ((target == null || !target.gameObject.activeInHierarchy) && Time.time >= nextTargetSearchTime)
        {
            Debug.Log("[EnemyAStar] Searching for new player...");
            FindPlayer();
            nextTargetSearchTime = Time.time + targetSearchInterval;
            return; // Salir y esperar al siguiente frame
        }

        if (target == null)
        {
            // Si no hay target, resetear estados
            currentPath = null;
            playerInRange = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        CheckGrounded();
        CheckPlayerRange();
        UpdateFacing();
        CheckAttack();

        if (playerInRange)
        {
            FindPath(transform.position, target.position);
            FollowPath();
        }
    }

    void UpdateFacing()
    {
        if (target != null)
        {
            float direction = target.position.x - transform.position.x;
            FlipSprite(direction);
        }
    }

    private void FlipSprite(float direction)
    {
        bool wasFlipped = transform.localScale.x < 0;
        bool willBeFlipped = direction < 0;
        
        // Solo proceder si hay un cambio real en la dirección
        if (wasFlipped != willBeFlipped)
        {
            // Voltear el padre
            Vector3 newScale = transform.localScale;
            newScale.x = direction > 0 ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
            transform.localScale = newScale;

            // Ajustar posiciones de los hijos
            if (mirrorChildPositions && originalPositions != null)
            {
                foreach (Transform child in transform)
                {
                    if (originalPositions.TryGetValue(child, out Vector3 originalPos))
                    {
                        child.localPosition = new Vector3(-originalPos.x, originalPos.y, originalPos.z);
                    }
                }
            }
        }

        // Corregir las escalas de los hijos si es necesario
        if (preserveChildScales && originalScales != null)
        {
            foreach (Transform child in transform)
            {
                if (originalScales.TryGetValue(child, out Vector3 originalScale))
                {
                    child.localScale = originalScale;
                }
            }
        }
    }

    void CheckPlayerRange()
    {
        if (target == null) return;
        playerInRange = Vector2.Distance(transform.position, target.position) <= checkRadius;
    }

    void CheckGrounded()
    {
        Vector2 groundCheckPosition = (Vector2)transform.position + Vector2.down * 0.5f;
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition, groundCheckRadius, groundLayer);

        if (showJumpChecks)
        {
            Debug.DrawRay(transform.position, Vector2.down * 0.5f, isGrounded ? Color.green : Color.red);
            Debug.Log($"Grounded: {isGrounded}");
        }
    }

    private void CheckAttack()
    {
        if (target == null || Time.time < nextAttackTime) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= attackRange)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        // Validar distancia máxima
        if (Vector2.Distance(startPos, targetPos) > checkRadius)
        {
            currentPath = null;
            return;
        }

        // Redondear posiciones a la cuadrícula
        startPos = new Vector2(
            Mathf.Round(startPos.x / nodeSpacing) * nodeSpacing,
            Mathf.Round(startPos.y / nodeSpacing) * nodeSpacing
        );
        targetPos = new Vector2(
            Mathf.Round(targetPos.x / nodeSpacing) * nodeSpacing,
            Mathf.Round(targetPos.y / nodeSpacing) * nodeSpacing
        );

        Node startNode = new Node(startPos);
        Node targetNode = new Node(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        int iterations = 0; // Contador de iteraciones

        while (openSet.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost &&
                     openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.position == targetNode.position)
            {
                currentPath = RetracePath(startNode, currentNode);
                targetIndex = 0;
                pathfindingFailed = false; // Resetear el estado cuando hay un path válido
                return;
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                int newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        if (iterations >= maxIterations)
        {
            Debug.LogWarning("Pathfinding aborted: reached maximum iterations");
            currentPath = null;
            pathfindingFailed = true; // Marcamos que el pathfinding falló
        }
    }

    List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        return Mathf.RoundToInt(Mathf.Abs(nodeA.position.x - nodeB.position.x));
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector2[] directions = {
            new Vector2(nodeSpacing, 0f),     // Derecha
            new Vector2(-nodeSpacing, 0f),    // Izquierda
            new Vector2(0f, maxJumpHeight)    // Arriba (para salto)
        };

        foreach (Vector2 dir in directions)
        {
            Vector2 neighborPos = node.position + dir;
            if (Vector2.Distance(transform.position, neighborPos) <= checkRadius)
            {
                bool walkable = !Physics2D.OverlapCircle(neighborPos, 0.4f, restrictedZoneLayer);
                neighbors.Add(new Node(neighborPos, walkable));
            }
        }

        return neighbors;
    }

    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            if (pathfindingFailed)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Mantener la velocidad vertical
                return;
            }
            return;
        }

        pathfindingFailed = false;

        Vector2 currentWaypoint = currentPath[targetIndex];
        Vector2 direction = ((Vector2)currentWaypoint - (Vector2)transform.position).normalized;

        // Mover horizontalmente
        float horizontalMove = direction.x * moveSpeed;

        // Aplicar gravedad
        float verticalVelocity = rb.linearVelocity.y;
        if (!isGrounded)
        {
            verticalVelocity += Physics2D.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        // Solo actualizar la velocidad vertical si estamos saltando
        if (shouldJump && isGrounded)
        {
            verticalVelocity = jumpForce;
            if (showJumpChecks)
            {
                Debug.Log($"Jumping! Height difference: {currentWaypoint.y - transform.position.y}");
            }
        }

        rb.linearVelocity = new Vector2(horizontalMove, verticalVelocity);

        // Verificar si llegó al waypoint
        if (Vector2.Distance(transform.position, currentWaypoint) < 0.1f)
        {
            targetIndex++;
            if (targetIndex >= currentPath.Count)
            {
                currentPath = null;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!alwaysShowRadius)
        {
            DrawDetectionGizmo();
        }
    }

    void OnDrawGizmos()
    {
        if (alwaysShowRadius)
        {
            DrawDetectionGizmo();
        }

        // Dibujar verificación de suelo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.down * 0.5f, groundCheckRadius);

        if (showJumpChecks && currentPath != null && targetIndex < currentPath.Count)
        {
            Gizmos.color = Color.blue;
            Vector2 currentWaypoint = currentPath[targetIndex];
            Gizmos.DrawLine(transform.position, currentWaypoint);
            Gizmos.DrawWireSphere(currentWaypoint, 0.3f);

            // Mostrar la diferencia de altura
            Gizmos.color = shouldJump ? Color.red : Color.green;
            Vector2 heightCheckStart = new Vector2(transform.position.x, transform.position.y);
            Vector2 heightCheckEnd = new Vector2(transform.position.x, currentWaypoint.y);
            Gizmos.DrawLine(heightCheckStart, heightCheckEnd);
        }

        // Dibujar rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void DrawDetectionGizmo()
    {
        // Radio de detección
        Gizmos.color = playerInRange ? inRangeColor : outOfRangeColor;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        // Ruta calculada
        if (currentPath != null)
        {
            Gizmos.color = pathColor;
            foreach (Vector2 point in currentPath)
            {
                Gizmos.DrawCube(point, Vector3.one * 0.3f);
            }
        }
    }

    class Node
    {
        public Vector2 position;
        public bool walkable;
        public int gCost;
        public int hCost;
        public Node parent;

        public int FCost { get { return gCost + hCost; } }

        public Node(Vector2 pos, bool walkable = true)
        {
            position = pos;
            this.walkable = walkable;
        }
    }
}