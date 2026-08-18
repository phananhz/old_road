using UnityEngine;

namespace TheOldRoad.Combat
{
    public enum DamageType
    {
        Physical,
        Fire,
        Blunt,
        Slashing
    }

    public struct DamageInfo
    {
        public int Amount { get; }
        public Vector2 Direction { get; }
        public float KnockbackForce { get; }
        public GameObject Source { get; }
        public DamageType Type { get; }

        public DamageInfo(int amount, Vector2 direction, float knockbackForce = 2.5f, GameObject source = null, DamageType type = DamageType.Physical)
        {
            Amount = Mathf.Max(1, amount);
            Direction = direction.normalized;
            KnockbackForce = knockbackForce;
            Source = source;
            Type = type;
        }
    }
}
