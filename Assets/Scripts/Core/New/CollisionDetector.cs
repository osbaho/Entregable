using UnityEngine;
using System;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayers;
    public event Action<Collider2D> OnCollision; // Public event with custom delegate

    public LayerMask CollisionLayers
    {
        get => collisionLayers;
        set => collisionLayers = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionLayers == (collisionLayers | (1 << collision.gameObject.layer)))
        {
            OnCollision?.Invoke(collision);
        }
    }
}
