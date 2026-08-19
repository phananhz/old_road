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
            TheOldRoad.Audio.AudioManager.PlayPlayerHurt();
        }

        public void TakeDamage(int amount)
        {
            Damage(amount);
        }

        public bool TryConsumeFood(string itemId, out int healedAmount)
        {
            healedAmount = 0;
            switch (itemId)
            {
                case "item.wild-berries":
                    healedAmount = 3;
                    break;
                case "item.medicinal-herb":
                    healedAmount = 6;
                    break;
                case "item.mushroom":
                    healedAmount = 4;
                    break;
                case "item.cooked-meal":
                    healedAmount = 15;
                    break;
                case "item.cooked-fish":
                    healedAmount = 16;
                    break;
                case "item.egg":
                    healedAmount = 3;
                    break;
                case "item.milk":
                    healedAmount = 5;
                    break;
                case "item.carrot":
                    healedAmount = 5;
                    break;
                case "item.potato":
                    healedAmount = 6;
                    break;
                case "item.corn":
                    healedAmount = 5;
                    break;
                case "item.tomato":
                    healedAmount = 5;
                    break;
                case "item.pineapple":
                    healedAmount = 8;
                    break;
                case "item.strawberry":
                    healedAmount = 6;
                    break;
                case "item.apple":
                    healedAmount = 7;
                    break;
                case "item.grape":
                    healedAmount = 6;
                    break;
                case "item.pumpkin":
                    healedAmount = 10;
                    break;
                case "item.cheese":
                    healedAmount = 14;
                    break;
                case "item.wine-fruit":
                    healedAmount = 8;
                    break;
                case "item.juice":
                    healedAmount = 8;
                    break;
                default:
                    return false;
            }

            if (currentHealth >= maxHealth && healedAmount > 0)
            {
                // Already full health
                return false;
            }

            Heal(healedAmount);
            return true;
        }
    }
}
