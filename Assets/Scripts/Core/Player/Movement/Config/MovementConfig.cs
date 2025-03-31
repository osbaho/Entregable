using UnityEngine;
using Core.Config;

namespace Core.Player.Movement.Config
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Config/Movement/MovementConfig")]
    public class MovementConfig : ConfigBase
    {
        [Header("Basic Movement")]
        public float MoveSpeed = 5f;
        public float JumpForce = 10f;
        public float GroundFriction = 10f;
        public float AirControl = 5f;

        [Header("Dash Settings")]
        public DashSettings DashSettings;

        [Header("Sprint Settings")]
        public SprintSettings SprintSettings;

        protected override void ValidateFields()
        {
            ValidateGreaterThanZero(ref MoveSpeed, nameof(MoveSpeed));
            ValidateGreaterThanZero(ref JumpForce, nameof(JumpForce));
            ValidateGreaterThanZero(ref GroundFriction, nameof(GroundFriction));
            ValidateGreaterThanZero(ref AirControl, nameof(AirControl));
        }
    }

    [System.Serializable]
    public struct DashSettings
    {
        public float DashSpeed;
        public float DashDuration;
        public float DashCooldown;
    }

    [System.Serializable]
    public struct SprintSettings
    {
        public float SprintMultiplier;
        public float SprintDrainRate;
        public float SprintRechargeRate;
    }
}
