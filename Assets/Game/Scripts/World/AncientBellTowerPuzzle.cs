using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.Input;

namespace TheOldRoad.World
{
    /// <summary>
    /// Ancient Bell Tower Ruins Puzzle.
    /// Player solves a 3-symbol pedestal puzzle (Sun -> Moon -> Star) to unlock the ancient sacred chest.
    /// </summary>
    public sealed class AncientBellTowerPuzzle : MonoBehaviour
    {
        [SerializeField] private Vector3 towerPosition = new Vector3(22f, 18f, 0f);
        [SerializeField] private bool isSolved;

        private readonly List<int> currentInputSequence = new List<int>();
        private readonly int[] correctSequence = { 0, 1, 2 }; // 0: Sun, 1: Moon, 2: Star

        private GameObject towerObj;
        private GameObject[] pedestalObjs = new GameObject[3];
        private SpriteRenderer[] pedestalRenderers = new SpriteRenderer[3];
        private bool[] pedestalStates = new bool[3];
        private GameObject chestObj;
        private bool chestOpened;

        public bool IsSolved => isSolved;
        public string InteractionHint { get; private set; } = string.Empty;

        private void Start()
        {
            InitializeRuins();
        }

        private void InitializeRuins()
        {
            // Tower Ruins Base
            towerObj = new GameObject("BellTowerRuins");
            towerObj.transform.SetParent(transform, false);
            towerObj.transform.position = towerPosition;
            SpriteRenderer tr = towerObj.AddComponent<SpriteRenderer>();
            tr.sprite = PrototypePixelArtFactory.BellTowerRuins();
            tr.sortingOrder = 50;

            // 3 Pedestals around the tower
            Vector3[] offsets = {
                new Vector3(-2.8f, -1.8f, 0f), // Sun Pedestal
                new Vector3(0f, -2.6f, 0f),     // Moon Pedestal
                new Vector3(2.8f, -1.8f, 0f)    // Star Pedestal
            };

            for (int i = 0; i < 3; i++)
            {
                int index = i;
                pedestalObjs[i] = new GameObject("Pedestal_" + i);
                pedestalObjs[i].transform.SetParent(transform, false);
                pedestalObjs[i].transform.position = towerPosition + offsets[i];
                pedestalRenderers[i] = pedestalObjs[i].AddComponent<SpriteRenderer>();
                pedestalRenderers[i].sprite = PrototypePixelArtFactory.PuzzlePedestal(index, false);
                pedestalRenderers[i].sortingOrder = 60;
                pedestalStates[i] = false;
            }

            // Sealed Chest in center
            chestObj = new GameObject("AncientPuzzleChest");
            chestObj.transform.SetParent(transform, false);
            chestObj.transform.position = towerPosition + new Vector3(0f, -0.6f, 0f);
            SpriteRenderer cr = chestObj.AddComponent<SpriteRenderer>();
            cr.sprite = PrototypePixelArtFactory.ChestClosed();
            cr.sortingOrder = 55;
        }

        private void Update()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            Vector3 playerPos = player.transform.position;

            // Check chest interaction if solved
            if (isSolved && !chestOpened)
            {
                float distChest = Vector2.Distance(playerPos, chestObj.transform.position);
                if (distChest <= 1.8f)
                {
                    InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Mở Rương Cổ Đại" : "[F] Open Ancient Chest";
                    if (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E))
                    {
                        OpenAncientChest();
                    }
                    return;
                }
            }

            // Check pedestal interactions
            InteractionHint = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                if (pedestalObjs[i] == null) continue;
                float dist = Vector2.Distance(playerPos, pedestalObjs[i].transform.position);
                if (dist <= 1.5f && !isSolved)
                {
                    string symbolName = (i == 0) ? (LocalizationRuntime.IsVietnamese ? "Thái Dương ☀️" : "Sun ☀️") :
                                        (i == 1) ? (LocalizationRuntime.IsVietnamese ? "Nguyệt Quang 🌙" : "Moon 🌙") :
                                                   (LocalizationRuntime.IsVietnamese ? "Tinh Tú ⭐" : "Star ⭐");

                    InteractionHint = LocalizationRuntime.IsVietnamese ? $"[F] Kích hoạt Bệ đá ({symbolName})" : $"[F] Activate Pedestal ({symbolName})";

                    if (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E))
                    {
                        ActivatePedestal(i);
                    }
                    return;
                }
            }
        }

        private void ActivatePedestal(int index)
        {
            if (isSolved) return;

            pedestalStates[index] = true;
            pedestalRenderers[index].sprite = PrototypePixelArtFactory.PuzzlePedestal(index, true);
            currentInputSequence.Add(index);
            AudioManager.PlayUiClick();

            // Check if matches sequence
            int stepIndex = currentInputSequence.Count - 1;
            if (currentInputSequence[stepIndex] != correctSequence[stepIndex])
            {
                // Wrong sequence -> reset
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? "Biểu tượng phát sáng rồi vụt tắt... (Sai thứ tự!)" : "The runes flicker and reset! (Wrong order)");
                ResetPedestals();
                return;
            }

            // If completed all 3
            if (currentInputSequence.Count == 3)
            {
                isSolved = true;
                AudioManager.PlayDoorTransition();
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? "✨ Phong ấn Tháp Chuông đã được giải mã! Rương cổ đại mở khóa!" : "✨ Bell Tower seal shattered! Ancient Chest unlocked!");
            }
            else
            {
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? "Bệ đá phát sáng năng lượng cổ..." : "The ancient stone hums with light...");
            }
        }

        private void ResetPedestals()
        {
            currentInputSequence.Clear();
            for (int i = 0; i < 3; i++)
            {
                pedestalStates[i] = false;
                if (pedestalRenderers[i] != null)
                {
                    pedestalRenderers[i].sprite = PrototypePixelArtFactory.PuzzlePedestal(i, false);
                }
            }
        }

        private void OpenAncientChest()
        {
            chestOpened = true;
            if (chestObj != null)
            {
                chestObj.GetComponent<SpriteRenderer>().sprite = PrototypePixelArtFactory.ChestOpen();
            }

            AudioManager.PlayChestOpen();

            InventorySession session = FindAnyObjectByType<InventorySession>();
            if (session != null && session.Runtime != null)
            {
                session.Runtime.Add("item.silver-coin", 15);
                session.Runtime.Add("item.bell-fragment", 1);
                session.Runtime.Add("item.iron-ore", 3);
                session.Runtime.Add("item.seed-corn", 2);
                session.Runtime.Add("item.seed-carrot", 2);
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? "Nhận được: 15 Đồng bạc, Mảnh Chuông Cổ, 3 Quặng sắt & Hạt giống quý!" : "Looted: 15 Silver Coins, Bell Fragment, 3 Iron Ore & Rare Seeds!");
            }
        }
    }
}
