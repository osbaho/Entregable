using UnityEngine;

namespace Core.Combat.Interfaces
{
    public interface IAttackStrategy
    {
        void PerformAttack();
        void Execute(Vector2 origin, Vector2 direction, LayerMask targetLayer);
    }
}