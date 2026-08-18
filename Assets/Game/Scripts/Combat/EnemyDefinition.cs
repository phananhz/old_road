using UnityEngine;

namespace TheOldRoad.Combat
{
    [System.Serializable]
    public struct EnemyLootEntry
    {
        public string itemId;
        public int minQuantity;
        public int maxQuantity;
        [Range(0f, 1f)] public float dropChance;
    }

    [CreateAssetMenu(fileName = "NewEnemy", menuName = "The Old Road/Combat/Enemy Definition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string enemyId = "enemy.forest-wolf";
        [SerializeField] private string displayName = "Forest Wolf";
        [SerializeField, Min(1)] private int maxHealth = 12;
        [SerializeField, Min(0.1f)] private float moveSpeed = 2.4f;
        [SerializeField, Min(1)] private int attackDamage = 3;
        [SerializeField, Min(0.2f)] private float attackRange = 0.95f;
        [SerializeField, Min(1f)] private float detectionRadius = 5.5f;
        [SerializeField, Min(0.2f)] private float attackCooldown = 1.2f;
        [SerializeField] private EnemyLootEntry[] lootTable;

        public string EnemyId => enemyId;
        public string DisplayName => displayName;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public int AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public float DetectionRadius => detectionRadius;
        public float AttackCooldown => attackCooldown;
        public EnemyLootEntry[] LootTable => lootTable ?? System.Array.Empty<EnemyLootEntry>();

        public void ConfigureForPrototype(
            string id,
            string name,
            int hp,
            float speed,
            int damage,
            float range,
            float detection,
            float cooldown,
            EnemyLootEntry[] loot = null)
        {
            enemyId = id;
            displayName = name;
            maxHealth = Mathf.Max(1, hp);
            moveSpeed = speed;
            attackDamage = Mathf.Max(1, damage);
            attackRange = range;
            detectionRadius = detection;
            attackCooldown = cooldown;
            lootTable = loot ?? System.Array.Empty<EnemyLootEntry>();
        }
    }
}
