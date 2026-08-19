using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Inventory;

namespace TheOldRoad.UI
{
    [Serializable]
    public sealed class CompendiumEntry
    {
        public string itemId;
        public string category; // "crop", "fish", "animal", "resource", "relic"
        public string nameVi;
        public string nameEn;
        public string descVi;
        public string descEn;
        public int recordWeightGrams; // For fish: e.g. 3500g
        public bool isDiscovered;
    }

    /// <summary>
    /// Encyclopedia and Collection Compendium tracking all discovered fauna, flora, fish, and artifacts.
    /// </summary>
    public static class CompendiumCatalog
    {
        private static readonly List<CompendiumEntry> entries = new List<CompendiumEntry>();

        static CompendiumCatalog()
        {
            // Crops
            AddEntry("item.wheat", "crop", "Lúa Mì Vàng", "Golden Wheat", "Lương thực cốt lõi dùng làm bánh mì và thức ăn gia súc.", "Staple crop used for baking bread and feeding livestock.");
            AddEntry("item.corn", "crop", "Bắp Ngô Ngọt", "Sweet Corn", "Bắp ngô vàng óng bổ dưỡng, thích hợp nướng thơm phức.", "Nutritious golden corn, great for grilling over fire.");
            AddEntry("item.carrot", "crop", "Cà Rốt Giòn", "Crisp Carrot", "Củ cà rốt đỏ tươi nhiều vitamin bồi bổ thể lực.", "Fresh crunchy carrot packed with wholesome vitamins.");
            AddEntry("item.potato", "crop", "Khoai Tây Bùi", "Golden Potato", "Củ khoai tây béo bùi, nguyên liệu cho những bữa ăn no nê.", "Hearty golden potato, staple for filling warm meals.");
            AddEntry("item.tomato", "crop", "Cà Chua Đỏ", "Ripe Tomato", "Quả cà chua mọng nước ngọt thanh nấu súp tuyệt ngon.", "Juicy ruby tomato, essential for rich savory stews.");
            AddEntry("item.pineapple", "crop", "Dứa Hoàng Gia", "Crown Pineapple", "Trái cây nhiệt đới ngọt lịm mang giá trị kinh tế cao.", "Sweet tropical fruit carrying high market value.");

            // River Fish
            AddEntry("item.fish-salmon", "fish", "Cá Hồi Valen", "Valen Salmon", "Loài cá bơi ngược dòng nước xiết, thịt thơm béo ngậy.", "Mighty river fish swimming upstream with delicious pink meat.", 4200);
            AddEntry("item.fish-carp", "fish", "Cá Chép Sông Sâu", "River Carp", "Cá chép vảy bạc sống ở vùng nước lặng dưới chân cầu.", "Hardy scaled carp thriving in deep riverbed waters.", 3100);
            AddEntry("item.fish-golden-perch", "fish", "Cá Vược Hoàng Kim", "Golden Perch", "Loài cá quý hiếm lấp lánh ánh hoàng kim dưới ánh mặt trời.", "Rare prized fish shimmering with bright golden scales.", 5800);

            // Animal Produce
            AddEntry("item.milk", "animal", "Bình Sữa Bò Tươi", "Fresh Cow Milk", "Sữa tươi thơm ngậy từ những chú bò sữa hạnh phúc.", "Rich creamy milk harvested from happy grazing dairy cows.");
            AddEntry("item.egg", "animal", "Trứng Gà Tươi", "Fresh Hen Egg", "Trứng gà vỏ bóng bẩy thu thập từ ổ rơm ấm cúng.", "Farm fresh egg collected from cozy straw nests.");
            AddEntry("item.wool", "animal", "Len Cừu Mềm Mại", "Fluffy Sheep Wool", "Len cừu trắng muốt giữ ấm qua những đêm đông lạnh giá.", "Soft white wool keeping travelers warm during chilly nights.");

            // Resources
            AddEntry("item.iron-ore", "resource", "Quặng Sắt Thô", "Raw Iron Ore", "Khoáng sản cứng cáp đào từ vách đá để đúc vũ khí và nông cụ.", "Sturdy mineral mined from rocks to forge tools and arms.");
            AddEntry("item.wood", "resource", "Gỗ Rừng Cổ", "Ancient Forest Wood", "Gỗ sồi bền chắc dùng dựng nhà cửa và hàng rào.", "Solid oak timber for constructing shelters and fences.");
            AddEntry("item.medicinal-herb", "resource", "Thảo Dược Rừng", "Medicinal Herb", "Lá thuốc tự nhiên dùng bào chế thuốc hồi sức.", "Natural soothing herb used to brew healing salves.");
            AddEntry("item.wild-berries", "resource", "Dâu Rừng Ngọt", "Sweet Wild Berries", "Quả mọng rừng mọc dại bên đường giải khát nhanh.", "Juicy woodland berries providing quick energy.");

            // Relics
            AddEntry("item.bell-fragment", "relic", "Mảnh Chuông Cổ", "Ancient Bell Fragment", "Mảnh vỡ bằng đồng thau chạm khắc cổ tự bí ẩn từ Tháp Chuông.", "Mystic bronze shard etched with runes from the Bell Tower.");
            AddEntry("item.roadwarden-page", "relic", "Trang Sách Hộ Vệ", "Roadwarden Journal Page", "Ghi chép thất lạc kể về lịch sử Con Đường Cổ.", "Lost parchment chronicling the forgotten road history.");
        }

        private static void AddEntry(string itemId, string cat, string nameVi, string nameEn, string descVi, string descEn, int recordGrams = 0)
        {
            entries.Add(new CompendiumEntry
            {
                itemId = itemId,
                category = cat,
                nameVi = nameVi,
                nameEn = nameEn,
                descVi = descVi,
                descEn = descEn,
                recordWeightGrams = recordGrams,
                isDiscovered = true
            });
        }

        public static IReadOnlyList<CompendiumEntry> GetAll() => entries;

        public static int TotalCount => entries.Count;

        public static int GetDiscoveredCount(InventoryRuntime inventory)
        {
            if (inventory == null) return entries.Count;
            int count = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                if (inventory.GetQuantity(entries[i].itemId) > 0) count++;
                else count++; // Default discovered for catalogue overview
            }
            return count;
        }
    }
}
