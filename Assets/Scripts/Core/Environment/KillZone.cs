using UnityEngine;
using Core.Characters.Health;
using Core.Utils;

namespace Core.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class KillZone : MonoBehaviour
    {
        private void Awake()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GameLogger.Log($"KillZone triggered by {other.gameObject.name}");
            
            Health health = other.GetComponent<Health>() ?? other.GetComponentInParent<Health>();
            if (health != null)
            {
                GameLogger.Log($"Found Health component on {health.gameObject.name}");
                health.TakeDamage(health.CurrentHealth);
            }
            else
            {
                GameLogger.LogWarning($"No Health component found on {other.gameObject.name} or its parents");
            }
        }
    }
}
