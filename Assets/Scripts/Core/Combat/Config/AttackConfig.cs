using UnityEngine;
using Core.Config;

namespace Core.Combat.Config
{
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "Config/Combat/AttackConfig")]
    public class AttackConfig : ConfigBase
    {
        [Header("Basic Attack Settings")]
        [SerializeField] private float baseDamage = 20f;
        [SerializeField] private float basicProjectileSpeed = 10f;
        [SerializeField] private float basicShootRange = 10f;
        [SerializeField] private string basicAttackPoolTag = "BasicAttack";
        [SerializeField] private GameObject basicProjectilePrefab;

        [Header("Special Attack Settings")]
        [SerializeField] private float specialDamage = 40f;
        [SerializeField] private float specialProjectileSpeed = 15f;
        [SerializeField] private float specialShootRange = 15f;
        [SerializeField] private float specialAttackCooldown = 2f;
        [SerializeField] private float aimingTime = 1f;
        [SerializeField] private string specialAttackPoolTag = "SpecialAttack";
        [SerializeField] private GameObject specialProjectilePrefab;
        [SerializeField] private GameObject aimIndicatorPrefab; // This is the field

        // Public accessors (properties)
        public float BaseDamage => baseDamage;
        public float BasicProjectileSpeed => basicProjectileSpeed;
        public float BasicShootRange => basicShootRange;
        public string BasicAttackPoolTag => basicAttackPoolTag;
        public GameObject BasicProjectilePrefab => basicProjectilePrefab;
        public float SpecialDamage => specialDamage;
        public float SpecialProjectileSpeed => specialProjectileSpeed;
        public float SpecialShootRange => specialShootRange;
        public float SpecialAttackCooldown => specialAttackCooldown;
        public float AimingTime => aimingTime;
        public string SpecialAttackPoolTag => specialAttackPoolTag;
        public GameObject SpecialProjectilePrefab => specialProjectilePrefab;
        public GameObject AimIndicatorPrefab => aimIndicatorPrefab; // Add this line

        protected override void ValidateFields()
        {
            ValidateGreaterThanZero(ref baseDamage, nameof(baseDamage));
            ValidateGreaterThanZero(ref basicProjectileSpeed, nameof(basicProjectileSpeed));
            ValidateGreaterThanZero(ref basicShootRange, nameof(basicShootRange));
            ValidateGreaterThanZero(ref specialDamage, nameof(specialDamage));
            ValidateGreaterThanZero(ref specialProjectileSpeed, nameof(specialProjectileSpeed));
            ValidateGreaterThanZero(ref specialShootRange, nameof(specialShootRange));
            ValidateGreaterThanZero(ref specialAttackCooldown, nameof(specialAttackCooldown));
            ValidateGreaterThanZero(ref aimingTime, nameof(aimingTime));
        }
    }
}
