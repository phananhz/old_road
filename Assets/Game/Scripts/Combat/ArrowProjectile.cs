using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.Combat
{
    /// <summary>Flying arrow projectile shot from bows.</summary>
    public sealed class ArrowProjectile : MonoBehaviour
    {
        private Vector2 direction;
        private float speed = 13.5f;
        private int damage = 5;
        private float maxDistance = 9.5f;
        private Vector3 startPosition;
        private SpriteRenderer spriteRenderer;

        public static void Launch(Vector3 origin, Vector2 dir, int dmg)
        {
            GameObject go = new GameObject("ArrowProjectile");
            go.transform.position = origin;
            ArrowProjectile arrow = go.AddComponent<ArrowProjectile>();
            arrow.Initialize(dir, dmg);
        }

        public void Initialize(Vector2 dir, int dmg)
        {
            direction = dir.normalized;
            damage = dmg;
            startPosition = transform.position;

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PrototypePixelArtFactory.ArrowSprite;
            spriteRenderer.sortingOrder = 35;

            // Rotate towards direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Update()
        {
            Vector3 movement = (Vector3)(direction * (speed * UnityEngine.Time.deltaTime));
            transform.position += movement;

            // Check collision with enemies
            CheckHitEnemies();

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                Destroy(gameObject);
            }
        }

        private void CheckHitEnemies()
        {
            Vector3 pos = transform.position;
            const float hitRadius = 0.55f;

            EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyController enemy = enemies[i];
                if (enemy == null || enemy.IsDead) continue;

                if (Vector2.Distance(pos, enemy.transform.position) <= hitRadius)
                {
                    DamageInfo info = new DamageInfo(damage, direction, 3.0f, gameObject, DamageType.Piercing);
                    enemy.TakeDamage(info);
                    Destroy(gameObject);
                    return;
                }
            }

            NightMonsterController[] monsters = FindObjectsByType<NightMonsterController>(FindObjectsInactive.Exclude);
            for (int i = 0; i < monsters.Length; i++)
            {
                NightMonsterController monster = monsters[i];
                if (monster == null || monster.IsDead) continue;

                if (Vector2.Distance(pos, monster.transform.position) <= hitRadius)
                {
                    monster.TakeDamage(damage);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
