using UnityEngine;
using TheOldRoad.Player;
using TheOldRoad.World;
using TheOldRoad.Audio;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.Time;

namespace TheOldRoad.Combat
{
    /// <summary>
    /// Shadow beast / Night monster that roams the forest at night.
    /// Weakened and slowed when entering bright light (campfires, hearths, lit cabins, player torch).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public sealed class NightMonsterController : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 18;
        [SerializeField, Min(0.1f)] private float normalSpeed = 2.4f;
        [SerializeField, Min(0.1f)] private float lightSlowedSpeed = 1.0f;
        [SerializeField, Min(0.5f)] private float aggroRange = 7.5f;
        [SerializeField, Min(0.2f)] private float attackRange = 1.2f;
        [SerializeField] private int attackDamage = 4;
        [SerializeField, Min(0.2f)] private float attackCooldown = 1.6f;

        private int currentHealth;
        private SpriteRenderer spriteRenderer;
        private Transform playerTarget;
        private PlayerVitals playerVitals;
        private float nextAttackTime;
        private float frameTimer;
        private int frameIndex;
        private float hitFlashTimer;
        private bool isLightWeakened;

        public bool IsDead => currentHealth <= 0;
        public bool IsLightWeakened => isLightWeakened;

        private void Awake()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
            GameObject player = GameObject.FindWithTag("Player");
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
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    playerTarget = player.transform;
                    playerVitals = player.GetComponent<PlayerVitals>();
                }
                return;
            }

            // Check light proximity
            isLightWeakened = CheckIfInLightSource();

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= aggroRange)
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                float currentSpeed = isLightWeakened ? lightSlowedSpeed : normalSpeed;

                if (distanceToPlayer > attackRange)
                {
                    transform.position += (Vector3)(direction * (currentSpeed * UnityEngine.Time.deltaTime));
                    if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.05f)
                    {
                        spriteRenderer.flipX = direction.x < 0f;
                    }
                }
                else if (UnityEngine.Time.time >= nextAttackTime)
                {
                    PerformAttack();
                }
            }

            // Animation
            frameTimer += UnityEngine.Time.deltaTime;
            if (frameTimer >= 0.16f)
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % 4;
                if (spriteRenderer != null && hitFlashTimer <= 0f)
                {
                    spriteRenderer.sprite = PrototypePixelArtFactory.NightMonsterSprite(frameIndex);
                }
            }

            // Hit flash
            if (hitFlashTimer > 0f)
            {
                hitFlashTimer -= UnityEngine.Time.deltaTime;
                if (hitFlashTimer <= 0f && spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            // Extra damage if weakened by light
            int finalDamage = isLightWeakened ? Mathf.RoundToInt(damage * 1.5f) : damage;
            currentHealth -= finalDamage;
            hitFlashTimer = 0.15f;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
            }

            AudioManager.PlayAttackImpact();
            FloatingTextController.Spawn("-" + finalDamage, transform.position + Vector3.up * 0.8f, isLightWeakened ? new Color(1f, 0.9f, 0.3f, 1f) : new Color(1f, 0.3f, 0.3f, 1f));

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void PerformAttack()
        {
            nextAttackTime = UnityEngine.Time.time + attackCooldown;
            AudioManager.PlayAttackSwing();

            if (playerVitals != null)
            {
                int dmg = isLightWeakened ? Mathf.Max(1, attackDamage / 2) : attackDamage;
                playerVitals.TakeDamage(dmg);
                FloatingTextController.Spawn("-" + dmg, playerTarget.position + Vector3.up * 0.8f, new Color(0.9f, 0.2f, 0.2f, 1f));
            }
        }

        private void Die()
        {
            currentHealth = 0;
            AudioManager.PlayChestOpen();

            // Reward silver coins & items
            InventorySession session = FindAnyObjectByType<InventorySession>();
            if (session != null && session.Runtime != null)
            {
                int coins = UnityEngine.Random.Range(3, 7);
                session.Runtime.Add("item.silver-coin", coins);
                if (UnityEngine.Random.value > 0.5f)
                {
                    session.Runtime.Add("item.iron-ore", 1);
                }
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? $"Đã tiêu diệt Quái bóng đêm! (+{coins} Đồng bạc)" : $"Defeated Shadow Beast! (+{coins} Silver)");
            }

            Destroy(gameObject, 0.1f);
        }

        private bool CheckIfInLightSource()
        {
            // Check near campfires, cooking hearths, lit cabins
            GameObject[] lights = GameObject.FindGameObjectsWithTag("Respawn"); // fallback check
            Vector3 myPos = transform.position;

            // Check distance to player if player has torch equipped or near campfire/hearth
            if (playerTarget != null && Vector2.Distance(myPos, playerTarget.position) < 3.5f)
            {
                InventorySession session = FindAnyObjectByType<InventorySession>();
                if (session != null && session.Runtime != null && session.Runtime.GetQuantity("item.torch") > 0)
                {
                    return true;
                }
            }

            // Search nearby construction sites (campfires, cooking hearths)
            TheOldRoad.Construction.ConstructionSite[] sites = FindObjectsByType<TheOldRoad.Construction.ConstructionSite>(FindObjectsInactive.Exclude);
            for (int i = 0; i < sites.Length; i++)
            {
                if (sites[i] == null || !sites[i].IsCompleted) continue;
                string bId = sites[i].BuildingId;
                if (bId == "building.campfire" || bId == "building.cooking-hearth" || bId == "building.cabin" || bId == "building.stone-cottage" || bId == "building.lookout-tower")
                {
                    if (Vector2.Distance(myPos, sites[i].transform.position) < 6.5f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
