using Core.Characters.Health;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 5f; // Revertido a 5f
    [SerializeField] private Transform[] attackPoints; // Cambiado a array
    [SerializeField] private LayerMask attackCollisionLayers;
    [SerializeField] private int attackDamage = 15; // Cambiado a int
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Attack Area Settings")]
    [SerializeField] private Vector2 attackAreaSize = new Vector2(1f, 1f); // Revertido a 1f,1f
    [SerializeField] private Vector2 attackAreaOffset = new Vector2(1f, 0f); // Revertido el offset
    private Transform player;
    private bool canAttack = true;

    private void Awake()
    {
        FindPlayer();
    }

    private void OnEnable()
    {
        FindPlayer();
        InvokeRepeating(nameof(TryFindPlayer), 0f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(TryFindPlayer));
    }

    private void TryFindPlayer()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log($"[EnemyAttack] Found new player reference: {player.name}");
        }
        else
        {
            Debug.LogWarning("[EnemyAttack] No player found in scene");
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (canAttack && IsPlayerInRange())
        {
            PerformAttack();
        }
    }

    public void PerformAttack()
    {
        if (!canAttack) return;

        canAttack = false;
        Debug.Log($"Enemy attempting attack from {gameObject.name}");

        foreach (Transform attackPoint in attackPoints)
        {
            Vector2 attackPosition = attackPoint.position + (Vector3)attackAreaOffset;
            Debug.Log($"Attack position: {attackPosition}, Size: {attackAreaSize}, Layers: {attackCollisionLayers.value}");
            
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                attackPosition,
                attackAreaSize,
                0,
                attackCollisionLayers
            );

            Debug.Log($"Found {hitColliders.Length} colliders in attack area");
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                Debug.Log($"Hit object: {hitCollider.gameObject.name} with tag: {hitCollider.tag}");
                if (hitCollider.CompareTag("Player"))
                {
                    Debug.Log($"Player Hit by {gameObject.name}! Attempting to deal {attackDamage} damage");
                    Health playerHealth = hitCollider.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(attackDamage);
                        Debug.Log($"Damage dealt successfully. Player's current health: {playerHealth.CurrentHealth}");
                    }
                    else
                    {
                        Debug.LogError($"Player object {hitCollider.gameObject.name} does not have Health component!");
                    }
                }
            }
        }
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoints != null)
        {
            foreach (Transform attackPoint in attackPoints)
            {
                if (attackPoint != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.matrix = Matrix4x4.TRS(
                        attackPoint.position + (Vector3)attackAreaOffset,
                        attackPoint.rotation,
                        Vector3.one
                    );
                    Gizmos.DrawWireCube(Vector3.zero, attackAreaSize);
                }
            }
        }
    }
}
