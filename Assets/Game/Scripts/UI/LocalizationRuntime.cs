using System;
using UnityEngine;

namespace TheOldRoad.UI
{
    /// <summary>Comprehensive PlayerPrefs-backed localization layer for English and Vietnamese.</summary>
    public static class LocalizationRuntime
    {
        private const string LanguagePrefKey = "the_old_road.settings.language";

        public static int LanguageIndex { get; private set; }
        public static bool IsVietnamese => LanguageIndex == 1;
        public static string LanguageName => IsVietnamese ? "Tiếng Việt" : "English";

        public static void Load()
        {
            LanguageIndex = Mathf.Clamp(PlayerPrefs.GetInt(LanguagePrefKey, 0), 0, 1);
        }

        public static void SetLanguage(int index)
        {
            LanguageIndex = Mathf.Clamp(index, 0, 1);
            PlayerPrefs.SetInt(LanguagePrefKey, LanguageIndex);
            PlayerPrefs.Save();
        }

        public static string T(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (!IsVietnamese) return English(key);
            return Vietnamese(key);
        }

        public static string Objective(string english)
        {
            if (!IsVietnamese || string.IsNullOrWhiteSpace(english)) return english;

            switch (english)
            {
                case "Inspect an old-road landmark": return "Khám phá một địa danh trên Con Đường Cũ";
                case "Open an old chest": return "Mở một rương cũ";
                case "Recover Father's Roadwarden journal page": return "Tìm lại trang nhật ký Roadwarden của cha";
                case "Speak with a village NPC": return "Nói chuyện với một dân làng";
                case "Craft a worn axe": return "Chế tạo rìu cũ";
                case "Gather 3 wood": return "Thu thập 3 gỗ";
                case "Gather 2 stone": return "Thu thập 2 đá";
                case "Craft a stone pick": return "Chế tạo cuốc đá";
                case "Mine iron ore": return "Khai thác quặng sắt";
                case "Forage any wild food or herb": return "Hái thức ăn hoặc thảo dược hoang dã";
                case "Craft 1 cabin plank": return "Chế tạo 1 ván gỗ cabin";
                case "Start cabin construction": return "Bắt đầu xây cabin";
                case "Complete the first cabin": return "Hoàn thành cabin đầu tiên";
                case "Find the first bell fragment": return "Tìm mảnh chuông đầu tiên";
                case "Build a campfire": return "Xây bếp lửa";
                case "Cook one meal": return "Nấu một bữa ăn";
                case "Build an animal pen": return "Xây chuồng nuôi";
                case "Find a cave entrance": return "Tìm một cửa hang";
                case "Read the dragon-scarred ridge": return "Đọc dấu tích rồng trên sườn núi";
                default: return english;
            }
        }

        public static string ItemName(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return string.Empty;
            string key = itemId.StartsWith("item.") ? itemId : "item." + itemId;
            return T(key);
        }

        public static string ItemCategory(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return string.Empty;
            switch (itemId)
            {
                case "item.wood":
                case "item.stone":
                case "item.cabin-plank":
                case "item.iron-ore":
                    return IsVietnamese ? "Nguyên liệu xây dựng" : "Building Material";
                case "item.tool-axe":
                case "item.tool-pickaxe":
                case "item.tool-hoe":
                case "item.weapon-sword":
                case "item.weapon-bow":
                case "item.ammo-arrow":
                case "item.shield-wood":
                case "item.armor-knight":
                case "item.fishing-rod":
                case "item.watering-can":
                case "item.torch":
                    return IsVietnamese ? "Công cụ & Vũ khí" : "Tools & Weapons";
                case "item.wild-berries":
                case "item.mushroom":
                case "item.cooked-meal":
                case "item.cooked-fish":
                    return IsVietnamese ? "Thực phẩm hồi sức" : "Consumable Food";
                case "item.medicinal-herb":
                    return IsVietnamese ? "Dược liệu & Bào chế" : "Herbal & Apothecary";
                case "item.old-coin":
                case "item.silver-coin":
                    return IsVietnamese ? "Tiền tệ & Trao đổi" : "Currency & Trade";
                case "item.roadwarden-page":
                case "item.bell-fragment":
                case "item.farm-deed":
                    return IsVietnamese ? "Cổ vật & Giấy tờ" : "Artifacts & Deeds";
                case "item.seed-wheat":
                case "item.seed-corn":
                case "item.seed-carrot":
                case "item.seed-potato":
                case "item.seed-pineapple":
                case "item.seed-tomato":
                case "item.fishing-bait":
                    return IsVietnamese ? "Hạt giống & Mồi câu" : "Seeds & Bait";
                case "item.wheat":
                case "item.corn":
                case "item.carrot":
                case "item.potato":
                case "item.pineapple":
                case "item.tomato":
                case "item.egg":
                case "item.wool":
                case "item.milk":
                case "item.meat-raw":
                case "item.leather":
                case "item.hay":
                case "item.grape":
                case "item.pumpkin":
                case "item.flour":
                case "item.fish-salmon":
                case "item.fish-carp":
                case "item.fish-golden-perch":
                    return IsVietnamese ? "Nông sản & Thủy sản" : "Produce & Fish";
                case "item.cheese":
                case "item.wine":
                case "item.juice":
                    return IsVietnamese ? "Đồ uống & Thành phẩm" : "Beverage & Artisan Food";
                case "item.cloth":
                case "item.iron-bar":
                case "item.fertilizer":
                    return IsVietnamese ? "Nguyên liệu chế tác" : "Crafting Material";
                case "item.fence-wood":
                case "item.gate-wood":
                    return IsVietnamese ? "Vật phẩm kiến trúc" : "Structure Part";
                default:
                    return IsVietnamese ? "Vật phẩm thông thường" : "Standard Item";
            }
        }

        public static string ItemDescription(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return string.Empty;
            if (IsVietnamese)
            {
                switch (itemId)
                {
                    case "item.wood": return "Gỗ sồi chắc chắn thu thập từ các cây đại thụ. Dùng trong chế tạo công cụ, nhóm lửa và dựng các công trình cơ bản.";
                    case "item.stone": return "Đá tảng cứng cáp khai thác từ các mỏ đá ven đường. Dùng làm móng kiên cố cho nhà cửa và công trình đá.";
                    case "item.cabin-plank": return "Ván gỗ dày dặn được xẻ và vát mịn cẩn thận. Vật liệu cốt lõi để dựng cabin và rương chứa đồ.";
                    case "item.tool-axe": return "Rìu tiều phu đã qua sử dụng nhưng lưỡi vẫn sắc bén. Cho phép chặt gỗ nhanh hơn và khai hoang các rào chắn.";
                    case "item.tool-pickaxe": return "Cuốc đá thô sơ dùng để đào đá và khai thác các vỉa quặng sắt quý giá ẩn sâu trong lòng đất.";
                    case "item.iron-ore": return "Quặng sắt nguyên chất chứa kim loại cứng cáp. Cần thiết để rèn trang bị, vũ khí và công cụ cao cấp.";
                    case "item.wild-berries": return "Những chùm quả mọng đỏ ngọt mọc tự nhiên. Có thể ăn ngay (nhấn Q) để hồi máu hoặc dùng nấu nướng.";
                    case "item.medicinal-herb": return "Thảo dược hoang dã tỏa hương thơm thanh mát. Dùng để bào chế thuốc và nấu các bữa ăn bổ dưỡng.";
                    case "item.mushroom": return "Nấm rừng béo ngậy mọc dưới tán lá ẩm ướt. Gia vị tuyệt hảo cho các món hầm trên bếp lửa.";
                    case "item.cooked-meal": return "Bữa ăn nóng sốt nấu từ quả dại và nấm thảo mộc. Hồi phục lượng lớn sinh lực khi tiêu thụ (nhấn Q).";
                    case "item.torch": return "Ngọn đuốc tẩm dầu sáng rực. Xua tan bóng đêm u tối và bảo vệ người lữ hành khỏi quái vật săn đêm.";
                    case "item.roadwarden-page": return "Trang nhật ký ố vàng của cha - người Roadwarden tiền nhiệm, ghi lại các bí mật về Con Đường Cũ.";
                    case "item.bell-fragment": return "Mảnh vỡ kim loại cổ phát ra âm vang vi vu bí ẩn, tàn tích của mạng lưới tháp chuông xưa.";
                    case "item.silver-coin": return "Đồng bạc chạm khắc gia huy vương quốc. Tiền tệ chính để giao thương mua bán vật phẩm với thương nhân.";
                    case "item.old-coin": return "Đồng xu cổ xỉn màu có niên đại hàng thế kỷ từ thời kỳ Con Đường Cũ hưng thịnh.";
                    case "item.tool-hoe": return "Cuốc làm vườn cán gỗ cứng cáp. Dùng để xới các ô đất hoang thành luống đất màu mỡ sẵn sàng gieo hạt giống.";
                    case "item.watering-can": return "Bình tưới nước bằng sắt. Dùng để tưới ẩm các luống đất đã cuốc giúp cây trồng sinh trưởng nhanh.";
                    case "item.seed-wheat": return "Hạt giống lúa mì chọn lọc. Gieo vào đất đã xới để thu hoạch những bông lúa mì vàng óng.";
                    case "item.seed-corn": return "Hạt bắp giống chất lượng. Gieo trồng trên đất ẩm để thu hoạch những bắp ngô ngọt bùi.";
                    case "item.seed-carrot": return "Hạt giống cà rốt tươi tốt. Trồng và tưới nước đều đặn để thu hoạch củ cà rốt giòn ngọt.";
                    case "item.seed-potato": return "Mầm khoai tây giống khỏe mạnh. Vùi xuống đất tơi xốp và tưới ẩm để thu hoạch những củ khoai tây bùi ngậy.";
                    case "item.seed-pineapple": return "Hạt giống dứa nhiệt đới quý hiếm. Trồng để thu hoạch quả dứa vàng mật thơm ngon.";
                    case "item.seed-tomato": return "Hạt giống cà chua đỏ mọng. Dễ trồng và cho năng suất quả cao.";
                    case "item.wheat": return "Bông lúa mì vàng óng ả vừa thu hoạch. Nguyên liệu quan trọng cho các bữa ăn no nê.";
                    case "item.corn": return "Bắp ngô vàng ngọt bùi đậm đà hương vị đồng quê.";
                    case "item.carrot": return "Củ cà rốt tươi ngon bổ dưỡng, có thể dùng làm thức ăn trực tiếp hoặc nấu nướng.";
                    case "item.potato": return "Củ khoai tây vàng ươm bùi béo vừa nhổ lên từ luống đất. Lương thực no lâu và dễ chế biến.";
                    case "item.pineapple": return "Quả dứa vàng mật ngọt ngào, chứa nhiều nước và giá trị dinh dưỡng cao.";
                    case "item.tomato": return "Quả cà chua mọng nước, gia vị tuyệt vời cho các bữa ăn ấm áp.";
                    case "item.egg": return "Trứng gà tươi thu gom từ ổ rơm chuồng nuôi. Thực phẩm giàu đạm.";
                    case "item.wool": return "Bó len mềm mịn thu hoạch từ cừu. Nguyên liệu dệt may và lót ấm.";
                    case "item.milk": return "Bình sữa tươi nguyên chất từ bò sữa Valen. Thức uống bổ dưỡng hồi phục thể lực.";
                    case "item.fence-wood": return "Đoạn hàng rào gỗ mộc mạc dùng để bao quanh và bảo vệ khuôn viên nông trại.";
                    case "item.gate-wood": return "Cánh cổng rào gỗ có then cài. Cho phép đóng mở lối ra vào chuồng trại.";
                    case "item.fishing-rod": return "Cần câu làm từ thân tre dẻo dai. Dùng để thả câu ven các khúc sông Valen yên ả.";
                    case "item.fishing-bait": return "Trùn đất tươi béo ngậy đào từ đất ẩm. Mồi câu khoái khẩu của các loài cá sông.";
                    case "item.fish-salmon": return "Cá hồi sông tươi rói vừa kéo lên từ làn nước mát. Thịt cá thơm ngon giàu dinh dưỡng.";
                    case "item.fish-carp": return "Cá chép sông vảy xanh bóng. Nguyên liệu tuyệt vời cho các món cá nướng thảo mộc.";
                    case "item.fish-golden-perch": return "Cá vược hoàng kim quý hiếm. Mang lại giá trị trao đổi rất cao với thương nhân.";
                    case "item.cooked-fish": return "Cá sông nướng mọi thơm lừng tẩm ướp thảo dược rừng. Hồi phục một lượng lớn sinh lực (+18 HP).";
                    case "item.weapon-sword": return "Thanh kiếm dài rèn từ quặng sắt tinh luyện. Vũ khí cận chiến sắc bén gây sát thương cao (+6 DMG).";
                    case "item.weapon-bow": return "Cung săn bắn gỗ sồi dẻo dai. Cho phép bắn tên từ khoảng cách an toàn (+5 DMG).";
                    case "item.ammo-arrow": return "Mũi tên bịt sắt có cánh lông chim. Đạn dược cần thiết để sử dụng cung săn bắn.";
                    case "item.shield-wood": return "Khiên gỗ tròn viền sắt. Giúp giảm thiểu đáng kể sát thương từ các đòn tấn công của quái vật.";
                    case "item.armor-knight": return "Bộ giáp ngực thép của hiệp sĩ hoàng hôn. Tăng cường giới hạn sinh lực tối đa (+10 Max HP).";
                    case "item.meat-raw": return "Tảng thịt tươi săn được từ thú rừng. Có thể chế biến thành các món ăn bổ dưỡng.";
                    case "item.leather": return "Tấm da thú dẻo dai đã qua xử lý. Nguyên liệu may giáp và chế tạo túi đồ.";
                    case "item.hay": return "Bó cỏ khô thơm lừng thu hoạch từ đồng cỏ. Thức ăn khoái khẩu giúp gia súc tăng sản lượng.";
                    case "item.farm-deed": return "Chứng thư khai hoang đất đai có đóng dấu đỏ của Làng Valen. Cho phép mở rộng thêm 12 ô đất nông trại mới.";
                    case "item.flour": return "Bột mì nguyên cám xay nhuyễn từ cối xay gió. Nguyên liệu chính để nướng bánh mì và làm lương khô.";
                    case "item.cheese": return "Khối phô mai vàng béo ngậy ủ từ sữa tươi nguyên chất. Món ngon cao cấp có giá trị thương mại cao.";
                    case "item.cloth": return "Cuộn vải dệt từ sợi len mềm mịn. Nguyên liệu không thể thiếu để may trang phục và lều bạt.";
                    case "item.wine": return "Bình rượu vang hảo hạng lên men từ nho rừng chín mọng. Thức uống quý phái được lái buôn săn đón.";
                    case "item.juice": return "Cốc nước ép quả mọng tươi mát. Giải khát và hồi phục thể lực tức thì (+15 HP).";
                    case "item.iron-bar": return "Thỏi sắt đúc từ lò rèn. Dùng để chế tạo các loại vũ khí, áo giáp và công cụ bền chắc.";
                    case "item.fertilizer": return "Phân bón hữu cơ ủ từ thùng mùn. Bón vào đất giúp cây trồng tăng gấp đôi tốc độ phát triển.";
                    case "item.grape": return "Chùm nho tím mọng nước vừa hái từ giàn leo. Vừa ngọt mát vừa là nguyên liệu ủ rượu tuyệt hảo.";
                    case "item.pumpkin": return "Quả bí ngô to tròn thu hoạch từ luống vườn. Có thể nấu súp bổ dưỡng hoặc bán lấy tiền.";
                    default: return "Vật phẩm được lưu giữ an toàn trong túi hành lý của bạn.";
                }
            }
            else
            {
                switch (itemId)
                {
                    case "item.wood": return "Sturdy oak timber gathered from ancient trees. Used in crafting tools, lighting fires, and constructing shelters.";
                    case "item.stone": return "Solid rock quarried along the old paths. Used as a strong foundation for cabins and stonework.";
                    case "item.cabin-plank": return "Carefully cut and planed wooden planks. Essential component for cabin construction and storage chests.";
                    case "item.tool-axe": return "A worn woodcutter's axe with a sharp edge. Allows faster wood chopping and clearing road blockades.";
                    case "item.tool-pickaxe": return "A sturdy stone pickaxe used for mining rocks and precious iron ore veins underground.";
                    case "item.tool-hoe": return "A sturdy wooden-handled garden hoe. Used to till uncultivated dirt into fertile soil ready for planting seeds.";
                    case "item.iron-ore": return "Pure unrefined iron ore. Essential for forging high-tier metal gear and advanced tools.";
                    case "item.wild-berries": return "Sweet red forest berries. Can be eaten raw (press Q) to restore health or used in cooking.";
                    case "item.medicinal-herb": return "Fragrant wild herbs with soothing properties. Used in brewing remedies and nutritious meals.";
                    case "item.mushroom": return "Earthy wild mushrooms gathered in damp soil. A flavorful ingredient for warm cooking pot stews.";
                    case "item.cooked-meal": return "A steaming hot cooked meal prepared on a campfire. Restores significant health (press Q).";
                    case "item.torch": return "A flaming resin-soaked torch. Wards off darkness and lurking nighttime predators.";
                    case "item.roadwarden-page": return "A weathered journal page from your father, the former Roadwarden, recording regional lore.";
                    case "item.bell-fragment": return "An ancient metallic chime fragment resonating with strange echoes of the old bell towers.";
                    case "item.silver-coin": return "Minted Valen silver currency. Used to trade and buy valuable goods from visiting merchants.";
                    case "item.old-coin": return "A tarnished ancient coin from the prosperity era of The Old Road.";
                    case "item.watering-can": return "An iron watering can. Used to water tilled soil plots to accelerate crop growth.";
                    case "item.seed-wheat": return "Carefully sorted wheat seeds. Plant in tilled soil to grow golden wheat crops.";
                    case "item.seed-corn": return "Fresh corn seeds. Plant in moist soil to grow sweet farm corn.";
                    case "item.seed-carrot": return "Crisp carrot seeds. Cultivate and water regularly to harvest fresh carrots.";
                    case "item.seed-potato": return "Healthy seed potatoes with vigorous eyes. Plant in tilled soil and water regularly to grow rich golden potatoes.";
                    case "item.seed-pineapple": return "Rare sweet pineapple seeds. Cultivate to produce juicy tropical pineapples.";
                    case "item.seed-tomato": return "Ripe tomato seeds. Fast-growing crop yielding juicy red tomatoes.";
                    case "item.wheat": return "Golden wheat sheaf harvested from the field. Staple grain for cooking meals.";
                    case "item.corn": return "Sweet golden corn cob, freshly picked from the farm plots.";
                    case "item.carrot": return "Crisp nutritious carrot, ready to eat or cook into stews.";
                    case "item.potato": return "Golden farm potato harvested fresh from the soil. A hearty staple food rich in energy.";
                    case "item.pineapple": return "Juicy golden pineapple packed with sweetness and nutrients.";
                    case "item.tomato": return "Plump farm-fresh tomato, great for culinary recipes.";
                    case "item.egg": return "Fresh egg gathered from poultry nests in the animal pens.";
                    case "item.wool": return "Soft fleece wool sheared from livestock for crafting and bedding.";
                    case "item.milk": return "Fresh dairy milk jug collected from cows at the Valen barn.";
                    case "item.fence-wood": return "Rustic wooden fence section to enclose farm animals and protect crops.";
                    case "item.gate-wood": return "A latchable wooden gate section for easy access into fenced areas.";
                    case "item.fishing-rod": return "A flexible bamboo fishing rod. Use along the serene riverbanks of Valen to hook fish.";
                    case "item.fishing-bait": return "Juicy earthworms dug up from fertile soil. Irresistible bait for river fish.";
                    case "item.fish-salmon": return "A fresh river salmon caught from clear waters. Highly nutritious when cooked.";
                    case "item.fish-carp": return "A plump river carp with shiny greenish scales. Perfect for grilling with herbs.";
                    case "item.fish-golden-perch": return "A rare and prized golden perch. Highly valued by travelling merchants.";
                    case "item.cooked-fish": return "A steaming grilled fish platter seasoned with forest herbs (+18 HP).";
                    case "item.weapon-sword": return "A forged iron longsword with a keen edge. High melee slashing damage (+6 DMG).";
                    case "item.weapon-bow": return "A sturdy wooden hunter's bow. Enables ranged combat against dangerous beasts (+5 DMG).";
                    case "item.ammo-arrow": return "Feathered iron-tipped arrows. Required ammunition for shooting bows.";
                    case "item.shield-wood": return "A round wooden shield with an iron rim. Blocks incoming melee and projectile damage.";
                    case "item.armor-knight": return "A knight's steel chestplate. Enhances maximum health pool (+10 Max HP).";
                    case "item.meat-raw": return "Fresh meat gathered from hunted wildlife. Can be cooked over fire.";
                    case "item.leather": return "Tough cured beast leather. Essential for crafting armor and fine gear.";
                    case "item.hay": return "A bundle of sweet dried hay. Nutritious feed to boost livestock production.";
                    case "item.farm-deed": return "An official land reclamation deed bearing the Valen seal. Unlocks 12 additional farm plots.";
                    case "item.flour": return "Fine whole grain flour ground at the windmill. Essential for bread and pastries.";
                    case "item.cheese": return "Rich golden cheese aged from pure cow's milk. A delicacy of high trade value.";
                    case "item.cloth": return "A roll of fine woven fabric made from soft sheep wool. Used in tents and clothing.";
                    case "item.wine": return "A vintage bottle of ruby wine brewed in oak kegs. Favored by travelling merchants.";
                    case "item.juice": return "A cup of refreshing chilled berry juice. Quenches thirst and restores health (+15 HP).";
                    case "item.iron-bar": return "A refined iron ingot forged in the blacksmith furnace. Used to craft weapons and armor.";
                    case "item.fertilizer": return "Organic soil fertilizer from composted herbs. Doubles crop growth speed when applied.";
                    case "item.grape": return "Juicy purple grapes picked from the trellis. Delicious fresh or brewed into fine wine.";
                    case "item.pumpkin": return "A massive ripe pumpkin harvested from the garden patch. Great for hearty soups.";
                    default: return "A useful item stored safely in your adventurer's backpack.";
                }
            }
        }

        public static string BuildingName(string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId)) return string.Empty;
            switch (buildingId)
            {
                case "building.cabin": return T("building_cabin");
                case "building.stone-cottage": return T("building_cottage");
                case "building.storage-shed": return T("building_shed");
                case "building.campfire": return T("building_campfire");
                case "building.cooking-hearth": return T("building_hearth");
                case "building.animal-pen-small": return T("building_pen_small");
                case "building.animal-pen-long": return T("building_pen_long");
                default: return buildingId;
            }
        }

        public static string StageName(string stage)
        {
            if (string.IsNullOrEmpty(stage)) return string.Empty;
            switch (stage.ToLowerInvariant())
            {
                case "foundation": return IsVietnamese ? "Nền móng" : "Foundation";
                case "frame": return IsVietnamese ? "Khung nhà" : "Frame";
                case "walls": return IsVietnamese ? "Tường" : "Walls";
                case "roof": return IsVietnamese ? "Mái" : "Roof";
                case "complete": return IsVietnamese ? "Hoàn tất" : "Complete";
                case "ready": return IsVietnamese ? "Sẵn sàng" : "Ready";
                case "ring": return IsVietnamese ? "Vòng đá" : "Ring";
                case "kindling": return IsVietnamese ? "Bùi nhùi" : "Kindling";
                case "flame": return IsVietnamese ? "Ngọn lửa" : "Flame";
                case "base": return IsVietnamese ? "Bệ bếp" : "Base";
                case "chamber": return IsVietnamese ? "Buồng lửa" : "Chamber";
                case "posts": return IsVietnamese ? "Cột rào" : "Posts";
                case "rails": return IsVietnamese ? "Thanh chắn" : "Rails";
                case "gate": return IsVietnamese ? "Cổng vào" : "Gate";
                default: return stage;
            }
        }

        public static string NpcTitle(string jobTitle)
        {
            if (!IsVietnamese || string.IsNullOrEmpty(jobTitle)) return jobTitle;
            switch (jobTitle)
            {
                case "Miller": return "Thợ Xay";
                case "Woodcutter": return "Tiều Phu";
                case "Herbalist": return "Dược Sư";
                case "Blacksmith": return "Thợ Rèn";
                case "Guard": return "Lính Gác";
                case "Stablehand": return "Quản Ngựa";
                case "Worker": return "Người Làm";
                default: return jobTitle;
            }
        }

        public static string NpcDialogue(string jobTitle)
        {
            if (!IsVietnamese)
            {
                switch (jobTitle)
                {
                    case "Miller": return "The old road woke before dawn. Follow it, but keep food in your pack.";
                    case "Woodcutter": return "Trees past the village are fair game. If the bark glows, your axe-hand is close enough.";
                    case "Herbalist": return "Berries, herbs, and mushrooms grow far from Valen. Gather them before night falls.";
                    case "Blacksmith": return "Iron from cave mouths carries old heat. Bring enough and better tools become possible.";
                    case "Guard": return "Travellers whisper about claw marks on the northern ridge. Not wolves. Bigger.";
                    case "Stablehand": return "The animals are harmless. They know which roads still feel safe.";
                    default: return "No one here means you harm. Roads bring trouble, but also trade.";
                }
            }

            switch (jobTitle)
            {
                case "Miller": return "Con Đường Cũ đã thức giấc trước bình minh. Hãy đi theo nó, nhưng nhớ giữ thức ăn trong túi.";
                case "Woodcutter": return "Cây cối bên ngoài làng cứ tự nhiên đốn. Nếu vỏ cây phát sáng, rìu của bạn đã ở đủ gần rồi.";
                case "Herbalist": return "Quả mọng, thảo dược và nấm mọc xa làng Valen. Hãy hái chúng trước khi trời tối.";
                case "Blacksmith": return "Sắt từ cửa hang mang theo hơi ấm cổ xưa. Mang đủ quặng về và chúng ta có thể làm công cụ tốt hơn.";
                case "Guard": return "Khách bộ hành thì thầm về vết cào trên sườn núi phía bắc. Không phải sói đâu. Thứ gì đó to lớn hơn nhiều.";
                case "Stablehand": return "Bọn động vật vô hại lắm. Chúng biết con đường nào vẫn an toàn.";
                default: return "Không ai ở đây có ý xấu với bạn đâu. Những con đường mang lại rắc rối, nhưng cũng mang lại giao thương.";
            }
        }

        public static string EnemyName(string name)
        {
            if (!IsVietnamese || string.IsNullOrEmpty(name)) return name;
            switch (name)
            {
                case "Forest Wolf": return "Sói Rừng";
                case "Bandit Scout": return "Trinh Sát Thổ Phỉ";
                case "Ancient Dragon": return "Cổ Long";
                default: return name;
            }
        }

        private static string English(string key)
        {
            switch (key)
            {
                case "title.subtitle": return "A bell rings beyond Valen. The ancient road has returned, and your father's journal still has missing pages.";
                case "start": return "Start Journey";
                case "settings": return "Settings";
                case "quit": return "Quit";
                case "settings.tip": return "Tip: settings can be reopened from the top-left HUD after starting.";
                case "settings.subtitle": return "Adjust prototype graphics, frame rate, language, and audio output.";
                case "graphics_quality": return "Graphics Quality";
                case "no_quality_levels": return "No Unity quality levels found.";
                case "frame_rate": return "Frame Rate";
                case "language": return "Language";
                case "sound": return "Sound";
                case "master_volume": return "Master Volume";
                case "music_volume": return "Music Volume";
                case "sfx_volume": return "SFX Volume";
                case "on": return "On";
                case "off": return "Off";
                case "current": return "Current";
                case "actual": return "Actual";
                case "target": return "Target";
                case "unlimited": return "Unlimited";
                case "back": return "Back";
                case "back_to_game": return "Back to Game";
                case "roadwarden": return "Roadwarden";
                case "tasks": return "Current Roadwarden Tasks";
                case "landmarks": return "Landmarks";
                case "survey": return "Old road survey";
                case "wood": return "Wood";
                case "stone": return "Stone";
                case "food": return "Food";
                case "ore": return "Ore";
                case "map": return "Map";
                case "bag": return "Bag";
                case "log": return "Log";
                case "gather": return "Gather";
                case "talk": return "Talk";
                case "attack": return "Attack";
                case "eat": return "Eat";
                case "craft": return "Craft";
                case "build": return "Build";
                case "cook": return "Cook";
                case "close": return "Close";
                case "inspect": return "Inspect";
                case "use": return "Use";
                case "enter": return "Enter";
                case "exit": return "Exit";
                case "sleep": return "Sleep";
                case "pack_title": return "Roadwarden Pack";
                case "inventory_close": return "I / Esc to close";
                case "pack_subtitle": return "Materials gathered from Valen Outskirts";
                case "construction_catalog": return "Construction Catalog";
                case "build_close": return "B / Esc to close";
                case "categories": return "Categories";
                case "housing": return "🏡 Housing & Lodges";
                case "housing_desc": return "Homes, tents, cottages & estates";
                case "fire_light": return "🔥 Fire & Lighting";
                case "fire_light_desc": return "Warmth, torches, lanterns & hearths";
                case "animal_pens": return "🐄 Animal Husbandry";
                case "animal_pens_desc": return "Pastures, coops & livestock yards";
                case "fences_security": return "🪵 Fences & Walls";
                case "fences_security_desc": return "Enclose farms & protect homesteads";
                case "paths_decor": return "🛤️ Paths & Landscaping";
                case "paths_decor_desc": return "Paved roads, decking & bridges";
                case "furniture_living": return "🪑 Furniture & Living";
                case "furniture_living_desc": return "Beds, tables, chairs & rugs";
                case "artisan_processing": return "🏭 Artisan & Processing";
                case "artisan_processing_desc": return "Cheese press, loom, kegs & windmill";
                case "storage_logistics": return "📦 Storage & Logistics";
                case "storage_logistics_desc": return "Chests, stone vaults & silos";
                case "gardening_greenery": return "🌿 Gardening & Greenery";
                case "gardening_greenery_desc": return "Trellises, planters & flowerbeds";
                case "water_irrigation": return "⛲ Water & Irrigation";
                case "water_irrigation_desc": return "Wells, aqueducts & stone fountains";
                case "monuments_shrines": return "🗿 Monuments & Shrines";
                case "monuments_shrines_desc": return "Statues, relic altars & bell pillars";
                case "market_commerce": return "🏪 Market & Commerce";
                case "market_commerce_desc": return "Market stalls, signs & caravans";
                case "defenses_traps": return "🏹 Defenses & Traps";
                case "defenses_traps_desc": return "Spike traps, barricades & warning bells";
                case "leisure_camping": return "🎪 Leisure & Camping";
                case "leisure_camping_desc": return "Swings, chess tables & hammocks";
                case "festivals_ornaments": return "🏮 Festivals & Ornaments";
                case "festivals_ornaments_desc": return "Night lanterns, banners & firefly jars";

                case "build_select_hint": return "Select a buildable card to enter placement mode.";
                case "building_cabin": return "Wood Cabin (6x5)";
                case "building_cabin_desc": return "Cozy starter log home with bed, hearth and rest bonus.";
                case "building_cottage": return "Stone Cottage (7x6)";
                case "building_cottage_desc": return "Sturdy stone home with reinforced cellar & forge.";
                case "building_shed": return "Storage Shed (4x4)";
                case "building_shed_desc": return "Utility storage shed for tools and excess materials.";
                case "building_tent": return "Explorer Tent (3x3)";
                case "building_tent_desc": return "Portable canvas tent for quick wilderness resting.";
                case "building_manor": return "Grand Estate Manor (10x8)";
                case "building_manor_desc": return "Expansive 2-story manor: +20% movement & wine cellar.";
                case "building_greenhouse": return "All-Season Greenhouse (8x8)";
                case "building_greenhouse_desc": return "Glasshouse allowing continuous crop growth in winter.";
                case "building_silo": return "Grain Silo (4x4)";
                case "building_silo_desc": return "Tall masonry silo storing bulk animal feed.";
                case "building_herbalist": return "Herbalist Hut (5x4)";
                case "building_herbalist_desc": return "Thatch roof hut boosting potion & medicine potency x2.";
                case "building_tower": return "Lookout Watchtower (4x6)";
                case "building_tower_desc": return "High observation stilt tower revealing monster scouts.";
                case "building_barn": return "Red Dairy Barn (8x6)";
                case "building_barn_desc": return "Spacious barn housing dairy cows and shearable sheep.";

                case "building_campfire": return "Campfire (1x1)";
                case "building_campfire_desc": return "Outdoor fire pit for simple roasting and night warmth.";
                case "building_hearth": return "Cooking Hearth (2x2)";
                case "building_hearth_desc": return "Stone hearth for crafting complex stat-boosting meals.";
                case "building_street_lamp": return "Wrought Street Lamp (1x1)";
                case "building_street_lamp_desc": return "Cast iron street lantern illuminating wide perimeter.";
                case "building_ground_torch": return "Ground Torch (1x1)";
                case "building_ground_torch_desc": return "Simple wooden torch to mark borders and illuminate paths.";
                case "building_lantern_pole": return "Wooden Lantern Post (1x2)";
                case "building_lantern_pole_desc": return "Rustic timber pole hung with glowing oil lantern.";
                case "building_stone_fireplace": return "Stone Fireplace (2x2)";
                case "building_stone_fireplace_desc": return "Indoor stone hearth providing comfort & warmth.";

                case "building_pen_small": return "Small Animal Pen (7x5)";
                case "building_pen_small_desc": return "7x5 fenced yard: shelter, 2 nests, hens, sheep & troughs.";
                case "building_pen_long": return "Grand Avatar Pasture (11x6)";
                case "building_pen_long_desc": return "11x6 compound: Red Barn, cows, sheep, hens, dog & troughs!";
                case "building_sheep_pasture": return "Sheep Pasture (8x6)";
                case "building_sheep_pasture_desc": return "Enclosed grassy pasture dedicated to fluffy wool sheep.";
                case "building_hen_coop": return "Poultry Coop (6x4)";
                case "building_hen_coop_desc": return "Enclosed coop keeping hens safe from nighttime foxes.";
                case "building_feed_trough": return "Feed Trough (2x1)";
                case "building_feed_trough_desc": return "Wooden trough holding hay to boost animal productivity.";
                case "building_water_trough": return "Water Trough (2x1)";
                case "building_water_trough_desc": return "Stone basin providing fresh water for livestock.";

                case "building_fence_drag": return "Custom Drag Fence";
                case "building_fence_drag_desc": return "Click and drag to enclose any custom boundary.";
                case "building_fence_small": return "Small Yard Fence (6x4)";
                case "building_fence_small_desc": return "Complete 4-side timber fence with latching gate.";
                case "building_fence_med": return "Medium Estate Fence (10x8)";
                case "building_fence_med_desc": return "Spacious perimeter fence with gate and lanterns.";
                case "building_fence_lrg": return "Large Farmstead Fence (16x12)";
                case "building_fence_lrg_desc": return "Large farm enclosure for house, barn, and fields.";
                case "building_fence_grd": return "Grand Homestead Fence (24x16)";
                case "building_fence_grd_desc": return "Expansive estate perimeter fence.";
                case "building_fence": return "Timber Fence Post (1x1)";
                case "building_fence_desc": return "Individual wooden fence post to enclose properties.";
                case "building_gate": return "Wood Gate (1x1)";
                case "building_gate_desc": return "Openable wooden gate for quick access.";
                case "building_stone_wall": return "Mortared Stone Wall (1x1)";
                case "building_stone_wall_desc": return "Heavy stone wall resisting monster breaches.";
                case "building_iron_gate": return "Reinforced Iron Gate (2x1)";
                case "building_iron_gate_desc": return "Heavy wrought iron double gate for maximum security.";
                case "building_log_palisade": return "Sharpened Log Palisade (1x1)";
                case "building_log_palisade_desc": return "Defensive wooden barrier repelling wilderness beasts.";

                case "building_path_dirt": return "Dirt Trail (1x1)";
                case "building_path_dirt_desc": return "Compacted earthen trail. +15% move speed.";
                case "building_path_cobble": return "Cobblestone Paving (1x1)";
                case "building_path_cobble_desc": return "Interlocking stone road. +20% move speed.";
                case "building_path_wood": return "Wood Boardwalk (1x1)";
                case "building_path_wood_desc": return "Plank walkway keeping feet dry in muddy rain.";
                case "building_path_stone_tile": return "Granite Tile (1x1)";
                case "building_path_stone_tile_desc": return "Elegant polished plaza tile for courtyard paths.";
                case "building_wood_bridge": return "Timber Bridge (3x2)";
                case "building_wood_bridge_desc": return "Rustic wooden arch bridge crossing irrigation streams.";
                case "building_scarecrow": return "Straw Scarecrow (1x1)";
                case "building_scarecrow_desc": return "Straw scarecrow in coat protecting crops from crows.";

                case "building_straw_bed": return "Straw Mattress Bed (2x2)";
                case "building_straw_bed_desc": return "Soft herbal straw bed to rest and skip night hours.";
                case "building_oak_table": return "Oak Dining Table (2x2)";
                case "building_oak_table_desc": return "Solid oak table for dining and displaying farm wares.";
                case "building_leather_chair": return "Leather Armchair (1x1)";
                case "building_leather_chair_desc": return "Comfortable upholstered chair providing rest bonus.";
                case "building_bookshelf": return "Scholar Bookshelf (2x3)";
                case "building_bookshelf_desc": return "Holds Roadwarden logs and regional lore manuscripts.";
                case "building_woven_rug": return "Woven Wool Rug (3x2)";
                case "building_woven_rug_desc": return "Warm patterned wool rug insulating cold stone floors.";

                case "building_cheese_press": return "Artisan Cheese Press (2x2)";
                case "building_cheese_press_desc": return "Processes raw milk into high-value aged artisan cheese.";
                case "building_loom": return "Spinning Loom (2x2)";
                case "building_loom_desc": return "Weaves sheared raw wool into fine rolls of cloth.";
                case "building_keg": return "Fermentation Keg (2x2)";
                case "building_keg_desc": return "Brews harvested fruits and honey into fine wine & cider.";
                case "building_windmill": return "Stone Windmill (4x4)";
                case "building_windmill_desc": return "Grinds raw wheat grains into fine baking flour.";
                case "building_blacksmith_forge": return "Blacksmith Forge (3x3)";
                case "building_blacksmith_forge_desc": return "Smelts iron ores into refined ingots, weapons and armor.";
                case "building_carpenter_bench": return "Carpenter Workbench (2x2)";
                case "building_carpenter_bench_desc": return "Crafts furniture, barrels and intricate woodwork.";

                case "building_wood_chest": return "Large Wood Chest (2x1)";
                case "building_wood_chest_desc": return "Stores up to 24 item stacks securely at home.";
                case "building_stone_vault": return "Reinforced Stone Vault (2x2)";
                case "building_stone_vault_desc": return "Fireproof security vault for rare coins and artifacts.";
                case "building_compost_bin": return "Organic Compost Bin (2x2)";
                case "building_compost_bin_desc": return "Recycles excess crop leaves into potent fertilizer.";
                case "building_barrel_rack": return "Oak Barrel Rack (3x1)";
                case "building_barrel_rack_desc": return "Aging rack storing barrels of wine and vinegar.";

                case "building_grape_trellis": return "Grapevine Trellis (3x1)";
                case "building_grape_trellis_desc": return "Timber climbing trellis for sweet purple grapes.";
                case "building_pumpkin_patch": return "Giant Pumpkin Trellis (3x3)";
                case "building_pumpkin_patch_desc": return "Dedicated patch for growing giant prize pumpkins.";
                case "building_flower_planter": return "Wildflower Planter (2x1)";
                case "building_flower_planter_desc": return "Decorative planter blooming with fragrance.";
                case "building_garden_hedge": return "Flowering Hedge (1x1)";
                case "building_garden_hedge_desc": return "Lush green natural hedge separating garden zones.";

                case "building_ancient_well": return "Stone Water Well (3x3)";
                case "building_ancient_well_desc": return "Endless source of fresh clean water for crops.";
                case "building_water_aqueduct": return "Irrigation Aqueduct (3x1)";
                case "building_water_aqueduct_desc": return "Elevated timber flume channeling water to fields.";
                case "building_stone_fountain": return "Granite Fountain (4x4)";
                case "building_stone_fountain_desc": return "Majestic water fountain boosting farm aesthetic.";
                case "building_hot_bath": return "Mineral Hot Tub (3x3)";
                case "building_hot_bath_desc": return "Outdoor heated wooden tub rapidly restoring stamina.";

                case "building_knight_statue": return "Guardian Knight Monument (2x3)";
                case "building_knight_statue_desc": return "Carved stone monument honoring the first Roadwarden.";
                case "building_guardian_shrine": return "Ancient Bell Shrine (2x2)";
                case "building_guardian_shrine_desc": return "Consecrated shrine offering blessings and protection.";
                case "building_bell_pillar": return "Resonant Bell Obelisk (2x4)";
                case "building_bell_pillar_desc": return "Towering obelisk warding off wandering night spirits.";

                case "building_market_stall": return "Village Market Stall (3x2)";
                case "building_market_stall_desc": return "Roadside merchant stall to sell goods directly.";
                case "building_farm_sign": return "Carved Estate Sign (1x1)";
                case "building_farm_sign_desc": return "Wooden signboard displaying custom estate name.";
                case "building_travel_cart": return "Merchant Wagon (4x3)";
                case "building_travel_cart_desc": return "Covered wagon for transporting large harvest loads.";

                case "building_spike_trap": return "Concealed Spike Trap (1x1)";
                case "building_spike_trap_desc": return "Camouflaged pit trap damaging hostile night predators.";
                case "building_wooden_barricade": return "Spiked Barricade (2x1)";
                case "building_wooden_barricade_desc": return "Sturdy barrier stopping beast charges.";
                case "building_alarm_bell": return "Watchpost Alarm Bell (1x2)";
                case "building_alarm_bell_desc": return "Rings loudly to alert homestead during beast attacks.";

                case "building_wood_swing": return "Garden Rope Swing (2x2)";
                case "building_wood_swing_desc": return "Relaxing wooden swing suspended under tree shade.";
                case "building_chess_table": return "Stone Chess Table (2x2)";
                case "building_chess_table_desc": return "Outdoor chess table for peaceful afternoon pastimes.";
                case "building_hammock": return "Shaded Canvas Hammock (3x1)";
                case "building_hammock_desc": return "Breezy resting hammock hung between wooden posts.";
                case "building_bbq_grill": return "Stone BBQ Grill Pit (2x2)";
                case "building_bbq_grill_desc": return "Open charcoal grill for roasting large meat feasts.";

                case "building_festival_banner": return "Pennant Banner Pole (3x1)";
                case "building_festival_banner_desc": return "Colorful celebration flags fluttering in breeze.";
                case "building_sky_lantern": return "Floating Sky Lantern (1x1)";
                case "building_sky_lantern_desc": return "Luminous paper lantern glowing warmly in night.";
                case "building_firefly_jar": return "Glowing Firefly Jar (1x1)";
                case "building_firefly_jar_desc": return "Glass jar captured with magical bioluminescent fireflies.";
                case "demolish_btn": return "🔨 Demolish (100% Refund)";
                case "demolish_btn_desc": return "Left-click structure to remove & get items back [X]";
                case "merchant": return "Travelling Merchant";
                case "merchant_shop": return "Eldon's Travelling Wagon";
                case "buy_tab": return "Buy Goods";
                case "sell_tab": return "Sell Goods";
                case "silver_coins": return "Silver Coins";
                case "required_items": return "Required items";
                case "requirements_unfinalized": return "Prototype: requirements not finalized yet.";
                case "select_place": return "Select & Place";
                case "selected_place_hint": return "selected. Move cursor to a valid grid cell, then left click.";
                case "not_enough_items": return "Not enough items";
                case "cannot_build": return "Cannot build";
                case "missing_definition": return "Missing definition";
                case "coming_soon": return "Coming soon";
                case "no_material_cost": return "No material cost.";
                case "missing_building_definition": return "Missing building definition.";
                case "missing": return "Missing";
                case "materials_ready": return "Materials are ready.";
                case "map_title": return "Valen Outskirts Map";
                case "map_close": return "M / Esc to close";
                case "journal": return "Roadwarden Journal";
                case "story_arc": return "Story Arc";
                case "story_no_entries": return "Complete story tasks to unlock Roadwarden notes.";
                case "landmark_records": return "Landmark Records";
                case "esc_close": return "J / Esc to close";
                case "inspect_landmarks": return "Inspect landmarks to fill the journal.";
                case "no_landmarks": return "No landmarks found in this scene.";
                case "unknown_landmark": return "Unknown landmark";
                case "journal_hint": return "Follow the road and inspect this place to record it.";
                case "legend": return "Legend";
                case "legend_player": return "Player";
                case "legend_wood": return "Wood nodes";
                case "legend_stone": return "Stone nodes";
                case "legend_food": return "Food/herb nodes";
                case "legend_ore": return "Ore nodes";
                case "legend_cabin_site": return "Cabin site";
                case "legend_landmark": return "Landmark";
                case "legend_loot": return "Loot chest";
                case "legend_npc": return "Villager NPC";
                case "legend_animal": return "Harmless animal";
                case "legend_hint": return "Explore by following the road. Markers are prototype testing aids.";
                case "home_locator": return "Home Locator";
                case "home": return "Home";
                case "home_site": return "Home site";
                case "no_home_built": return "No home built yet.";
                case "follow_arrow_home": return "Follow the arrow to return.";
                case "waypoint": return "Waypoint";
                case "no_waypoint": return "Open Map and left-click to pin a marker.";
                case "follow_arrow_waypoint": return "Follow the arrow to the pin.";
                case "map_pin_hint": return "Left click/tap map to pin. Right click to clear.";
                case "clear_waypoint": return "Clear Waypoint";
                case "waypoint_set": return "Waypoint pinned.";
                case "waypoint_cleared": return "Waypoint cleared.";
                case "legend_home": return "Home";
                case "legend_building": return "Building";
                case "legend_waypoint": return "Waypoint";
                case "legend_clean_map_hint": return "Map hides resources, loot, NPCs, and animals so navigation stays clean.";
                case "guide": return "Game Guide & Tutorial";
                case "guide_short": return "Guide";
                case "guide_tab_basics": return "1. Basics & Controls";
                case "guide_tab_farming": return "2. Farming & Crops";
                case "guide_tab_fishing": return "3. Fishing & Cooking";
                case "guide_tab_combat": return "4. Combat & Defense";
                case "guide_tab_expansion": return "5. Animals & Expansion";
                case "guide_tab_compendium": return "6. Compendium & Records";
                case "bulletin_board": return "Town Bulletin Board";
                case "mailbox": return "Daily Gift Mailbox";
                case "deliver": return "Deliver";
                case "claim_gift": return "Claim Gift";
                case "claimed": return "Claimed";
                case "day_streak": return "Daily Streak";
                case "compendium_title": return "Rural Compendium & Museum Records";
                case "item.wood": return "Wood";
                case "item.stone": return "Stone";
                case "item.cabin-plank": return "Cabin Plank";
                case "item.tool-axe": return "Worn Axe";
                case "item.tool-pickaxe": return "Stone Pick";
                case "item.tool-hoe": return "Worn Hoe";
                case "item.wild-berries": return "Wild Berries";
                case "item.medicinal-herb": return "Medicinal Herb";
                case "item.mushroom": return "Mushroom";
                case "item.iron-ore": return "Iron Ore";
                case "item.torch": return "Torch";
                case "item.bell-fragment": return "Bell Fragment";
                case "item.old-coin": return "Old Coin";
                case "item.cooked-meal": return "Cooked Meal";
                case "item.egg": return "Egg";
                case "item.wool": return "Wool";
                case "item.milk": return "Milk Bottle";
                case "item.silver-coin": return "Silver Coin";
                case "item.watering-can": return "Watering Can";
                case "item.seed-wheat": return "Wheat Seeds";
                case "item.seed-corn": return "Corn Seeds";
                case "item.seed-carrot": return "Carrot Seeds";
                case "item.seed-potato": return "Potato Seeds";
                case "item.wheat": return "Golden Wheat";
                case "item.corn": return "Sweet Corn";
                case "item.carrot": return "Crisp Carrot";
                case "item.potato": return "Golden Potato";
                case "item.fence-wood": return "Wood Fence";
                case "item.gate-wood": return "Wood Gate";
                case "item.seed-pineapple": return "Pineapple Seeds";
                case "item.seed-tomato": return "Tomato Seeds";
                case "item.pineapple": return "Sweet Pineapple";
                case "item.tomato": return "Ripe Tomato";
                case "item.fishing-rod": return "Bamboo Rod";
                case "item.fishing-bait": return "Earthworm Bait";
                case "item.fish-salmon": return "River Salmon";
                case "item.fish-carp": return "Common Carp";
                case "item.fish-golden-perch": return "Golden Perch";
                case "item.cooked-fish": return "Grilled Fish";
                case "item.weapon-sword": return "Iron Longsword";
                case "item.weapon-bow": return "Hunter's Bow";
                case "item.ammo-arrow": return "Flint Arrow";
                case "item.shield-wood": return "Round Shield";
                case "item.armor-knight": return "Knight Cuirass";
                case "item.meat-raw": return "Raw Meat";
                case "item.leather": return "Beast Leather";
                case "item.hay": return "Dry Hay";
                case "item.farm-deed": return "Farm Land Deed";
                case "item.flour": return "Whole Grain Flour";
                case "item.cheese": return "Artisan Cheese";
                case "item.cloth": return "Fine Cloth";
                case "item.wine": return "Vintage Wine";
                case "item.juice": return "Fresh Berry Juice";
                case "item.iron-bar": return "Iron Ingot";
                case "item.fertilizer": return "Organic Fertilizer";
                case "item.grape": return "Fresh Grapes";
                case "item.pumpkin": return "Giant Pumpkin";
                case "item.roadwarden-page": return "Father's Journal Page";
                case "speech.gather_start": return "Let's see what I can use here.";
                case "speech.gather_done": return "That should help on the road.";
                case "speech.gather_blocked": return "I need the right tool first.";
                case "speech.gather_cancelled": return "Too far. I'll come back to it.";
                case "speech.action_busy": return "One thing at a time.";
                case "speech.craft_done": return "Good. This will be useful.";
                case "speech.craft_blocked": return "I still need more materials.";
                case "speech.cook_done": return "Warm food. That helps.";
                case "speech.cook_blocked": return "I need berries and something to cook with.";
                case "speech.build_begin": return "This place could become something.";
                case "speech.build_started": return "Work has begun.";
                case "speech.build_invalid": return "This ground will not work.";
                case "speech.build_blocked": return "I do not have enough materials.";
                case "speech.loot_start": return "Someone left this behind.";
                case "speech.loot_done": return "Found something.";
                case "speech.landmark_start": return "The old road left a mark here.";
                case "speech.landmark_done": return "I should write this down.";
                case "speech.enter_home": return "At least there is shelter.";
                case "speech.exit_home": return "Back to the road.";
                case "speech.sleep_prompt": return "Eight hours might clear my head.";
                case "speech.sleep_done": return "I feel rested.";
                case "speech.sleep_cancelled": return "Not yet.";
                case "ui.use_bed": return "Use Bed";
                case "ui.sleep_confirm_question": return "Do you want to sleep for 8 in-game hours?";
                case "ui.yes_key": return "Yes (Y)";
                case "ui.no_key": return "No (N)";
                case "ui.player_respawn": return "YOU AWAKEN BY THE EMBERS...";
                case "ui.defeated": return "DEFEATED";
                case "animal.chicken": return "Chicken";
                case "animal.cow": return "Cow";
                case "animal.sheep": return "Sheep";
                case "animal.dog": return "Dog";

                // Chapters
                case "chapter.01.bell.title": return "Chapter I - The Bell Beyond Valen";
                case "chapter.01.bell.summary": return "A bell rings where no bell tower should still stand. Recover your father's trail and prove the old road has returned.";
                case "chapter.02.roadwarden.title": return "Chapter II - Roadwarden's Burden";
                case "chapter.02.roadwarden.summary": return "The road is no longer safe. Prepare tools, shelter, food, and a small fire before following it farther.";
                case "chapter.03.shelter.title": return "Chapter III - Fire Against The Dark";
                case "chapter.03.shelter.summary": return "Build a base near the old road so exploration has a real point of return.";
                case "chapter.04.blackwood.title": return "Chapter IV - Blackwood Omen";
                case "chapter.04.blackwood.summary": return "The road points toward the Blackwood caves and an old dragon scar. This is the next major adventure arc.";

                // Steps
                case "story.01.inspect-road.title": return "Read the first mark on the old road";
                case "story.01.inspect-road.detail": return "Inspect any landmark near Valen to confirm the road is awake.";
                case "story.01.inspect-road.lore": return "The first mark is not weathered by time. Someone, or something, has touched the road recently.";

                case "story.02.open-cache.title": return "Search an abandoned cache";
                case "story.02.open-cache.detail": return "Open an old chest and look for Roadwarden supplies.";
                case "story.02.open-cache.lore": return "The cache still carries Valen resin and Roadwarden twine. It was meant for someone coming back.";

                case "story.03.father-page.title": return "Recover Father's missing page";
                case "story.03.father-page.detail": return "Find the Roadwarden journal page hidden near the starting road.";
                case "story.03.father-page.lore": return "Father's page says: If the roads open again, do not believe what they told us about that night.";

                case "story.04.ask-village.title": return "Ask Valen what they heard";
                case "story.04.ask-village.detail": return "Speak with a villager and collect the first rumour.";
                case "story.04.ask-village.lore": return "The villagers heard the bell before dawn. No one admits being awake, but every hearth was lit.";

                case "story.05.make-axe.title": return "Make a worn axe";
                case "story.05.make-axe.detail": return "Craft an axe so the forest does not decide your path for you.";
                case "story.05.make-axe.lore": return "The axe is crude, but the first Roadwardens started with less.";

                case "story.06.gather-stonewood.title": return "Gather road materials";
                case "story.06.gather-stonewood.detail": return "Carry at least 3 wood and 2 stone.";
                case "story.06.gather-stonewood.lore": return "Wood, stone, and patience: enough to build a place the dark cannot immediately take.";

                case "story.07.make-pick.title": return "Make a stone pick";
                case "story.07.make-pick.detail": return "Craft a pick and prepare to mine old iron.";
                case "story.07.make-pick.lore": return "The pick rings differently near the road, as if the ground remembers iron below it.";

                case "story.08.first-iron.title": return "Mine the first iron";
                case "story.08.first-iron.detail": return "Mine iron ore from an exposed vein.";
                case "story.08.first-iron.lore": return "The iron is warm in your hand. Father wrote that bell towers were built on warmer metal.";

                case "story.09.prepare-food.title": return "Forage travel food";
                case "story.09.prepare-food.detail": return "Gather berries, herbs, or mushrooms before nightfall.";
                case "story.09.prepare-food.lore": return "The road is easier to follow when hunger is not deciding for you.";

                case "story.10.cabin-plank.title": return "Cut the first cabin plank";
                case "story.10.cabin-plank.detail": return "Craft one cabin plank.";
                case "story.10.cabin-plank.lore": return "A single plank is not a home, but it is the first proof you are staying.";

                case "story.11.start-home.title": return "Raise a Roadwarden shelter";
                case "story.11.start-home.detail": return "Start construction on any cabin or cottage.";
                case "story.11.start-home.lore": return "The frame faces the old road like a question waiting for an answer.";

                case "story.12.first-bell-fragment.title": return "Find the first bell fragment";
                case "story.12.first-bell-fragment.detail": return "Recover a bell fragment from the eastern road.";
                case "story.12.first-bell-fragment.lore": return "The fragment does not ring, but it hums when held near Father's page.";

                case "story.13-light-fire.title": return "Build a fire for the night";
                case "story.13-light-fire.detail": return "Complete a campfire or cooking hearth.";
                case "story.13-light-fire.lore": return "The firelight is small. That is why it matters.";

                case "story.14-cook-meal.title": return "Cook a warm meal";
                case "story.14-cook-meal.detail": return "Cook one meal at a completed fire building.";
                case "story.14-cook-meal.lore": return "Warm food turns a camp into a place people can return to.";

                case "story.15-animal-pen.title": return "Make the first village pen";
                case "story.15-animal-pen.detail": return "Build a small or long animal pen.";
                case "story.15-animal-pen.lore": return "The road needs defenders, but Valen first needs ordinary life to continue.";

                case "story.16-find-cave.title": return "Find Blackwood Cave";
                case "story.16-find-cave.detail": return "Discover the Blackwood cave mouth.";
                case "story.16-find-cave.lore": return "The cave breathes cold air. Roadwarden marks warn that the tunnels run below the forest.";

                case "story.17-read-ridge.title": return "Read the dragon-scarred ridge";
                case "story.17-read-ridge.detail": return "Discover the ridge scarred by ancient dragon flame.";
                case "story.17-read-ridge.lore": return "The fused stone proves the old stories were not only stories. Something burned here and survived memory.";

                // Landmarks
                case "landmark.waystone.north.title": return "Northern Waystone";
                case "landmark.waystone.north.journal": return "An ancient guiding monolith along the northern road.";
                case "landmark.sign.road.title": return "Old Road Sign";
                case "landmark.sign.road.journal": return "Weathered wooden signpost pointing toward distant settlements.";
                case "landmark.arch.watch.title": return "Broken Watch Arch";
                case "landmark.arch.watch.journal": return "Ruins of an ancient Roadwarden watch arch overlooking the pass.";
                case "landmark.bridge.river.title": return "River Footbridge";
                case "landmark.bridge.river.journal": return "Sturdy timber bridge crossing the rushing Valen river.";
                case "landmark.camp.abandoned.title": return "Abandoned Camp";
                case "landmark.camp.abandoned.journal": return "Cold ash and weathered tents from an earlier expedition.";
                case "landmark.bell.01.title": return "Eastern Bell Marker";
                case "landmark.bell.01.journal": return "A mossy stone bell tower marking where ancient wardens kept watch.";
                case "landmark.shrine.hunter.title": return "Hunter Shrine";
                case "landmark.shrine.hunter.journal": return "A stone shrine carved with forest runes for safe hunting.";
                case "landmark.dragon.ridge.title": return "Dragon-Scarred Ridge";
                case "landmark.dragon.ridge.journal": return "Vitrified stone cliffs bearing ancient scorching marks.";
                case "landmark.ruin.south.title": return "South Ruin Gate";
                case "landmark.ruin.south.journal": return "A crumbling archway standing at the southern border.";
                case "landmark.cave.blackwood.title": return "Blackwood Cave Mouth";
                case "landmark.cave.blackwood.journal": return "A dark opening leading down into cold subterranean tunnels.";
                case "landmark.village.valen.title": return "Valen Village";
                case "landmark.village.valen.journal": return "The rustic settlement along River Valen with warm cottages and friendly villagers.";
                case "landmark.npc.eldon.title": return "Travelling Merchant Eldon";
                case "landmark.npc.eldon.journal": return "Eldon travels the road selling rare seeds, deeds, equipment, and purchasing local goods.";
                case "landmark.service.bulletin.title": return "Town Bulletin Board";
                case "landmark.service.bulletin.journal": return "Community delivery board offering daily bounty rewards from town residents.";
                case "landmark.service.mailbox.title": return "Daily Gift Mailbox";
                case "landmark.service.mailbox.journal": return "Pigeon post mailbox providing daily login streak gifts and traveler rewards.";
                case "landmark.farm.pasture.title": return "Avatar Animal Pasture";
                case "landmark.farm.pasture.journal": return "Grand animal farm with dairy barn, cows, sheep, hens, troughs, and loyal farm dog.";
                case "landmark.farm.garden.title": return "Farm Crop Garden";
                case "landmark.farm.garden.journal": return "Fertile starter plots growing wheat, corn, carrots, potatoes, tomatoes, and pineapples.";

                default: return key;
            }
        }

        private static string Vietnamese(string key)
        {
            switch (key)
            {
                case "title.subtitle": return "Tiếng chuông vang lên bên ngoài Valen. Con Đường Cũ đã trở lại, và nhật ký của cha bạn vẫn còn thiếu nhiều trang.";
                case "start": return "Bắt đầu hành trình";
                case "settings": return "Cài đặt";
                case "quit": return "Thoát";
                case "settings.tip": return "Gợi ý: có thể mở lại cài đặt ở HUD góc trên trái sau khi vào game.";
                case "settings.subtitle": return "Chỉnh đồ họa, FPS, ngôn ngữ và âm thanh của bản prototype.";
                case "graphics_quality": return "Chất lượng đồ họa";
                case "no_quality_levels": return "Không tìm thấy mức chất lượng Unity.";
                case "frame_rate": return "Tốc độ FPS";
                case "language": return "Ngôn ngữ";
                case "sound": return "Âm thanh";
                case "master_volume": return "Âm lượng tổng";
                case "music_volume": return "Nhạc nền";
                case "sfx_volume": return "Hiệu ứng âm thanh";
                case "on": return "Bật";
                case "off": return "Tắt";
                case "current": return "Hiện tại";
                case "actual": return "Thực tế";
                case "target": return "Mục tiêu";
                case "unlimited": return "Không giới hạn";
                case "back": return "Quay lại";
                case "back_to_game": return "Vào lại game";
                case "roadwarden": return "Roadwarden";
                case "tasks": return "Nhiệm vụ Roadwarden";
                case "landmarks": return "Địa danh";
                case "survey": return "Khảo sát Con Đường Cũ";
                case "wood": return "Gỗ";
                case "stone": return "Đá";
                case "food": return "Thức ăn";
                case "ore": return "Quặng";
                case "map": return "Bản đồ";
                case "bag": return "Túi";
                case "log": return "Nhật ký";
                case "gather": return "Thu thập";
                case "talk": return "Nói chuyện";
                case "attack": return "Tấn công";
                case "eat": return "Ăn";
                case "craft": return "Chế tạo";
                case "build": return "Xây dựng";
                case "cook": return "Nấu ăn";
                case "close": return "Đóng";
                case "inspect": return "Kiểm tra";
                case "use": return "Dùng";
                case "enter": return "Vào";
                case "exit": return "Ra";
                case "sleep": return "Ngủ";
                case "pack_title": return "Túi Roadwarden";
                case "inventory_close": return "I / Esc để đóng";
                case "pack_subtitle": return "Nguyên liệu thu thập từ vùng ngoại ô Valen";
                case "construction_catalog": return "Danh mục xây dựng";
                case "build_close": return "B / Esc để đóng";
                case "categories": return "Thể loại";
                case "housing": return "🏡 Nhà Ở & Kiến Trúc";
                case "housing_desc": return "Nhà gỗ, lều bạt, biệt thự đá & trang viên";
                case "fire_light": return "🔥 Lò Lửa, Sưởi & Chiếu Sáng";
                case "fire_light_desc": return "Giữ ấm, đuốc cắm đất, đèn đường & lò sưởi";
                case "animal_pens": return "🐄 Chuồng Trại & Chăn Nuôi";
                case "animal_pens_desc": return "Khuôn viên chăn thả bò, cừu, chuồng gà & máng ăn";
                case "fences_security": return "🪵 Hàng Rào, Cổng & Tường Rào";
                case "fences_security_desc": return "Rào chắn đất trồng & phòng thủ thú dữ";
                case "paths_decor": return "🛤️ Lối Đi, Đường Lát & Cầu";
                case "paths_decor_desc": return "Đường lát đá, sàn ván gỗ & cầu qua mương";
                case "furniture_living": return "🪑 Nội Thất & Tiện Nghi";
                case "furniture_living_desc": return "Giường nệm rơm, bàn sồi, ghế da & thảm dệt";
                case "artisan_processing": return "🏭 Máy Chế Biến & Xưởng";
                case "artisan_processing_desc": return "Ép phô mai, khung dệt, thùng ủ & cối xay gió";
                case "storage_logistics": return "📦 Kho Chứa & Thùng Hàng";
                case "storage_logistics_desc": return "Rương gỗ lớn, hầm đá bảo mật & thùng ủ phân";
                case "gardening_greenery": return "🌿 Làm Vườn, Giàn Leo & Chậu Cây";
                case "gardening_greenery_desc": return "Giàn nho, giàn bí ngô, bồn hoa & bờ rào bụi";
                case "water_irrigation": return "⛲ Thủy Lợi, Giếng Nước & Bồn Nước";
                case "water_irrigation_desc": return "Giếng đá cổ, máng dẫn nước & đài phun nước";
                case "monuments_shrines": return "🗿 Tượng Đài & Bia Đá Cổ Vật";
                case "monuments_shrines_desc": return "Tượng hiệp sĩ, bàn thờ thần hộ mệnh & trụ chuông";
                case "market_commerce": return "🏪 Thương Mại, Quầy Hàng & Biển";
                case "market_commerce_desc": return "Quầy hàng chợ phiên, biển gỗ nông trại & xe du mục";
                case "defenses_traps": return "🏹 Phòng Thủ, Bẫy Thú & Tháp Canh";
                case "defenses_traps_desc": return "Bẫy chông ngầm, rào cọc nhọn & chuông báo động";
                case "leisure_camping": return "🎪 Thư Giãn, Cắm Trại & Nghỉ Ngơi";
                case "leisure_camping_desc": return "Xích đu gỗ, bàn cờ đá, võng dù & bếp nướng BBQ";
                case "festivals_ornaments": return "🏮 Lễ Hội & Đèn Trang Trí Đêm";
                case "festivals_ornaments_desc": return "Lồng đèn hoa đăng, cờ ngũ sắc & hũ đom đóm";

                case "build_select_hint": return "Chọn thẻ công trình để vào chế độ đặt.";
                case "building_cabin": return "Nhà gỗ Cabin (6x5)";
                case "building_cabin_desc": return "Nhà gỗ ấm cúng có giường ngủ, bếp sưởi và hồi phục thể lực.";
                case "building_cottage": return "Nhà đá Cottage (7x6)";
                case "building_cottage_desc": return "Biệt thự đá kiên cố có hầm ngầm bảo quản và xưởng rèn riêng.";
                case "building_shed": return "Nhà kho Shed (4x4)";
                case "building_shed_desc": return "Nhà kho lưu trữ dụng cụ và nông sản dư thừa.";
                case "building_tent": return "Lều bạt dã ngoại (3x3)";
                case "building_tent_desc": return "Lều bạt cơ động cắm ngoài trời giúp nghỉ ngơi nhanh.";
                case "building_manor": return "Đại trang viên 2 tầng (10x8)";
                case "building_manor_desc": return "Dinh thự sang trọng: Tăng 20% tốc độ chạy & có hầm ủ rượu.";
                case "building_greenhouse": return "Nhà kính bốn mùa (8x8)";
                case "building_greenhouse_desc": return "Nhà kính giữ nhiệt cho phép cây trồng lớn cả vào mùa đông.";
                case "building_silo": return "Tháp Silo chứa hạt (4x4)";
                case "building_silo_desc": return "Tháp trụ đá cao trữ lượng lớn cỏ khô và thức ăn gia súc.";
                case "building_herbalist": return "Nhà thảo dược học (5x4)";
                case "building_herbalist_desc": return "Nhà lợp rêu phơi dược liệu: Tăng gấp đôi hiệu lực thuốc hồi phục.";
                case "building_tower": return "Chòi canh thám hiểm (4x6)";
                case "building_tower_desc": return "Tháp gỗ cao thắp đuốc xua đuổi quái vật đêm từ xa.";
                case "building_barn": return "Nhà chuồng nông trại (8x6)";
                case "building_barn_desc": return "Chuồng mái đỏ rộng rãi nuôi bò sữa và cừu lông.";

                case "building_campfire": return "Bếp lửa dã ngoại (1x1)";
                case "building_campfire_desc": return "Nguồn sáng ngoài trời kiêm điểm nướng thịt cá đơn giản.";
                case "building_hearth": return "Bếp nấu gia đình (2x2)";
                case "building_hearth_desc": return "Trạm nấu nướng kiên cố chế biến món ăn tăng chỉ số.";
                case "building_street_lamp": return "Cột đèn đường sắt (1x1)";
                case "building_street_lamp_desc": return "Đèn đường sắt rèn chiếu sáng rực rỡ một vùng rộng lớn.";
                case "building_ground_torch": return "Đuốc cắm đất (1x1)";
                case "building_ground_torch_desc": return "Đuốc gỗ giản dị cắm dọc ranh giới để định hướng ban đêm.";
                case "building_lantern_pole": return "Cột đèn lồng gỗ (1x2)";
                case "building_lantern_pole_desc": return "Trụ gỗ mộc mạc treo lồng đèn dầu ấm áp.";
                case "building_stone_fireplace": return "Lò sưởi đá phòng khách (2x2)";
                case "building_stone_fireplace_desc": return "Lò sưởi bằng đá trong nhà xua tan giá lạnh mùa đông.";

                case "building_pen_small": return "Chuồng nuôi nhỏ (7x5)";
                case "building_pen_small_desc": return "Khuôn viên rào 7x5 gồm chuồng gà, 2 ổ rơm, đàn gà, cừu lông & máng ăn.";
                case "building_pen_long": return "Vườn chăn nuôi Avatar (11x6)";
                case "building_pen_long_desc": return "Khuôn viên 11x6: Chuồng bò đỏ, 2 bò sữa, 2 cừu lông, gà, chó & máng ăn!";
                case "building_sheep_pasture": return "Đồng cỏ nuôi cừu (8x6)";
                case "building_sheep_pasture_desc": return "Khuôn viên cỏ xanh rào kín dành riêng cho đàn cừu lấy len.";
                case "building_hen_coop": return "Chuồng nuôi gia cầm (6x4)";
                case "building_hen_coop_desc": return "Chuồng gà ấm áp an toàn tránh thú săn đêm.";
                case "building_feed_trough": return "Máng cỏ ăn (2x1)";
                case "building_feed_trough_desc": return "Máng gỗ đựng rơm cỏ giúp vật nuôi tăng năng suất.";
                case "building_water_trough": return "Máng nước uống (2x1)";
                case "building_water_trough_desc": return "Bồn đá chứa nước sạch cho vật nuôi giải khát.";

                case "building_fence_drag": return "Rào Kéo Thả Tự Do";
                case "building_fence_drag_desc": return "Nhấn giữ kéo chuột để rào quanh bất kỳ khu đất nào.";
                case "building_fence_small": return "Rào Sân Nhỏ (6x4)";
                case "building_fence_small_desc": return "Hàng rào bao quanh 4 phía có cổng gỗ mở/đóng.";
                case "building_fence_med": return "Rào Khuôn Viên Vừa (10x8)";
                case "building_fence_med_desc": return "Khuôn viên rào rộng có cổng, đèn lồng và biển tên.";
                case "building_fence_lrg": return "Rào Trang Trại Rộng (16x12)";
                case "building_fence_lrg_desc": return "Khuôn viên trang trại lớn đủ chỗ làm nhà, chuồng, vườn.";
                case "building_fence_grd": return "Rào Đại Điền Trang (24x16)";
                case "building_fence_grd_desc": return "Khuôn viên điền trang siêu rộng bao quanh toàn bộ khu đất.";
                case "building_fence": return "Cọc Rào Gỗ Đơn (1x1)";
                case "building_fence_desc": return "Cọc rào gỗ bao quanh bảo vệ nhà & đất trồng.";
                case "building_gate": return "Cổng Rào Gỗ (1x1)";
                case "building_gate_desc": return "Cổng rào gỗ mở/đóng ra vào tự do.";
                case "building_stone_wall": return "Tường Đá Kiên Cố (1x1)";
                case "building_stone_wall_desc": return "Tường xây từ đá tảng chống chọi quái vật hung hãn.";
                case "building_iron_gate": return "Cổng Sắt Rèn Đôi (2x1)";
                case "building_iron_gate_desc": return "Cổng sắt đúc nặng bảo vệ kiên cố cho trang viên.";
                case "building_log_palisade": return "Hàng Rào Cọc Gỗ Vót Nhọn (1x1)";
                case "building_log_palisade_desc": return "Cọc gỗ vót nhọn dựng đứng đẩy lùi thú rừng xâm nhập.";

                case "building_path_dirt": return "Lối Mòn Đất Nện (1x1)";
                case "building_path_dirt_desc": return "Đường đất nện nông thôn. Tăng 15% tốc độ chạy.";
                case "building_path_cobble": return "Đường Lát Đá Sỏi (1x1)";
                case "building_path_cobble_desc": return "Đường lát đá sỏi kiên cố. Tăng 20% tốc độ chạy.";
                case "building_path_wood": return "Sàn Lát Ván Gỗ (1x1)";
                case "building_path_wood_desc": return "Sàn ván gỗ sạch sẽ giúp không bị lấm bùn khi mưa.";
                case "building_path_stone_tile": return "Gạch Đá Hoa Cương (1x1)";
                case "building_path_stone_tile_desc": return "Đá hoa cương mài bóng sang trọng cho sân vườn.";
                case "building_wood_bridge": return "Cầu Gỗ Qua Mương (3x2)";
                case "building_wood_bridge_desc": return "Cầu gỗ uốn cong mộc mạc bắc qua các rãnh thủy lợi.";
                case "building_scarecrow": return "Bù nhìn rơm (1x1)";
                case "building_scarecrow_desc": return "Bù nhìn rơm mặc áo xanh giữ vườn bảo vệ mùa màng.";

                case "building_straw_bed": return "Giường Nệm Rơm Êm (2x2)";
                case "building_straw_bed_desc": return "Giường rơm êm ái thơm hương thảo mộc để ngủ qua đêm.";
                case "building_oak_table": return "Bàn Ăn Gỗ Sồi (2x2)";
                case "building_oak_table_desc": return "Bàn ăn gỗ sồi dày dặn bày biện nông sản và thức ăn.";
                case "building_leather_chair": return "Ghế Bành Bọc Da (1x1)";
                case "building_leather_chair_desc": return "Ghế ngồi thư giãn êm ái hồi phục năng lượng.";
                case "building_bookshelf": return "Tủ Sách Học Giả (2x3)";
                case "building_bookshelf_desc": return "Lưu giữ các trang nhật ký Roadwarden và bản đồ cổ.";
                case "building_woven_rug": return "Thảm Len Dệt Tay (3x2)";
                case "building_woven_rug_desc": return "Thảm len hoa văn ấm áp trải sàn nhà.";

                case "building_cheese_press": return "Thùng Ép Phô Mai (2x2)";
                case "building_cheese_press_desc": return "Chế biến sữa bò tươi thành phô mai ủ giá trị cao.";
                case "building_loom": return "Khung Dệt Len (2x2)";
                case "building_loom_desc": return "Dệt lông cừu thô thành những súc vải mềm mịn.";
                case "building_keg": return "Thùng Gỗ Lên Men Rượu (2x2)";
                case "building_keg_desc": return "Ủ trái cây và mật ngọt thành rượu hoa quả hảo hạng.";
                case "building_windmill": return "Cối Xay Gió Bột Mì (4x4)";
                case "building_windmill_desc": return "Xay hạt lúa mì thành bột mì mịn làm bánh.";
                case "building_blacksmith_forge": return "Lò Rèn Kim Khí (3x3)";
                case "building_blacksmith_forge_desc": return "Nung chảy quặng sắt rèn vũ khí, giáp trụ và công cụ.";
                case "building_carpenter_bench": return "Bàn Thợ Mộc (2x2)";
                case "building_carpenter_bench_desc": return "Xưởng mộc gia công đồ gỗ nội thất tinh xảo.";

                case "building_wood_chest": return "Rương Gỗ Lớn (2x1)";
                case "building_wood_chest_desc": return "Cất giữ an toàn 24 ngăn đồ đạc tại nông trại.";
                case "building_stone_vault": return "Hầm Đá Gia Cố (2x2)";
                case "building_stone_vault_desc": return "Hầm đá chống cháy bảo quản tiền bạc và cổ vật quý.";
                case "building_compost_bin": return "Thùng Ủ Phân Hữu Cơ (2x2)";
                case "building_compost_bin_desc": return "Tái chế phụ phẩm nông nghiệp thành phân bón tốt.";
                case "building_barrel_rack": return "Kệ Thùng Gỗ Ủ (3x1)";
                case "building_barrel_rack_desc": return "Kệ cất giữ các thùng rượu và giấm lâu năm.";

                case "building_grape_trellis": return "Giàn Nho Leo Gỗ (3x1)";
                case "building_grape_trellis_desc": return "Giàn leo gỗ sồi nâng đỡ các chùm nho ngọt.";
                case "building_pumpkin_patch": return "Giàn Trồng Bí Ngô (3x3)";
                case "building_pumpkin_patch_desc": return "Khu vực chuyên biệt nuôi dưỡng những quả bí khổng lồ.";
                case "building_flower_planter": return "Bồn Hoa Thơm (2x1)";
                case "building_flower_planter_desc": return "Bồn hoa dại nở rộ tỏa ngát hương thơm.";
                case "building_garden_hedge": return "Bờ Rào Cây Xanh (1x1)";
                case "building_garden_hedge_desc": return "Hàng rào cây bụi xanh mướt phân chia khu vườn.";

                case "building_ancient_well": return "Giếng Nước Đá Cổ (3x3)";
                case "building_ancient_well_desc": return "Nguồn nước ngọt ngầm vô tận tưới mát mọi mùa màng.";
                case "building_water_aqueduct": return "Máng Dẫn Nước Gỗ (3x1)";
                case "building_water_aqueduct_desc": return "Máng gỗ nâng cao dẫn nước ngọt tưới khắp các luống đất.";
                case "building_stone_fountain": return "Đài Phun Nước Đá (4x4)";
                case "building_stone_fountain_desc": return "Đài phun nước mát rượi tô điểm vẻ đẹp điền trang.";
                case "building_hot_bath": return "Bồn Tắm Nước Khoáng (3x3)";
                case "building_hot_bath_desc": return "Bồn tắm nước nóng gỗ sồi xua tan mỏi mệt tức thì.";

                case "building_knight_statue": return "Tượng Đài Hiệp Sĩ (2x3)";
                case "building_knight_statue_desc": return "Tượng đá tạc hình người Roadwarden đầu tiên hộ quốc.";
                case "building_guardian_shrine": return "Bia Thờ Thần Hộ Mệnh (2x2)";
                case "building_guardian_shrine_desc": return "Bàn thờ linh thiêng ban phước lành và bình an.";
                case "building_bell_pillar": return "Cột Chuông Linh Hồn (2x4)";
                case "building_bell_pillar_desc": return "Cột đá chuông cổ ngân vang xua đuổi tà khí bóng đêm.";

                case "building_market_stall": return "Quầy Hàng Chợ Phiên (3x2)";
                case "building_market_stall_desc": return "Quầy bán hàng trực tiếp đón khách lữ hành ghé thăm.";
                case "building_farm_sign": return "Biển Gỗ Điền Trang (1x1)";
                case "building_farm_sign_desc": return "Biển gỗ khắc tên nông trại trang nghiêm.";
                case "building_travel_cart": return "Xe Ngựa Du Mục (4x3)";
                case "building_travel_cart_desc": return "Xe hàng bạt lữ hành chuyên chở mùa màng bội thu.";

                case "building_spike_trap": return "Bẫy Chông Ngầm (1x1)";
                case "building_spike_trap_desc": return "Bẫy hố chông nhọn ngụy trang sát thương thú dữ.";
                case "building_wooden_barricade": return "Rào Cọc Nhọn Chắn Địch (2x1)";
                case "building_wooden_barricade_desc": return "Rào cọc gỗ cứng cáp ngăn chặn quái vật lao vào.";
                case "building_alarm_bell": return "Chuông Báo Động (1x2)";
                case "building_alarm_bell_desc": return "Gõ chuông rung chuyển cảnh báo toàn bộ nông trang.";

                case "building_wood_swing": return "Ghế Xích Đu Gỗ (2x2)";
                case "building_wood_swing_desc": return "Xích đu gỗ đung đưa dưới bóng cây mát rượi.";
                case "building_chess_table": return "Bàn Cờ Đá Ngoài Trời (2x2)";
                case "building_chess_table_desc": return "Bàn cờ thảnh thơi giải trí sau những giờ đồng áng.";
                case "building_hammock": return "Võng Dù Mát Rượi (3x1)";
                case "building_hammock_desc": return "Võng dù êm ái đón gió hiu hiu ngày nắng đẹp.";
                case "building_bbq_grill": return "Bếp Nướng BBQ Đá (2x2)";
                case "building_bbq_grill_desc": return "Lò than nướng thịt ngoài trời thơm lừng khắp xóm.";

                case "building_festival_banner": return "Cột Cờ Lễ Hội Ngũ Sắc (3x1)";
                case "building_festival_banner_desc": return "Dải cờ hoa rực rỡ tung bay mừng ngày hội mùa.";
                case "building_sky_lantern": return "Lồng Đèn Hoa Đăng (1x1)";
                case "building_sky_lantern_desc": return "Đèn hoa đăng phát sáng lung linh giữa bầu trời đêm.";
                case "building_firefly_jar": return "Hũ Đom Đóm Ma Thuật (1x1)";
                case "building_firefly_jar_desc": return "Bình thủy tinh chứa đàn đom đóm phát quang huyền ảo.";
                case "demolish_btn": return "🔨 Xóa Công Trình (Hoàn 100%)";
                case "demolish_btn_desc": return "Click chuột trái vào công trình để phá & thu hồi [X]";
                case "merchant": return "Thương nhân lang thang";
                case "merchant_shop": return "Xe hàng thương nhân Eldon";
                case "buy_tab": return "Mua hàng";
                case "sell_tab": return "Bán hàng";
                case "silver_coins": return "Đồng bạc";
                case "required_items": return "Vật phẩm cần";
                case "requirements_unfinalized": return "Prototype: yêu cầu chưa chốt.";
                case "select_place": return "Chọn & đặt";
                case "selected_place_hint": return "đã chọn. Di chuyển trỏ đến ô hợp lệ, rồi click trái.";
                case "not_enough_items": return "Không đủ vật phẩm";
                case "cannot_build": return "Không thể xây";
                case "missing_definition": return "Thiếu định nghĩa";
                case "coming_soon": return "Sắp có";
                case "no_material_cost": return "Không tốn nguyên liệu.";
                case "missing_building_definition": return "Thiếu định nghĩa công trình.";
                case "missing": return "Thiếu";
                case "materials_ready": return "Nguyên liệu đã sẵn sàng.";
                case "map_title": return "Bản đồ ngoại ô Valen";
                case "map_close": return "M / Esc để đóng";
                case "journal": return "Nhật ký Roadwarden";
                case "story_arc": return "Cốt truyện";
                case "story_no_entries": return "Hoàn thành nhiệm vụ truyện để mở ghi chú Roadwarden.";
                case "landmark_records": return "Ghi chép địa danh";
                case "esc_close": return "J / Esc để đóng";
                case "inspect_landmarks": return "Kiểm tra địa danh để ghi vào nhật ký.";
                case "no_landmarks": return "Chưa có địa danh nào trong scene.";
                case "unknown_landmark": return "Địa danh chưa biết";
                case "journal_hint": return "Đi theo con đường và kiểm tra nơi này để ghi vào nhật ký.";
                case "legend": return "Chú giải";
                case "legend_player": return "Nhân vật";
                case "legend_wood": return "Cây gỗ";
                case "legend_stone": return "Mỏ đá";
                case "legend_food": return "Thực phẩm/thảo dược";
                case "legend_ore": return "Mỏ quặng";
                case "legend_cabin_site": return "Nền nhà";
                case "legend_landmark": return "Địa danh";
                case "legend_loot": return "Rương tiếp tế";
                case "legend_npc": return "Dân làng NPC";
                case "legend_animal": return "Động vật vô hại";
                case "legend_hint": return "Khám phá bằng cách men theo đường. Marker là hỗ trợ test prototype.";
                case "home_locator": return "Định vị nhà";
                case "home": return "Nhà";
                case "home_site": return "Nền nhà";
                case "no_home_built": return "Chưa xây nhà.";
                case "follow_arrow_home": return "Đi theo mũi tên để về.";
                case "waypoint": return "Mốc ghim";
                case "no_waypoint": return "Mở Bản đồ và click trái để ghim mốc.";
                case "follow_arrow_waypoint": return "Đi theo mũi tên tới mốc.";
                case "map_pin_hint": return "Click/chạm bản đồ để ghim. Click phải để xóa.";
                case "clear_waypoint": return "Xóa mốc";
                case "waypoint_set": return "Đã ghim mốc.";
                case "waypoint_cleared": return "Đã xóa mốc.";
                case "legend_home": return "Nhà";
                case "legend_building": return "Công trình";
                case "legend_waypoint": return "Mốc ghim";
                case "legend_clean_map_hint": return "Bản đồ ẩn tài nguyên, rương, NPC và động vật để dễ định hướng.";
                case "guide": return "Hướng Dẫn Chơi & Tân Thủ";
                case "guide_short": return "Hướng Dẫn";
                case "guide_tab_basics": return "1. Điều Khiển & Cơ Bản";
                case "guide_tab_farming": return "2. Nông Trại & Trồng Trọt";
                case "guide_tab_fishing": return "3. Câu Cá & Nấu Ăn";
                case "guide_tab_combat": return "4. Chiến Đấu & Phòng Thủ";
                case "guide_tab_expansion": return "5. Chăn Nuôi & Đất Đai";
                case "guide_tab_compendium": return "6. Sổ Bách Khoa Bộ Sưu Tập";
                case "bulletin_board": return "Bảng Đơn Hàng Thị Trấn";
                case "mailbox": return "Hòm Thư Quà Tặng Mỗi Ngày";
                case "deliver": return "Giao Hàng";
                case "claim_gift": return "Nhận Quà Hôm Nay";
                case "claimed": return "Đã Nhận";
                case "day_streak": return "Chuỗi Ngày";
                case "compendium_title": return "Sổ Bách Khoa & Kỷ Lục Đồng Quê";
                case "item.wood": return "Gỗ";
                case "item.stone": return "Đá";
                case "item.cabin-plank": return "Ván gỗ";
                case "item.tool-axe": return "Rìu cũ";
                case "item.tool-pickaxe": return "Cuốc đá";
                case "item.tool-hoe": return "Cuốc làm vườn";
                case "item.wild-berries": return "Quả dại";
                case "item.medicinal-herb": return "Thảo dược";
                case "item.mushroom": return "Nấm";
                case "item.iron-ore": return "Quặng sắt";
                case "item.torch": return "Đuốc";
                case "item.bell-fragment": return "Mảnh chuông";
                case "item.old-coin": return "Xu cổ";
                case "item.cooked-meal": return "Bữa ăn nóng";
                case "item.egg": return "Trứng";
                case "item.wool": return "Len";
                case "item.milk": return "Sữa tươi";
                case "item.silver-coin": return "Đồng bạc";
                case "item.watering-can": return "Bình tưới nước";
                case "item.seed-wheat": return "Hạt lúa mì";
                case "item.seed-corn": return "Hạt bắp";
                case "item.seed-carrot": return "Hạt cà rốt";
                case "item.seed-potato": return "Hạt khoai tây";
                case "item.wheat": return "Lúa mì vàng";
                case "item.corn": return "Bắp ngọt";
                case "item.carrot": return "Cà rốt tươi";
                case "item.potato": return "Khoai tây vàng";
                case "item.fence-wood": return "Hàng rào gỗ";
                case "item.gate-wood": return "Cổng rào gỗ";
                case "item.seed-pineapple": return "Hạt giống dứa";
                case "item.seed-tomato": return "Hạt giống cà chua";
                case "item.pineapple": return "Dứa vàng ngọt";
                case "item.tomato": return "Cà chua tươi";
                case "item.fishing-rod": return "Cần câu tre";
                case "item.fishing-bait": return "Mồi trùn đất";
                case "item.fish-salmon": return "Cá hồi sông";
                case "item.fish-carp": return "Cá chép sông";
                case "item.fish-golden-perch": return "Cá vược hoàng kim";
                case "item.cooked-fish": return "Cá nướng thảo mộc";
                case "item.weapon-sword": return "Kiếm sắt dài";
                case "item.weapon-bow": return "Cung săn bắn";
                case "item.ammo-arrow": return "Mũi tên bịt sắt";
                case "item.shield-wood": return "Khiên gỗ tròn";
                case "item.armor-knight": return "Giáp ngực hiệp sĩ";
                case "item.meat-raw": return "Thịt thú tươi";
                case "item.leather": return "Da thú thuộc";
                case "item.hay": return "Cỏ khô cho gia súc";
                case "item.farm-deed": return "Thư khai hoang đất";
                case "item.flour": return "Bột mì nguyên cám";
                case "item.cheese": return "Phô mai vàng";
                case "item.cloth": return "Cuộn vải dệt";
                case "item.wine": return "Rượu vang đỏ";
                case "item.juice": return "Nước ép quả mọng";
                case "item.iron-bar": return "Thỏi sắt đúc";
                case "item.fertilizer": return "Phân bón hữu cơ";
                case "item.grape": return "Chùm nho tươi";
                case "item.pumpkin": return "Quả bí ngô";
                case "item.roadwarden-page": return "Trang nhật ký của cha";
                case "speech.gather_start": return "Để xem có dùng được gì ở đây.";
                case "speech.gather_done": return "Thứ này sẽ có ích trên đường.";
                case "speech.gather_blocked": return "Mình cần đúng công cụ trước.";
                case "speech.gather_cancelled": return "Xa quá. Lát nữa quay lại.";
                case "speech.action_busy": return "Làm từng việc một thôi.";
                case "speech.craft_done": return "Tốt. Cái này sẽ hữu dụng.";
                case "speech.craft_blocked": return "Mình vẫn cần thêm nguyên liệu.";
                case "speech.cook_done": return "Đồ ăn nóng. Dễ chịu hơn rồi.";
                case "speech.cook_blocked": return "Mình cần quả dại và nấm/thảo dược để nấu.";
                case "speech.build_begin": return "Chỗ này có thể dựng được thứ gì đó.";
                case "speech.build_started": return "Công việc bắt đầu rồi.";
                case "speech.build_invalid": return "Nền đất này không ổn.";
                case "speech.build_blocked": return "Mình không đủ nguyên liệu.";
                case "speech.loot_start": return "Có ai đó đã để lại thứ này.";
                case "speech.loot_done": return "Tìm được thứ gì đó rồi.";
                case "speech.landmark_start": return "Con Đường Cũ để lại dấu vết ở đây.";
                case "speech.landmark_done": return "Mình nên ghi lại điều này.";
                case "speech.enter_home": return "Ít nhất cũng có chỗ trú ẩn.";
                case "speech.exit_home": return "Trở lại con đường.";
                case "speech.sleep_prompt": return "Ngủ tám tiếng có thể giúp đầu óc tỉnh táo hơn.";
                case "speech.sleep_done": return "Mình thấy khỏe hơn rồi.";
                case "speech.sleep_cancelled": return "Chưa phải lúc.";
                case "ui.use_bed": return "Sử Dụng Giường";
                case "ui.sleep_confirm_question": return "Bạn có muốn ngủ 8 tiếng trong game không?";
                case "ui.yes_key": return "Có (Y)";
                case "ui.no_key": return "Không (N)";
                case "ui.player_respawn": return "BẠN TỈNH LẠI BÊN ĐỐNG TRO TÀN...";
                case "ui.defeated": return "ĐÃ HẠ GỤC";
                case "animal.chicken": return "Gà";
                case "animal.cow": return "Bò";
                case "animal.sheep": return "Cừu";
                case "animal.dog": return "Chó";

                // Chapters
                case "chapter.01.bell.title": return "Chương I - Tiếng Chuông Bên Ngoài Valen";
                case "chapter.01.bell.summary": return "Tiếng chuông vang lên nơi không còn tháp chuông nào. Hãy lần theo dấu vết của cha bạn và chứng minh Con Đường Cũ đã trở lại.";
                case "chapter.02.roadwarden.title": return "Chương II - Trách Nhiệm Roadwarden";
                case "chapter.02.roadwarden.summary": return "Con đường không còn an toàn nữa. Hãy chuẩn bị công cụ, nơi trú ẩn, thức ăn và một ngọn lửa nhỏ trước khi dấn thân xa hơn.";
                case "chapter.03.shelter.title": return "Chương III - Ngọn Lửa Chống Lại Bóng Tối";
                case "chapter.03.shelter.summary": return "Xây dựng căn cứ gần Con Đường Cũ để hành trình khám phá luôn có một điểm trở về thực sự.";
                case "chapter.04.blackwood.title": return "Chương IV - Điềm Báo Blackwood";
                case "chapter.04.blackwood.summary": return "Con đường dẫn về phía các hang động Blackwood và vết sẹo rồng cổ xưa. Đây là chặng phiêu lưu lớn tiếp theo.";

                // Steps
                case "story.01.inspect-road.title": return "Đọc dấu tích đầu tiên trên Con Đường Cũ";
                case "story.01.inspect-road.detail": return "Khảo sát một địa danh gần Valen để xác nhận con đường đã thức tỉnh.";
                case "story.01.inspect-road.lore": return "Dấu tích đầu tiên không hề bị mài mòn theo năm tháng. Ai đó, hoặc thứ gì đó, đã chạm vào con đường gần đây.";

                case "story.02.open-cache.title": return "Tìm kiếm hòm đồ bỏ hoang";
                case "story.02.open-cache.detail": return "Mở một chiếc rương cũ và tìm kiếm đồ tiếp tế của Roadwarden.";
                case "story.02.open-cache.lore": return "Hòm đồ vẫn còn mùi nhựa cây Valen và dây buộc Roadwarden. Nó được chuẩn bị cho một người sẽ trở lại.";

                case "story.03.father-page.title": return "Tìm lại trang nhật ký của cha";
                case "story.03.father-page.detail": return "Tìm trang nhật ký Roadwarden được giấu gần con đường khởi đầu.";
                case "story.03.father-page.lore": return "Trang nhật ký của cha viết: Nếu những con đường mở ra lần nữa, đừng tin những gì họ đã kể về đêm hôm đó.";

                case "story.04.ask-village.title": return "Hỏi dân làng Valen về điều họ đã nghe";
                case "story.04.ask-village.detail": return "Nói chuyện với một dân làng và thu thập lời đồn đầu tiên.";
                case "story.04.ask-village.lore": return "Dân làng đã nghe thấy tiếng chuông trước lúc bình minh. Không ai nhận mình thức giấc, nhưng mọi bếp lửa đều đã được thắp sáng.";

                case "story.05.make-axe.title": return "Tạo chiếc rìu cũ";
                case "story.05.make-axe.detail": return "Chế tạo một chiếc rìu để khu rừng không quyết định con đường thay bạn.";
                case "story.05.make-axe.lore": return "Chiếc rìu tuy thô sơ, nhưng những Roadwarden đầu tiên đã bắt đầu với ít hơn thế.";

                case "story.06.gather-stonewood.title": return "Thu thập vật liệu đường xá";
                case "story.06.gather-stonewood.detail": return "Mang theo ít nhất 3 gỗ và 2 đá.";
                case "story.06.gather-stonewood.lore": return "Gỗ, đá và sự kiên nhẫn: đủ để dựng nên một nơi mà bóng tối không thể ngay lập tức nuốt chửng.";

                case "story.07.make-pick.title": return "Tạo chiếc cuốc đá";
                case "story.07.make-pick.detail": return "Chế tạo cuốc đá và chuẩn bị khai thác quặng sắt cổ.";
                case "story.07.make-pick.lore": return "Chiếc cuốc vang lên âm thanh khác lạ gần con đường, như thể lòng đất vẫn nhớ thứ kim loại bên dưới.";

                case "story.08.first-iron.title": return "Khai thác quặng sắt đầu tiên";
                case "story.08.first-iron.detail": return "Khai thác quặng sắt từ một mạch quặng lộ thiên.";
                case "story.08.first-iron.lore": return "Quặng sắt ấm áp trong tay bạn. Cha từng viết rằng các tháp chuông được xây dựng trên thứ kim loại ấm hơn.";

                case "story.09.prepare-food.title": return "Tìm kiếm thực phẩm đi đường";
                case "story.09.prepare-food.detail": return "Hái quả dại, thảo dược hoặc nấm trước khi màn đêm buông xuống.";
                case "story.09.prepare-food.lore": return "Con đường sẽ dễ đi hơn khi cơn đói không đưa ra quyết định thay bạn.";

                case "story.10.cabin-plank.title": return "Xẻ ván gỗ cabin đầu tiên";
                case "story.10.cabin-plank.detail": return "Chế tạo 1 tấm ván gỗ cabin.";
                case "story.10.cabin-plank.lore": return "Một tấm ván chưa thể làm nên ngôi nhà, nhưng là bằng chứng đầu tiên cho thấy bạn sẽ ở lại.";

                case "story.11.start-home.title": return "Dựng nơi trú ẩn Roadwarden";
                case "story.11.start-home.detail": return "Bắt đầu thi công một ngôi nhà gỗ hoặc nhà đá.";
                case "story.11.start-home.lore": return "Khung nhà hướng về Con Đường Cũ như một câu hỏi đang chờ đợi lời hồi đáp.";

                case "story.12.first-bell-fragment.title": return "Tìm mảnh chuông đầu tiên";
                case "story.12.first-bell-fragment.detail": return "Tìm lại mảnh chuông từ phía đông con đường.";
                case "story.12.first-bell-fragment.lore": return "Mảnh vỡ không rung chuông, nhưng nó khẽ ngân lên khi đặt gần trang nhật ký của cha.";

                case "story.13-light-fire.title": return "Nhóm lửa cho màn đêm";
                case "story.13-light-fire.detail": return "Hoàn thành bếp lửa trại hoặc bếp nấu ăn.";
                case "story.13-light-fire.lore": return "Ánh lửa tuy nhỏ, nhưng chính vì thế nó mới quan trọng.";

                case "story.14-cook-meal.title": return "Nấu một bữa ăn nóng";
                case "story.14-cook-meal.detail": return "Nấu một bữa ăn tại công trình bếp đã hoàn thành.";
                case "story.14-cook-meal.lore": return "Bữa ăn ấm biến một khu trại thành nơi mà người ta có thể tìm về.";

                case "story.15-animal-pen.title": return "Dựng chuồng nuôi đầu tiên";
                case "story.15-animal-pen.detail": return "Xây một chuồng nuôi nhỏ hoặc chuồng nuôi lớn.";
                case "story.15-animal-pen.lore": return "Con đường cần người bảo vệ, nhưng Valen trước hết cần cuộc sống thường nhật được tiếp diễn.";

                case "story.16-find-cave.title": return "Tìm Hang Động Blackwood";
                case "story.16-find-cave.detail": return "Khám phá cửa hang Blackwood.";
                case "story.16-find-cave.lore": return "Hang động thở ra luồng khí lạnh buốt. Dấu tích Roadwarden cảnh báo các đường hầm chạy sâu bên dưới khu rừng.";

                case "story.17-read-ridge.title": return "Đọc dấu tích trên Sườn Núi Vết Rồng";
                case "story.17-read-ridge.detail": return "Khám phá sườn núi bị thiêu rụi bởi ngọn lửa rồng cổ đại.";
                case "story.17-read-ridge.lore": return "Đá bị nung chảy chứng minh những truyền thuyết xưa không chỉ là hư cấu. Thứ gì đó đã thiêu rụi nơi này và sống sót qua ký ức.";

                // Landmarks
                case "landmark.waystone.north.title": return "Cột Mốc Phương Bắc";
                case "landmark.waystone.north.journal": return "Một cột đá nguyên khối cổ kính dẫn lối về phía bắc của Con Đường Cũ.";
                case "landmark.sign.road.title": return "Biển Báo Đường Cũ";
                case "landmark.sign.road.journal": return "Biển chỉ đường gỗ cũ kỹ ghi dấu ngã rẽ hướng tới các vùng định cư xa xôi.";
                case "landmark.arch.watch.title": return "Cổng Vọng Gác Đổ Nát";
                case "landmark.arch.watch.journal": return "Tàn tích của một vọng gác Roadwarden thời xưa bao quát toàn bộ con đèo.";
                case "landmark.bridge.river.title": return "Cầu Gỗ Qua Sông";
                case "landmark.bridge.river.journal": return "Cây cầu gỗ kiên cố bắc ngang dòng sông Valen chảy xiết.";
                case "landmark.camp.abandoned.title": return "Trại Bỏ Hoang";
                case "landmark.camp.abandoned.journal": return "Đống tro tàn và lều rách từ một đoàn thám hiểm trước đây.";
                case "landmark.bell.01.title": return "Cột Mốc Chuông Đông";
                case "landmark.bell.01.journal": return "Tháp chuông đá phủ đầy rêu phong, nơi các Roadwarden cổ xưa từng canh gác.";
                case "landmark.shrine.hunter.title": return "Đền Thờ Thợ Săn";
                case "landmark.shrine.hunter.journal": return "Miếu thờ bằng đá khắc cổ ngữ rừng sâu phù hộ cho những chuyến đi săn an toàn.";
                case "landmark.dragon.ridge.title": return "Sườn Núi Vết Rồng";
                case "landmark.dragon.ridge.journal": return "Vách đá cháy xém thủy tinh hóa mang dấu tích ngọn lửa cổ xưa.";
                case "landmark.ruin.south.title": return "Cổng Tàn Tích Phía Nam";
                case "landmark.ruin.south.journal": return "Cánh cổng đá đổ nát sừng sững tại ranh giới phía nam.";
                case "landmark.cave.blackwood.title": return "Cửa Hang Blackwood";
                case "landmark.cave.blackwood.journal": return "Cửa hang u tối dẫn sâu xuống những đường hầm buốt lạnh dưới lòng đất.";
                case "landmark.village.valen.title": return "Làng Valen";
                case "landmark.village.valen.journal": return "Khu định cư yên bình ven sông Valen với những ngôi nhà gỗ ấm cúng và cư dân thân thiện.";
                case "landmark.npc.eldon.title": return "Thương Nhân Eldon";
                case "landmark.npc.eldon.journal": return "Thương gia lữ hành buôn bán hạt giống quý, công cụ, thư khai hoang đất và thu mua nông sản.";
                case "landmark.service.bulletin.title": return "Bảng Đơn Hàng Thị Trấn";
                case "landmark.service.bulletin.journal": return "Bảng thông báo trung tâm nơi cư dân thị trấn đăng đơn hàng giao nộp nhận thưởng hàng ngày.";
                case "landmark.service.mailbox.title": return "Hòm Thư Quà Tặng Mỗi Ngày";
                case "landmark.service.mailbox.journal": return "Hòm thư bồ câu chuyển phát quà tặng điểm danh chuỗi 7 ngày và bưu phẩm.";
                case "landmark.farm.pasture.title": return "Trại Chăn Nuôi Avatar";
                case "landmark.farm.pasture.journal": return "Khu chăn nuôi rộng lớn với nhà chuồng bò đỏ, 2 bò sữa, 2 cừu lông, đàn gà đẻ trứng và chó giữ cổng.";
                case "landmark.farm.garden.title": return "Vườn Nông Trại Trồng Trọt";
                case "landmark.farm.garden.journal": return "Các luống đất màu mỡ trồng lúa mì, bắp ngọt, cà rốt giòn, khoai tây, cà chua và dứa hoàng gia.";

                default: return English(key);
            }
        }
    }
}
