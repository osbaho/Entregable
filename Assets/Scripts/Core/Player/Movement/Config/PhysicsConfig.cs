using UnityEngine;
using Core.Config;

namespace Core.Player.Movement.Config
{
    [CreateAssetMenu(fileName = "PhysicsConfig", menuName = "Config/Movement/PhysicsConfig")]
    public class PhysicsConfig : ConfigBase
    {
        [Header("Collision Settings")]
        [Range(0f, 90f)] public float WallCollisionAngleThreshold = 45f;
        [Range(0f, 90f)] public float GroundCollisionAngleThreshold = 45f;

        protected override void ValidateFields()
        {
            ValidateGreaterThanZero(ref WallCollisionAngleThreshold, nameof(WallCollisionAngleThreshold));
            ValidateGreaterThanZero(ref GroundCollisionAngleThreshold, nameof(GroundCollisionAngleThreshold));
        }
    }
}
