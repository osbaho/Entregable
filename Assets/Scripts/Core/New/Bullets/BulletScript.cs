using UnityEngine;
using Core.Characters.Health;

namespace Core
{
    public class BulletScript : MonoBehaviour
    {
        public float speed;
        [SerializeField] private int damage = 10;

        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = Vector2.right * speed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Health healthComponent = collision.gameObject.GetComponent<Health>();
                if (healthComponent != null)
                {
                    healthComponent.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}