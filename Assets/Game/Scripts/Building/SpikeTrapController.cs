using UnityEngine;
using TheOldRoad.Combat;
using TheOldRoad.Audio;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Spike trap that triggers when an enemy steps within proximity, dealing damage and knocking them back.
    /// </summary>
    public sealed class SpikeTrapController : MonoBehaviour
    {
        [SerializeField] private int trapDamage = 40;
        [SerializeField] private float triggerRadius = 1.15f;
        [SerializeField] private float rearmDuration = 4f;

        private float nextArmTime;
        private bool isArmed = true;

        private void Update()
        {
            if (!isArmed)
            {
                if (UnityEngine.Time.time >= nextArmTime)
                {
                    isArmed = true;
                }
                return;
            }

            Vector3 trapPos = transform.position;
            var damageables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);

            for (int i = 0; i < damageables.Length; i++)
            {
                if (damageables[i] is IDamageable target && !(damageables[i] is PlayerCombatController))
                {
                    if (target.IsDead) continue;
                    float dist = Vector2.Distance(trapPos, target.Transform.position);
                    if (dist <= triggerRadius)
                    {
                        TriggerTrap(target);
                        break;
                    }
                }
            }
        }

        private void TriggerTrap(IDamageable target)
        {
            isArmed = false;
            nextArmTime = UnityEngine.Time.time + rearmDuration;

            Vector2 knockbackDir = (target.Transform.position - transform.position).normalized;
            var damageInfo = new DamageInfo(
                trapDamage,
                knockbackDir,
                3f,
                gameObject,
                DamageType.Physical
            );

            target.TakeDamage(damageInfo);
            AudioManager.PlaySlashVfx();
        }
    }
}
