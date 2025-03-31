using System.Collections;
using UnityEngine;
using Core.Characters.Health;
using Core.Player;
using Core; // Add this for BulletScript

public class PatchyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashEndMultiplier = 0.5f;
    private bool canDash = true;
    private bool isDashing = false;
    private float originalGravity;

    [Header("Wall Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallChecker;
    [SerializeField] private float wallCheckRadius = 0.2f;
    private bool isTouchingWall = false;

    [Header("Aim Settings")]
    [SerializeField] private float aimDuration = 1.5f;
    private float aimTimer = 0f;

    [Header("Special Power Settings")]
    [SerializeField] private GameObject specialPowerProjectilePrefab;
    [SerializeField] private Transform specialPowerSpawnPoint;
    [SerializeField] private LayerMask specialPowerCollisionLayers;

    [Header("Basic Attack Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float basicAttackRange = 7f; // Add this line
    private float nextFireTime;

    private Rigidbody2D rb;
    private PlayerInputHandlerA inputHandler;
    private float horizontalInput;
    private bool isGrounded;
    private bool shouldJump;

    private static readonly Vector2 RightDirection = Vector2.right;

    private PlayerStats playerStats;
    private bool isSprinting;
    private float sprintExperienceTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = PlayerInputHandlerA.Instance;
        playerStats = GetComponent<PlayerStats>();
        originalGravity = rb.gravityScale;
    }

    private void Update()
    {
        if (isDashing) return;

        // Check if touching wall before reading input
        if (isTouchingWall)
        {
            horizontalInput = 0; // Stop reading input if touching a wall
        }
        else
        {
            horizontalInput = inputHandler.MoveInput.x;
        }

        shouldJump = inputHandler.JumpTriggered && isGrounded;

        if (horizontalInput != 0 && !isTouchingWall)
        {
            FlipSprite(horizontalInput);
        }

        HandleAiming();
        HandleSpecialPower();
        HandleBasicAttack();
        HandleDash();
    }

    private void FixedUpdate()
    {
        CheckWallCollision();

        if (isDashing) return;

        ApplyMovement();
        if (shouldJump)
        {
            ApplyJump();
        }
        HandleSprint(); // Only call HandleSprint()
    }


    private void CheckWallCollision()
    {
        isTouchingWall = Physics2D.OverlapCircle(wallChecker.position, wallCheckRadius, wallLayer);
    }

    private void ApplyMovement()
    {
        if (isTouchingWall)
        {
            Debug.Log("Movement canceled due to wall contact!"); // Added Debug.Log
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement, keep vertical movement
        }
        else
        {
            float speed = inputHandler.SprintInput ? moveSpeed * sprintMultiplier : moveSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        }
    }

    private void ApplyJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        shouldJump = false;
    }

    /*private void ConsumeExperienceSprint()
    {
        if (inputHandler.SprintInput)
        {
            playerStats.ConsumeExperience(playerStats.SprintExperienceCostPerSecond);
        }
    }*/

    private void HandleSprint()
    {
        if (inputHandler.SprintInput && playerStats.CanSprint())
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
        if (isSprinting && playerStats != null)
        {
            // Consumir experiencia por segundo mientras está corriendo
            sprintExperienceTimer += Time.deltaTime;
            if (sprintExperienceTimer >= 1f)
            {
                playerStats.ConsumeExperience(playerStats.SprintExperienceCostPerSecond);
                sprintExperienceTimer = 0f;
            }

            // Verificar si aún puede seguir corriendo
            isSprinting = playerStats.CanSprint();
        }
    }

    private void HandleAiming()
    {
        if (inputHandler.AimInput != Vector2.zero)
        {
            aimTimer += Time.deltaTime;
            if (aimTimer >= aimDuration)
            {
                Debug.Log("Special Power Aimed!");
                aimTimer = 0f;
            }
        }
        else
        {
            aimTimer = 0f;
        }
    }

    private void HandleSpecialPower()
    {
        if (inputHandler.SpecialPowerInput && SpecialPowerProjectile.CanUseSpecialPower() && playerStats.CanUseSpecialPower())
        {
            Debug.Log("Special Power Activated!");
            GameObject projectile = Instantiate(specialPowerProjectilePrefab, specialPowerSpawnPoint.position, specialPowerSpawnPoint.rotation);
            playerStats.ConsumeExperience(playerStats.SpecialPowerExperienceCost);
            SpecialPowerProjectile.UseSpecialPower();
        }
        else if (inputHandler.SpecialPowerInput && !playerStats.CanUseSpecialPower())
        {
            Debug.Log("Not enough experience to use Special Power!");
            // Optionally, you could add some visual/audio feedback here to indicate the lack of experience.
        }
    }


    private void HandleBasicAttack()
    {
        if (inputHandler.BasicAttackInput && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();
            if (bulletScript != null)
            {
                bulletScript.speed = transform.localScale.x * 15f; // Ajusta la velocidad según la dirección
                Destroy(bullet, basicAttackRange / 15f); // Destruir la bala basado en el rango y la velocidad
            }
        }
    }

    private void HandleDash()
    {
        if (inputHandler.DashInput && canDash)
        {
            float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : transform.localScale.x;
            StartCoroutine(PerformDash(dashDirection));
        }
    }

    private IEnumerator PerformDash(float direction)
    {
        canDash = false;
        isDashing = true;

        Vector2 originalVelocity = rb.linearVelocity;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashSpeed * direction, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * dashEndMultiplier, originalVelocity.y);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Gizmo para el ataque básico
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + transform.localScale.x * Vector3.right * basicAttackRange);

        // Gizmo para el detector de paredes
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallChecker.position, wallCheckRadius);
    }
}
