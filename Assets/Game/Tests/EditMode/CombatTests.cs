using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Combat;
using TheOldRoad.Player;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class CombatTests
    {
        [Test]
        public void DamageInfo_InitializesWithCorrectParameters()
        {
            Vector2 dir = new Vector2(1f, 1f);
            DamageInfo info = new DamageInfo(5, dir, 3.5f, null, DamageType.Slashing);

            Assert.AreEqual(5, info.Amount);
            Assert.AreEqual(dir.normalized, info.Direction);
            Assert.AreEqual(3.5f, info.KnockbackForce);
            Assert.AreEqual(DamageType.Slashing, info.Type);
        }

        [Test]
        public void EnemyDefinition_ConfiguresPropertiesAndLootTableCorrectly()
        {
            EnemyDefinition def = ScriptableObject.CreateInstance<EnemyDefinition>();
            EnemyLootEntry[] loot = new[]
            {
                new EnemyLootEntry { itemId = "item.wool", minQuantity = 1, maxQuantity = 2, dropChance = 0.8f }
            };

            def.ConfigureForPrototype("enemy.wolf", "Wolf", 15, 2.5f, 4, 1.0f, 6.0f, 1.2f, loot);

            Assert.AreEqual("enemy.wolf", def.EnemyId);
            Assert.AreEqual("Wolf", def.DisplayName);
            Assert.AreEqual(15, def.MaxHealth);
            Assert.AreEqual(2.5f, def.MoveSpeed);
            Assert.AreEqual(4, def.AttackDamage);
            Assert.AreEqual(1.0f, def.AttackRange);
            Assert.AreEqual(6.0f, def.DetectionRadius);
            Assert.AreEqual(1.2f, def.AttackCooldown);
            Assert.AreEqual(1, def.LootTable.Length);
            Assert.AreEqual("item.wool", def.LootTable[0].itemId);
        }

        [Test]
        public void PlayerVitals_TakeDamage_ReducesHealthCorrectly()
        {
            GameObject go = new GameObject("PlayerTest");
            PlayerVitals vitals = go.AddComponent<PlayerVitals>();
            vitals.Configure(20, 20);

            vitals.TakeDamage(6);
            Assert.AreEqual(14, vitals.CurrentHealth);

            vitals.TakeDamage(25);
            Assert.AreEqual(0, vitals.CurrentHealth);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PlayerVitals_TryConsumeFood_HealsCorrectlyWhenDamaged()
        {
            GameObject go = new GameObject("PlayerTest");
            PlayerVitals vitals = go.AddComponent<PlayerVitals>();
            vitals.Configure(20, 10); // Start with 10/20 HP

            // Eat Wild Berries (+2)
            bool ateBerries = vitals.TryConsumeFood("item.wild-berries", out int berriesHealed);
            Assert.IsTrue(ateBerries);
            Assert.AreEqual(2, berriesHealed);
            Assert.AreEqual(12, vitals.CurrentHealth);

            // Eat Medicinal Herb (+5)
            bool ateHerb = vitals.TryConsumeFood("item.medicinal-herb", out int herbHealed);
            Assert.IsTrue(ateHerb);
            Assert.AreEqual(5, herbHealed);
            Assert.AreEqual(17, vitals.CurrentHealth);

            // Eat Cooked Meal (+12, capped at max 20)
            bool ateMeal = vitals.TryConsumeFood("item.cooked-meal", out int mealHealed);
            Assert.IsTrue(ateMeal);
            Assert.AreEqual(12, mealHealed);
            Assert.AreEqual(20, vitals.CurrentHealth);

            // Try eat when at max health
            bool ateFull = vitals.TryConsumeFood("item.cooked-meal", out int fullHealed);
            Assert.IsFalse(ateFull);
            Assert.AreEqual(0, fullHealed);
            Assert.AreEqual(20, vitals.CurrentHealth);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PrototypePixelArtFactory_GeneratesCombatSpritesSuccessfully()
        {
            Sprite slash = PrototypePixelArtFactory.SlashArcSprite;
            Assert.IsNotNull(slash);
            Assert.AreEqual(24, slash.texture.width);
            Assert.AreEqual(24, slash.texture.height);

            for (int f = 0; f < 4; f++)
            {
                Sprite wolf = PrototypePixelArtFactory.WolfSprite(f);
                Assert.IsNotNull(wolf, $"Wolf sprite frame {f} should not be null");

                Sprite bandit = PrototypePixelArtFactory.BanditSprite(f);
                Assert.IsNotNull(bandit, $"Bandit sprite frame {f} should not be null");

                Sprite stalker = PrototypePixelArtFactory.ShadowStalkerSprite(f);
                Assert.IsNotNull(stalker, $"Shadow Stalker sprite frame {f} should not be null");
            }
        }

        [Test]
        public void CombatWeaponItems_AreDefinedInItemCatalog()
        {
            Assert.IsTrue(TheOldRoad.Items.PrototypeItemCatalog.TryGet("item.weapon-sword", out var sword));
            Assert.AreEqual("item.weapon-sword", sword.ItemId);

            Assert.IsTrue(TheOldRoad.Items.PrototypeItemCatalog.TryGet("item.weapon-bow", out var bow));
            Assert.AreEqual("item.weapon-bow", bow.ItemId);

            Assert.IsTrue(TheOldRoad.Items.PrototypeItemCatalog.TryGet("item.ammo-arrow", out var arrow));
            Assert.AreEqual("item.ammo-arrow", arrow.ItemId);

            Assert.IsTrue(TheOldRoad.Items.PrototypeItemCatalog.TryGet("item.shield-wood", out var shield));
            Assert.AreEqual("item.shield-wood", shield.ItemId);

            Assert.IsTrue(TheOldRoad.Items.PrototypeItemCatalog.TryGet("item.armor-knight", out var armor));
            Assert.AreEqual("item.armor-knight", armor.ItemId);
        }
    }
}
