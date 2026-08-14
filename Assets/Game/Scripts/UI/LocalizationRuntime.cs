using UnityEngine;

namespace TheOldRoad.UI
{
    /// <summary>Small PlayerPrefs-backed localization layer for prototype UI text.</summary>
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
                case "housing": return "Housing";
                case "housing_desc": return "Homes and shelters";
                case "fire_light": return "Fire & Light";
                case "fire_light_desc": return "Warmth and camp utility";
                case "animal_pens": return "Animal Pens";
                case "animal_pens_desc": return "Fenced square or rectangle yards";
                case "build_select_hint": return "Select a buildable card to enter placement mode.";
                case "building_cabin": return "Cabin";
                case "building_cabin_desc": return "Starter home with bed and interior.";
                case "building_cottage": return "Stone Cottage";
                case "building_cottage_desc": return "Larger stone home prototype.";
                case "building_shed": return "Storage Shed";
                case "building_shed_desc": return "Small utility storage building.";
                case "building_campfire": return "Campfire";
                case "building_campfire_desc": return "Small outdoor fire, light source, and cooking spot.";
                case "building_hearth": return "Cooking Hearth";
                case "building_hearth_desc": return "Stronger cooking station with warm light.";
                case "building_pen_small": return "Small Animal Pen";
                case "building_pen_small_desc": return "Square fenced yard. Produces eggs in prototype.";
                case "building_pen_long": return "Long Animal Pen";
                case "building_pen_long_desc": return "Rectangle fenced yard. Produces wool in prototype.";
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
                case "item.wood": return "Wood";
                case "item.stone": return "Stone";
                case "item.cabin-plank": return "Cabin Plank";
                case "item.tool-axe": return "Worn Axe";
                case "item.tool-pickaxe": return "Stone Pick";
                case "item.wild-berries": return "Wild Berries";
                case "item.medicinal-herb": return "Medicinal Herb";
                case "item.mushroom": return "Mushroom";
                case "item.iron-ore": return "Iron Ore";
                case "item.torch": return "Torch";
                case "item.bell-fragment": return "Bell Fragment";
                case "item.old-coin": return "Old Coin";
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
                case "craft": return "Chế tạo";
                case "build": return "Xây";
                case "cook": return "Nấu";
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
                case "housing": return "Nhà ở";
                case "housing_desc": return "Nhà và nơi trú ẩn";
                case "fire_light": return "Lửa & ánh sáng";
                case "fire_light_desc": return "Giữ ấm và tiện ích trại";
                case "animal_pens": return "Chuồng nuôi";
                case "animal_pens_desc": return "Rào vuông hoặc chữ nhật";
                case "build_select_hint": return "Chọn thẻ công trình để vào chế độ đặt.";
                case "building_cabin": return "Nhà gỗ";
                case "building_cabin_desc": return "Nhà khởi đầu có giường và nội thất.";
                case "building_cottage": return "Nhà đá";
                case "building_cottage_desc": return "Bản thử nghiệm nhà đá rộng hơn.";
                case "building_shed": return "Kho nhỏ";
                case "building_shed_desc": return "Công trình tiện ích kích thước nhỏ.";
                case "building_campfire": return "Bếp lửa";
                case "building_campfire_desc": return "Nguồn sáng nhỏ ngoài trời kiêm điểm nấu.";
                case "building_hearth": return "Bếp nấu";
                case "building_hearth_desc": return "Trạm nấu mạnh hơn với ánh sáng ấm.";
                case "building_pen_small": return "Chuồng nhỏ";
                case "building_pen_small_desc": return "Sân rào vuông. Prototype tạo trứng.";
                case "building_pen_long": return "Chuồng dài";
                case "building_pen_long_desc": return "Sân rào chữ nhật. Prototype tạo len.";
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
                case "legend_loot": return "Rương";
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
                case "item.wood": return "Gỗ";
                case "item.stone": return "Đá";
                case "item.cabin-plank": return "Ván gỗ";
                case "item.tool-axe": return "Rìu cũ";
                case "item.tool-pickaxe": return "Cuốc đá";
                case "item.wild-berries": return "Quả dại";
                case "item.medicinal-herb": return "Thảo dược";
                case "item.mushroom": return "Nấm";
                case "item.iron-ore": return "Quặng sắt";
                case "item.torch": return "Đuốc";
                case "item.bell-fragment": return "Mảnh chuông";
                case "item.old-coin": return "Xu cũ";
                case "speech.gather_start": return "Để xem có dùng được gì ở đây.";
                case "speech.gather_done": return "Thứ này sẽ có ích trên đường.";
                case "speech.gather_blocked": return "Mình cần đúng công cụ trước.";
                case "speech.gather_cancelled": return "Xa quá. Lát nữa quay lại.";
                case "speech.action_busy": return "Làm từng việc một thôi.";
                case "speech.craft_done": return "Tốt. Cái này sẽ hữu dụng.";
                case "speech.craft_blocked": return "Mình vẫn cần thêm nguyên liệu.";
                case "speech.cook_done": return "Đồ ăn nóng. Dễ chịu hơn rồi.";
                case "speech.cook_blocked": return "Mình cần quả dại và thứ gì đó để nấu.";
                case "speech.build_begin": return "Chỗ này có thể dựng được thứ gì đó.";
                case "speech.build_started": return "Công việc bắt đầu rồi.";
                case "speech.build_invalid": return "Nền đất này không ổn.";
                case "speech.build_blocked": return "Mình không đủ nguyên liệu.";
                case "speech.loot_start": return "Có ai đó đã để lại thứ này.";
                case "speech.loot_done": return "Tìm được thứ gì đó rồi.";
                case "speech.landmark_start": return "Con Đường Cũ để lại dấu vết ở đây.";
                case "speech.landmark_done": return "Mình nên ghi lại điều này.";
                case "speech.enter_home": return "Ít nhất cũng có chỗ trú.";
                case "speech.exit_home": return "Trở lại con đường.";
                case "speech.sleep_prompt": return "Ngủ tám tiếng có thể giúp đầu óc tỉnh hơn.";
                case "speech.sleep_done": return "Mình thấy khỏe hơn rồi.";
                case "speech.sleep_cancelled": return "Chưa phải lúc.";
                default: return English(key);
            }
        }
    }
}
