using System.Collections;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Player;

namespace TheOldRoad.Combat
{
    public sealed class PlayerCombatController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float attackCooldown = 0.32f;
        [SerializeField, Min(0.1f)] private float iFrameDuration = 0.75f;

        private PlayerMovement movement;
        private PlayerVitals vitals;
        private SpriteRenderer spriteRenderer;
        private InventorySession inventorySession;

        private float lastAttackTime = -10f;
        private bool isInvincible;
        private Vector2 lastFacingDirection = Vector2.right;

        public bool CanAttack => UnityEngine.Time.time >= lastAttackTime + attackCooldown;
        public bool IsInvincible => isInvincible;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            vitals = GetComponent<PlayerVitals>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Configure(InventorySession session)
        {
            inventorySession = session;
        }

        private bool isBlocking;
        public bool IsBlocking => isBlocking;

        private void Update()
        {
            UpdateFacing();

            // Check shield block (Right Click or V key)
            isBlocking = (UnityEngine.Input.GetKey(KeyCode.V) || UnityEngine.Input.GetMouseButton(1)) && HasShield();

            // Keyboard / Mouse attack input
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (!isBlocking) TryAttack();
            }
        }

        private void UpdateFacing()
        {
            if (movement != null && movement.LastMoveDirection.sqrMagnitude > 0.01f)
            {
                lastFacingDirection = movement.LastMoveDirection.normalized;
            }
        }

        public bool TryAttack()
        {
            if (!CanAttack) return false;

            lastAttackTime = UnityEngine.Time.time;
            ExecuteAttack();
            return true;
        }

        private void ExecuteAttack()
        {
            string currentItem = GetSelectedItemId();
            Vector2 attackDir = lastFacingDirection;
            Vector3 attackOrigin = transform.position;

            // Check if Bow is equipped
            if (currentItem == "item.weapon-bow")
            {
                InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
                if (inv != null && inv.GetQuantity("item.ammo-arrow") > 0)
                {
                    inv.TryRemove("item.ammo-arrow", 1);
                    ArrowProjectile.Launch(attackOrigin, attackDir, 6);
                    TheOldRoad.Audio.AudioManager.PlaySwordSlash();
                    return;
                }
                else
                {
                    FloatingTextController.Spawn(UI.LocalizationRuntime.IsVietnamese ? "Hết tên!" : "No arrows!", attackOrigin + Vector3.up * 0.6f, Color.red, 0.8f);
                    return;
                }
            }

            GetEquippedWeaponStats(out int damage, out float range, out float knockback, out Color slashColor, out DamageType damageType);

            // Visual slash arc
            SlashVfx.Create(attackOrigin, attackDir, slashColor, range);
            TheOldRoad.Audio.AudioManager.PlaySwordSlash();

            // Detect targets in front cone
            EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyController enemy = enemies[i];
                if (enemy == null || enemy.IsDead) continue;

                Vector2 toEnemy = (Vector2)(enemy.transform.position - attackOrigin);
                float dist = toEnemy.magnitude;

                if (dist <= range)
                {
                    float angle = Vector2.Angle(attackDir, toEnemy.normalized);
                    if (angle <= 75f) // 150 degree arc in front
                    {
                        DamageInfo info = new DamageInfo(damage, attackDir, knockback, gameObject, damageType);
                        enemy.TakeDamage(info);
                    }
                }
            }

            NightMonsterController[] monsters = FindObjectsByType<NightMonsterController>(FindObjectsInactive.Exclude);
            for (int i = 0; i < monsters.Length; i++)
            {
                NightMonsterController monster = monsters[i];
                if (monster == null || monster.IsDead) continue;

                Vector2 toMonster = (Vector2)(monster.transform.position - attackOrigin);
                float dist = toMonster.magnitude;
                if (dist <= range)
                {
                    float angle = Vector2.Angle(attackDir, toMonster.normalized);
                    if (angle <= 75f)
                    {
                        monster.TakeDamage(damage);
                    }
                }
            }
        }

        public void GetEquippedWeaponStats(out int damage, out float range, out float knockback, out Color slashColor, out DamageType damageType)
        {
            // Default unarmed
            damage = 1;
            range = 0.95f;
            knockback = 2.0f;
            slashColor = new Color(1f, 1f, 1f, 0.85f);
            damageType = DamageType.Physical;

            string currentItemId = GetSelectedItemId();
            if (string.IsNullOrEmpty(currentItemId)) return;

            switch (currentItemId)
            {
                case "item.weapon-sword":
                    damage = 7;
                    range = 1.55f;
                    knockback = 4.2f;
                    slashColor = new Color(1f, 0.88f, 0.40f, 0.95f);
                    damageType = DamageType.Slashing;
                    break;
                case "item.tool-axe":
                    damage = 4;
                    range = 1.35f;
                    knockback = 3.5f;
                    slashColor = new Color(0.9f, 0.95f, 1f, 0.9f);
                    damageType = DamageType.Slashing;
                    break;
                case "item.tool-pickaxe":
                    damage = 3;
                    range = 1.15f;
                    knockback = 4.5f;
                    slashColor = new Color(0.78f, 0.82f, 0.88f, 0.9f);
                    damageType = DamageType.Blunt;
                    break;
                case "item.torch":
                    damage = 2;
                    range = 1.05f;
                    knockback = 2.2f;
                    slashColor = new Color(1f, 0.65f, 0.2f, 0.95f);
                    damageType = DamageType.Fire;
                    break;
            }
        }

        public bool HasShield()
        {
            TheOldRoad.Core.VerticalSliceController vsc = FindAnyObjectByType<TheOldRoad.Core.VerticalSliceController>();
            return vsc != null && vsc.Inventory != null && vsc.Inventory.GetQuantity("item.shield-wood") > 0;
        }

        private string GetSelectedItemId()
        {
            TheOldRoad.Core.VerticalSliceController vsc = FindAnyObjectByType<TheOldRoad.Core.VerticalSliceController>();
            if (vsc != null && vsc.Inventory != null)
            {
                if (vsc.Inventory.GetQuantity("item.weapon-sword") > 0) return "item.weapon-sword";
                if (vsc.Inventory.GetQuantity("item.weapon-bow") > 0) return "item.weapon-bow";
                if (vsc.Inventory.GetQuantity("item.tool-axe") > 0) return "item.tool-axe";
                if (vsc.Inventory.GetQuantity("item.tool-pickaxe") > 0) return "item.tool-pickaxe";
                if (vsc.Inventory.GetQuantity("item.torch") > 0) return "item.torch";
            }
            return string.Empty;
        }

        public void TakeDamageFromEnemy(int damage, Vector2 knockbackDir)
        {
            if (isInvincible || vitals == null) return;

            if (isBlocking)
            {
                // Shield blocks 75% damage
                damage = Mathf.Max(1, Mathf.RoundToInt(damage * 0.25f));
                TheOldRoad.Audio.AudioManager.PlayHitImpact();
                FloatingTextController.Spawn("BLOCKED", transform.position + Vector3.up * 0.7f, Color.cyan, 0.8f);
            }

            vitals.TakeDamage(damage);
            FloatingTextController.SpawnPlayerDamage(damage, transform.position);

            // Apply knockback to player
            transform.position += (Vector3)(knockbackDir.normalized * 0.35f);

            if (vitals.CurrentHealth <= 0)
            {
                TheOldRoad.Core.VerticalSliceController vsc = FindAnyObjectByType<TheOldRoad.Core.VerticalSliceController>();
                if (vsc != null)
                {
                    vsc.OnPlayerDied();
                }
            }
            else
            {
                StartCoroutine(InvincibilityRoutine());
            }
        }

        private IEnumerator InvincibilityRoutine()
        {
            isInvincible = true;
            float elapsed = 0f;
            Color baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

            while (elapsed < iFrameDuration)
            {
                elapsed += 0.08f;
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 0.4f);
                }
                yield return new WaitForSeconds(0.04f);
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = baseColor;
                }
                yield return new WaitForSeconds(0.04f);
            }

            if (spriteRenderer != null) spriteRenderer.color = baseColor;
            isInvincible = false;
        }
    }
}
