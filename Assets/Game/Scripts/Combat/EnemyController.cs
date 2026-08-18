using System.Collections;
using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.World;

namespace TheOldRoad.Combat
{
    public sealed class EnemyController : MonoBehaviour, IDamageable
    {
        public enum EnemyState
        {
            Idle,
            Patrol,
            Chase,
            AttackWindup,
            Stagger,
            Dead
        }

        [SerializeField] private EnemyDefinition definition;
        [SerializeField] private int currentHealth;

        private EnemyState state = EnemyState.Idle;
        private Vector3 spawnPoint;
        private Vector3 patrolTarget;
        private Transform playerTarget;
        private PlayerVitals playerVitals;
        private SpriteRenderer spriteRenderer;

        private float stateTimer;
        private float attackTimer;
        private float animTimer;
        private int animFrame;
        private bool isFlipped;

        public Transform Transform => transform;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => definition != null ? definition.MaxHealth : 10;
        public bool IsDead => state == EnemyState.Dead || currentHealth <= 0;
        public string EnemyId => definition != null ? definition.EnemyId : "enemy.generic";
        public string DisplayName => definition != null ? definition.DisplayName : "Enemy";

        public void Configure(EnemyDefinition def)
        {
            definition = def;
            currentHealth = def.MaxHealth;
            spawnPoint = transform.position;
            patrolTarget = spawnPoint;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sortingOrder = 30;
            spawnPoint = transform.position;
            patrolTarget = spawnPoint;
        }

        private void Start()
        {
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<EnemyDefinition>();
                definition.ConfigureForPrototype("enemy.forest-wolf", "Forest Wolf", 10, 2.3f, 3, 0.95f, 5.5f, 1.4f);
                currentHealth = definition.MaxHealth;
            }

            FindPlayer();
            UpdateSprite();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) player = GameObject.Find("Player");
            if (player != null)
            {
                playerTarget = player.transform;
                playerVitals = player.GetComponent<PlayerVitals>();
            }
        }

        private void Update()
        {
            if (IsDead) return;

            if (playerTarget == null)
            {
                FindPlayer();
            }

            attackTimer -= UnityEngine.Time.deltaTime;
            stateTimer -= UnityEngine.Time.deltaTime;

            switch (state)
            {
                case EnemyState.Idle:
                    UpdateIdle();
                    break;
                case EnemyState.Patrol:
                    UpdatePatrol();
                    break;
                case EnemyState.Chase:
                    UpdateChase();
                    break;
                case EnemyState.AttackWindup:
                    UpdateAttackWindup();
                    break;
                case EnemyState.Stagger:
                    UpdateStagger();
                    break;
            }

            UpdateAnimation();
        }

        private void UpdateIdle()
        {
            if (CheckPlayerDetection()) return;

            if (stateTimer <= 0f)
            {
                // Pick new patrol point near spawn point
                patrolTarget = spawnPoint + (Vector3)(Random.insideUnitCircle * 3.5f);
                state = EnemyState.Patrol;
                stateTimer = Random.Range(3f, 6f);
            }
        }

        private void UpdatePatrol()
        {
            if (CheckPlayerDetection()) return;

            Vector3 delta = patrolTarget - transform.position;
            float dist = delta.magnitude;

            if (dist > 0.1f)
            {
                Vector3 move = delta.normalized * (definition.MoveSpeed * 0.5f * UnityEngine.Time.deltaTime);
                transform.position += move;
                isFlipped = delta.x < 0;
            }
            else if (stateTimer <= 0f)
            {
                state = EnemyState.Idle;
                stateTimer = Random.Range(2f, 4f);
            }
        }

        private void UpdateChase()
        {
            if (playerTarget == null)
            {
                state = EnemyState.Idle;
                return;
            }

            Vector3 delta = playerTarget.position - transform.position;
            float dist = delta.magnitude;

            if (dist > definition.DetectionRadius * 1.6f)
            {
                // Player escaped
                state = EnemyState.Idle;
                stateTimer = 2f;
                return;
            }

            if (dist <= definition.AttackRange)
            {
                if (attackTimer <= 0f)
                {
                    StartAttack();
                }
                return;
            }

            // Move towards player
            Vector3 move = delta.normalized * (definition.MoveSpeed * UnityEngine.Time.deltaTime);
            transform.position += move;
            isFlipped = delta.x < 0;
        }

        private void StartAttack()
        {
            state = EnemyState.AttackWindup;
            stateTimer = 0.28f; // Windup duration
        }

        private void UpdateAttackWindup()
        {
            if (stateTimer <= 0f)
            {
                ExecuteAttack();
                attackTimer = definition.AttackCooldown;
                state = EnemyState.Chase;
            }
        }

        private void ExecuteAttack()
        {
            if (playerTarget == null) return;

            float dist = Vector3.Distance(transform.position, playerTarget.position);
            if (dist <= definition.AttackRange * 1.35f)
            {
                if (playerVitals != null)
                {
                    Vector2 knockbackDir = (playerTarget.position - transform.position).normalized;
                    PlayerCombatController playerCombat = playerTarget.GetComponent<PlayerCombatController>();
                    if (playerCombat != null)
                    {
                        playerCombat.TakeDamageFromEnemy(definition.AttackDamage, knockbackDir);
                    }
                    else
                    {
                        playerVitals.TakeDamage(definition.AttackDamage);
                        FloatingTextController.SpawnPlayerDamage(definition.AttackDamage, playerTarget.position);
                    }
                }
            }
        }

        private void UpdateStagger()
        {
            if (stateTimer <= 0f)
            {
                state = EnemyState.Chase;
            }
        }

        private bool CheckPlayerDetection()
        {
            if (playerTarget == null) return false;

            float dist = Vector3.Distance(transform.position, playerTarget.position);
            if (dist <= definition.DetectionRadius)
            {
                state = EnemyState.Chase;
                FloatingTextController.Spawn("!", transform.position + Vector3.up * 0.6f, Color.red, 0.6f);
                return true;
            }
            return false;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (IsDead) return;

            TheOldRoad.Audio.AudioManager.PlayHitImpact();
            currentHealth = Mathf.Max(0, currentHealth - damage.Amount);
            FloatingTextController.SpawnDamage(damage.Amount, transform.position, damage.Amount >= 4);

            // Apply knockback
            transform.position += (Vector3)(damage.Direction * (damage.KnockbackForce * 0.12f));

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                state = EnemyState.Stagger;
                stateTimer = 0.25f;
                StartCoroutine(HitFlashRoutine());
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
            yield return new WaitForSeconds(0.12f);
            if (!IsDead) spriteRenderer.color = Color.white;
        }

        private void Die()
        {
            state = EnemyState.Dead;
            TheOldRoad.Audio.AudioManager.PlayEnemyDefeated();
            FloatingTextController.Spawn("DEFEATED", transform.position + Vector3.up * 0.5f, new Color(1f, 0.8f, 0.2f, 1f), 1f);

            DropLoot();
            StartCoroutine(DeathFadeRoutine());
        }

        private void DropLoot()
        {
            if (definition == null || definition.LootTable == null) return;

            foreach (EnemyLootEntry entry in definition.LootTable)
            {
                if (Random.value <= entry.dropChance)
                {
                    int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                    if (qty > 0)
                    {
                        // Add to player inventory if close, or spawn loot pickup
                        VerticalSliceController vsc = FindAnyObjectByType<VerticalSliceController>();
                        if (vsc != null && vsc.Inventory != null)
                        {
                            vsc.Inventory.Add(entry.itemId, qty);
                            FloatingTextController.Spawn("+" + qty + " " + entry.itemId.Replace("item.", ""), transform.position, Color.green, 1.2f);
                        }
                    }
                }
            }
        }

        private IEnumerator DeathFadeRoutine()
        {
            float elapsed = 0f;
            float duration = 0.45f;
            Vector3 baseScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += UnityEngine.Time.deltaTime;
                float t = elapsed / duration;
                spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f - t);
                transform.localScale = baseScale * (1f - t * 0.3f);
                yield return null;
            }

            Destroy(gameObject);
        }

        private void UpdateAnimation()
        {
            if (IsDead) return;

            animTimer += UnityEngine.Time.deltaTime * (state == EnemyState.Chase ? 8f : 4f);
            animFrame = Mathf.FloorToInt(animTimer) % 4;

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (EnemyId.Contains("wolf"))
            {
                spriteRenderer.sprite = PrototypePixelArtFactory.WolfSprite(animFrame);
            }
            else
            {
                spriteRenderer.sprite = PrototypePixelArtFactory.BanditSprite(animFrame);
            }

            spriteRenderer.flipX = isFlipped;
        }

        private void OnGUI()
        {
            if (IsDead || currentHealth >= MaxHealth) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 0.65f);
            if (screenPos.z < 0) return;

            float width = 36f;
            float height = 5f;
            float x = screenPos.x - width * 0.5f;
            float y = Screen.height - screenPos.y;

            // Background
            GUI.color = new Color(0f, 0f, 0f, 0.8f);
            GUI.DrawTexture(new Rect(x - 1, y - 1, width + 2, height + 2), Texture2D.whiteTexture);

            // Red Health Fill
            float fill = (float)currentHealth / MaxHealth;
            GUI.color = new Color(0.9f, 0.15f, 0.15f, 0.95f);
            GUI.DrawTexture(new Rect(x, y, width * fill, height), Texture2D.whiteTexture);

            GUI.color = Color.white;
        }
    }
}
