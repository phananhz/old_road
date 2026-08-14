using UnityEngine;

namespace TheOldRoad.Player
{
    /// <summary>Prototype player health model for HUD and future combat integration.</summary>
    public sealed class PlayerVitals : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 20;
        [SerializeField, Min(0)] private int currentHealth = 20;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => Mathf.Clamp(currentHealth, 0, maxHealth);
        public float Health01 => maxHealth <= 0 ? 0f : CurrentHealth / (float)maxHealth;

        public void Configure(int maxHealth, int currentHealth)
        {
            this.maxHealth = Mathf.Max(1, maxHealth);
            this.currentHealth = Mathf.Clamp(currentHealth, 0, this.maxHealth);
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        }

        public void Damage(int amount)
        {
            if (amount <= 0) return;
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        }
    }
}
