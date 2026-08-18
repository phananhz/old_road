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
                case "building_herbalist": return "Herbalist Hut";
                case "building_herbalist_desc": return "Thatch roof with dried herbs & flowerbeds.";
                case "building_tower": return "Lookout Tower";
                case "building_tower_desc": return "High observation stilt tower with beacon fire.";
                case "building_barn": return "Farm Barn";
                case "building_barn_desc": return "Corrugated tin roof barn with hay pile & tool room.";
                case "building_fence": return "Wood Fence";
                case "building_fence_desc": return "Timber fence post to enclose properties.";
                case "building_gate": return "Wood Gate";
                case "building_gate_desc": return "Openable wooden gate for fences.";
                case "building_fence_drag": return "Custom Drag Fence";
                case "building_fence_drag_desc": return "Click and drag to enclose any custom area.";
                case "building_fence_small": return "Small Yard Fence (6x4)";
                case "building_fence_small_desc": return "Complete 4-side perimeter fence with gate.";
                case "building_fence_med": return "Medium Estate Fence (10x8)";
                case "building_fence_med_desc": return "Spacious perimeter fence with gate and lanterns.";
                case "building_fence_lrg": return "Large Farmstead Fence (16x12)";
                case "building_fence_lrg_desc": return "Large farm enclosure for house, barn, and fields.";
                case "building_fence_grd": return "Grand Homestead Fence (24x16)";
                case "building_fence_grd_desc": return "Expansive estate perimeter fence.";
                case "fences_security": return "Fences & Security";
                case "fences_security_desc": return "Enclose farms & protect homes";
                case "paths_decor": return "Paths & Decor";
                case "paths_decor_desc": return "Paved paths & farm decorations";
                case "building_path_dirt": return "Dirt Path";
                case "building_path_dirt_desc": return "Compacted earthen trail. +15% move speed.";
                case "building_path_cobble": return "Cobblestone Path";
                case "building_path_cobble_desc": return "Interlocking stone road. +15% move speed.";
                case "building_scarecrow": return "Farm Scarecrow";
                case "building_scarecrow_desc": return "Straw scarecrow in blue coat to protect crops.";
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
                case "item.cooked-meal": return "Cooked Meal";
                case "item.egg": return "Egg";
                case "item.wool": return "Wool";
                case "item.milk": return "Milk Bottle";
                case "item.silver-coin": return "Silver Coin";
                case "item.watering-can": return "Watering Can";
                case "item.seed-wheat": return "Wheat Seeds";
                case "item.seed-corn": return "Corn Seeds";
                case "item.seed-carrot": return "Carrot Seeds";
                case "item.wheat": return "Golden Wheat";
                case "item.corn": return "Sweet Corn";
                case "item.carrot": return "Crisp Carrot";
                case "item.fence-wood": return "Wood Fence";
                case "item.gate-wood": return "Wood Gate";
                case "item.seed-pineapple": return "Pineapple Seeds";
                case "item.seed-tomato": return "Tomato Seeds";
                case "item.pineapple": return "Sweet Pineapple";
                case "item.tomato": return "Ripe Tomato";
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
                case "building_shed": return "Kho chứa";
                case "building_shed_desc": return "Công trình tiện ích cất trữ vật tư.";
                case "building_campfire": return "Bếp lửa";
                case "building_campfire_desc": return "Nguồn sáng ngoài trời kiêm điểm nấu ăn.";
                case "building_hearth": return "Bếp nấu";
                case "building_hearth_desc": return "Trạm nấu kiên cố với ánh sáng ấm áp.";
                case "building_pen_small": return "Chuồng nuôi nhỏ";
                case "building_pen_small_desc": return "Sân rào vuông. Thu hoạch trứng.";
                case "building_pen_long": return "Chuồng nuôi lớn";
                case "building_pen_long_desc": return "Sân rào chữ nhật. Thu hoạch len cừu.";
                case "building_herbalist": return "Nhà thảo dược";
                case "building_herbalist_desc": return "Nhà lợp rêu phơi thảo dược & giàn hoa tím.";
                case "building_tower": return "Chòi canh thám hiểm";
                case "building_tower_desc": return "Tháp gỗ cao thắp đuốc sáng rực suốt đêm.";
                case "building_barn": return "Nhà chuồng nông trại";
                case "building_barn_desc": return "Chuồng mái tôn có đống rơm và kho nông cụ.";
                case "building_fence": return "Hàng rào gỗ";
                case "building_fence_desc": return "Cọc rào gỗ bao quanh bảo vệ nhà & đất trồng.";
                case "building_gate": return "Cổng rào gỗ";
                case "building_gate_desc": return "Cổng rào gỗ mở/đóng ra vào tự do.";
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
                case "fences_security": return "Hàng rào & bảo vệ";
                case "fences_security_desc": return "Rào chắn đất trồng & bảo vệ nhà";
                case "paths_decor": return "Đường đi & trang trí";
                case "paths_decor_desc": return "Lát đường chạy nhanh và đạo cụ nông trại";
                case "building_path_dirt": return "Đường đất nện";
                case "building_path_dirt_desc": return "Đường đất nện nông thôn. Tăng 15% tốc độ chạy.";
                case "building_path_cobble": return "Đường đá sỏi";
                case "building_path_cobble_desc": return "Đường lát đá sỏi kiên cố. Tăng 15% tốc độ chạy.";
                case "building_scarecrow": return "Bù nhìn rơm";
                case "building_scarecrow_desc": return "Bù nhìn rơm mặc áo xanh giữ vườn bảo vệ mùa màng.";
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
                case "item.wheat": return "Lúa mì vàng";
                case "item.corn": return "Bắp ngọt";
                case "item.carrot": return "Cà rốt tươi";
                case "item.fence-wood": return "Hàng rào gỗ";
                case "item.gate-wood": return "Cổng rào gỗ";
                case "item.seed-pineapple": return "Hạt giống dứa";
                case "item.seed-tomato": return "Hạt giống cà chua";
                case "item.pineapple": return "Dứa vàng ngọt";
                case "item.tomato": return "Cà chua tươi";
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

                default: return English(key);
            }
        }
    }
}
