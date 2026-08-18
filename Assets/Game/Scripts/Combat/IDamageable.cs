using UnityEngine;

namespace TheOldRoad.Combat
{
    public interface IDamageable
    {
        Transform Transform { get; }
        int CurrentHealth { get; }
        int MaxHealth { get; }
        bool IsDead { get; }

        void TakeDamage(DamageInfo damage);
    }
}
