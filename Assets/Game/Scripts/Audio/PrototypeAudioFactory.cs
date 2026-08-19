using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheOldRoad.Audio
{
    /// <summary>
    /// Procedural audio synthesizer for prototype SFX, Ambient, and BGM.
    /// Creates pure runtime AudioClips without external asset dependencies.
    /// </summary>
    public static class PrototypeAudioFactory
    {
        private const int SampleRate = 44100;
        private static readonly Dictionary<string, AudioClip> ClipCache = new Dictionary<string, AudioClip>();

        public static AudioClip Footstep => GetOrCreate("sfx.footstep", 0.09f, (t, d) =>
        {
            float noise = (Mathf.PerlinNoise(t * 800f, 0f) * 2f - 1f) * Mathf.Exp(-t * 35f);
            float lowThud = Mathf.Sin(2f * Mathf.PI * 90f * t) * Mathf.Exp(-t * 40f);
            return (noise * 0.4f + lowThud * 0.6f) * 0.6f;
        });

        public static AudioClip ChopWood => GetOrCreate("sfx.chop_wood", 0.22f, (t, d) =>
        {
            float click = (Mathf.PerlinNoise(t * 1200f, 0.5f) * 2f - 1f) * Mathf.Exp(-t * 60f);
            float body = Mathf.Sin(2f * Mathf.PI * 180f * t) * Mathf.Exp(-t * 22f);
            float tone = Mathf.Sin(2f * Mathf.PI * 340f * t) * Mathf.Exp(-t * 30f);
            return (click * 0.5f + body * 0.7f + tone * 0.3f) * 0.8f;
        });

        public static AudioClip MineStone => GetOrCreate("sfx.mine_stone", 0.25f, (t, d) =>
        {
            float clink = Mathf.Sin(2f * Mathf.PI * (1100f - t * 800f) * t) * Mathf.Exp(-t * 30f);
            float crack = (Mathf.PerlinNoise(t * 2400f, 1.2f) * 2f - 1f) * Mathf.Exp(-t * 45f);
            float bass = Mathf.Sin(2f * Mathf.PI * 140f * t) * Mathf.Exp(-t * 25f);
            return (clink * 0.55f + crack * 0.45f + bass * 0.4f) * 0.85f;
        });

        public static AudioClip ForageHerb => GetOrCreate("sfx.forage", 0.18f, (t, d) =>
        {
            float rustle = (Mathf.PerlinNoise(t * 1600f, 3.4f) * 2f - 1f) * Mathf.Exp(-t * 20f);
            float pluck = Mathf.Sin(2f * Mathf.PI * (480f + t * 300f) * t) * Mathf.Exp(-t * 25f);
            return (rustle * 0.45f + pluck * 0.55f) * 0.75f;
        });

        public static AudioClip LootPickup => GetOrCreate("sfx.loot_pickup", 0.35f, (t, d) =>
        {
            float freq = t < 0.12f ? 587.33f : (t < 0.24f ? 739.99f : 880f); // D5, F#5, A5
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-(t % 0.12f) * 16f);
            return tone * 0.6f;
        });

        public static AudioClip CraftHammer => GetOrCreate("sfx.craft", 0.28f, (t, d) =>
        {
            float anvil = Mathf.Sin(2f * Mathf.PI * 1480f * t) * Mathf.Exp(-t * 20f);
            float thump = Mathf.Sin(2f * Mathf.PI * 220f * t) * Mathf.Exp(-t * 35f);
            return (anvil * 0.6f + thump * 0.5f) * 0.8f;
        });

        public static AudioClip CookSizzle => GetOrCreate("sfx.cook", 0.65f, (t, d) =>
        {
            float noise = (Mathf.PerlinNoise(t * 3000f, 4.2f) * 2f - 1f);
            float bubble = Mathf.Sin(2f * Mathf.PI * (180f + Mathf.Sin(t * 40f) * 60f) * t) * 0.3f;
            float envelope = Mathf.Sin(Mathf.PI * (t / d));
            return (noise * 0.6f + bubble) * envelope * 0.65f;
        });

        public static AudioClip BuildPlace => GetOrCreate("sfx.build_place", 0.32f, (t, d) =>
        {
            float thud = Mathf.Sin(2f * Mathf.PI * 95f * t) * Mathf.Exp(-t * 18f);
            float wood = (Mathf.PerlinNoise(t * 1100f, 5f) * 2f - 1f) * Mathf.Exp(-t * 30f);
            return (thud * 0.7f + wood * 0.45f) * 0.85f;
        });

        public static AudioClip BuildComplete => GetOrCreate("sfx.build_complete", 0.55f, (t, d) =>
        {
            float freq = t < 0.16f ? 523.25f : (t < 0.32f ? 659.25f : 783.99f); // C5, E5, G5
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-(t % 0.16f) * 10f);
            return tone * 0.65f;
        });

        public static AudioClip SwordSlash => GetOrCreate("sfx.slash", 0.16f, (t, d) =>
        {
            float whoosh = (Mathf.PerlinNoise(t * 2200f, 6.7f) * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / d));
            float blade = Mathf.Sin(2f * Mathf.PI * (700f - t * 2500f) * t) * Mathf.Exp(-t * 25f);
            return (whoosh * 0.6f + blade * 0.4f) * 0.85f;
        });

        public static AudioClip HitImpact => GetOrCreate("sfx.hit_impact", 0.22f, (t, d) =>
        {
            float punch = Mathf.Sin(2f * Mathf.PI * 130f * t) * Mathf.Exp(-t * 30f);
            float crunch = (Mathf.PerlinNoise(t * 1800f, 7.8f) * 2f - 1f) * Mathf.Exp(-t * 35f);
            return (punch * 0.65f + crunch * 0.45f) * 0.9f;
        });

        public static AudioClip EnemyDefeated => GetOrCreate("sfx.enemy_defeated", 0.45f, (t, d) =>
        {
            float boom = Mathf.Sin(2f * Mathf.PI * (160f - t * 180f) * t) * Mathf.Exp(-t * 10f);
            float noise = (Mathf.PerlinNoise(t * 900f, 9.1f) * 2f - 1f) * Mathf.Exp(-t * 12f);
            return (boom * 0.7f + noise * 0.4f) * 0.85f;
        });

        public static AudioClip WolfGrowl => GetOrCreate("sfx.wolf_growl", 0.38f, (t, d) =>
        {
            float growl = Mathf.Sin(2f * Mathf.PI * (110f + Mathf.Sin(t * 60f) * 35f) * t);
            float noise = (Mathf.PerlinNoise(t * 1400f, 10.3f) * 2f - 1f) * 0.4f;
            float envelope = Mathf.Sin(Mathf.PI * (t / d));
            return (growl + noise) * envelope * 0.7f;
        });

        public static AudioClip PlayerHurt => GetOrCreate("sfx.player_hurt", 0.25f, (t, d) =>
        {
            float grunt = Mathf.Sin(2f * Mathf.PI * (150f - t * 200f) * t) * Mathf.Exp(-t * 20f);
            float thud = Mathf.Sin(2f * Mathf.PI * 75f * t) * Mathf.Exp(-t * 25f);
            return (grunt * 0.5f + thud * 0.6f) * 0.85f;
        });

        public static AudioClip DoorLatch => GetOrCreate("sfx.door_latch", 0.26f, (t, d) =>
        {
            float click = Mathf.Sin(2f * Mathf.PI * 880f * t) * Mathf.Exp(-t * 40f);
            float creak = Mathf.Sin(2f * Mathf.PI * (260f + Mathf.Sin(t * 50f) * 40f) * t) * Mathf.Exp(-t * 15f);
            return (click * 0.5f + creak * 0.6f) * 0.75f;
        });

        public static AudioClip SleepMorning => GetOrCreate("sfx.sleep_morning", 1.2f, (t, d) =>
        {
            float f1 = Mathf.Sin(2f * Mathf.PI * 440f * t) * Mathf.Exp(-t * 3f);
            float f2 = t > 0.35f ? Mathf.Sin(2f * Mathf.PI * 554.37f * (t - 0.35f)) * Mathf.Exp(-(t - 0.35f) * 3f) : 0f;
            float f3 = t > 0.7f ? Mathf.Sin(2f * Mathf.PI * 659.25f * (t - 0.7f)) * Mathf.Exp(-(t - 0.7f) * 2.5f) : 0f;
            return (f1 + f2 + f3) * 0.5f;
        });

        public static AudioClip UiClick => GetOrCreate("sfx.ui_click", 0.06f, (t, d) =>
        {
            float blip = Mathf.Sin(2f * Mathf.PI * 1200f * t) * Mathf.Exp(-t * 70f);
            return blip * 0.55f;
        });

        public static AudioClip QuestComplete => GetOrCreate("sfx.quest_complete", 0.9f, (t, d) =>
        {
            float[] freqs = { 440f, 554.37f, 659.25f, 880f }; // A4, C#5, E5, A5
            int idx = Mathf.Clamp(Mathf.FloorToInt(t / 0.22f), 0, freqs.Length - 1);
            float subT = t - idx * 0.22f;
            float tone = Mathf.Sin(2f * Mathf.PI * freqs[idx] * subT) * Mathf.Exp(-subT * 8f);
            return tone * 0.65f;
        });

        public static AudioClip OverworldMusic => GetOrLoadWav("bgm.overworld", "Assets/Game/Audio/Music/bgm_peaceful_valley.wav", 16.0f, (t, d) =>
        {
            // Warm pastoral Celtic / G-Major acoustic guitar arpeggios + flute
            float[] progression = { 392.00f, 329.63f, 293.66f, 261.63f, 329.63f, 349.23f, 392.00f, 293.66f };
            int chordIdx = Mathf.Clamp(Mathf.FloorToInt((t / d) * progression.Length), 0, progression.Length - 1);
            float rootFreq = progression[chordIdx];
            float chordT = (t % (d / progression.Length));

            // Warm cello bass
            float bass = Mathf.Sin(2f * Mathf.PI * (rootFreq * 0.5f) * t) * 0.22f;
            // Warm nylon guitar pluck with gentle exponential decay
            float guitar = (Mathf.Sin(2f * Mathf.PI * rootFreq * t) + Mathf.Sin(2f * Mathf.PI * rootFreq * 2f * t) * 0.35f) * Mathf.Exp(-chordT * 2.2f) * 0.30f;
            // Gentle flute melody with expressive vibrato
            float vibrato = Mathf.Sin(2f * Mathf.PI * 5.2f * t) * 0.008f;
            float flute = (Mathf.Sin(2f * Mathf.PI * (rootFreq * (1f + vibrato)) * t) + Mathf.Sin(2f * Mathf.PI * rootFreq * 2f * t) * 0.2f) * 0.22f;

            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (bass + guitar + flute) * loopFade * 0.65f;
        });

        public static AudioClip OverworldMusicNight => GetOrLoadWav("bgm.overworld.night", "Assets/Game/Audio/Music/bgm_night_peace.wav", 16.0f, (t, d) =>
        {
            // Calm night lullaby with soft music box & harp
            float[] nightChords = { 329.63f, 261.63f, 392.00f, 293.66f, 329.63f, 440.00f, 246.94f, 329.63f };
            int idx = Mathf.Clamp(Mathf.FloorToInt((t / d) * nightChords.Length), 0, nightChords.Length - 1);
            float root = nightChords[idx];
            float chordT = (t % (d / nightChords.Length));

            float pad = Mathf.Sin(2f * Mathf.PI * (root * 0.5f) * t) * 0.18f;
            float musicBox = (Mathf.Sin(2f * Mathf.PI * root * 2f * t) + Mathf.Sin(2f * Mathf.PI * root * 4f * t) * 0.15f) * Mathf.Exp(-chordT * 1.8f) * 0.25f;

            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (pad + musicBox) * loopFade * 0.6f;
        });

        public static AudioClip AmbientDay => GetOrLoadWav("ambient.day", "Assets/Game/Audio/Ambient/ambient_day_nature.wav", 8.0f, (t, d) =>
        {
            // Gentle soothing breeze (pink noise filtered), 100% free of high frequency beeps
            float wind = (Mathf.PerlinNoise(t * 8f, 2.5f) * 2f - 1f) * 0.12f;
            float breeze = Mathf.Sin(2f * Mathf.PI * 0.25f * t) * 0.06f;
            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (wind + breeze) * loopFade * 0.45f;
        });

        public static AudioClip AmbientNight => GetOrLoadWav("ambient.night", "Assets/Game/Audio/Ambient/ambient_night_peace.wav", 8.0f, (t, d) =>
        {
            // Soft calm night breeze, 100% free of high frequency harsh tones
            float wind = (Mathf.PerlinNoise(t * 6f, 7.1f) * 2f - 1f) * 0.10f;
            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return wind * loopFade * 0.45f;
        });

        public static AudioClip RainLoop => GetOrCreate("ambient.rain", 6.0f, (t, d) =>
        {
            float hiss = (Mathf.PerlinNoise(t * 3200f, 1.1f) * 2f - 1f) * 0.28f;
            float drops = (Mathf.PerlinNoise(t * 850f, 4.4f) * 2f - 1f) * 0.18f;
            float deepRumble = Mathf.Sin(2f * Mathf.PI * 55f * t) * 0.08f;
            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (hiss + drops + deepRumble) * loopFade * 0.7f;
        });

        public static AudioClip Thunder => GetOrCreate("sfx.thunder", 1.8f, (t, d) =>
        {
            float boom = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(90f, 35f, t / d) * t) * Mathf.Exp(-t * 1.8f);
            float rumble = (Mathf.PerlinNoise(t * 180f, 9.2f) * 2f - 1f) * Mathf.Exp(-t * 1.2f) * 0.55f;
            return (boom * 0.7f + rumble * 0.5f) * 0.95f;
        });

        public static AudioClip WaterSplash => GetOrCreate("sfx.water_splash", 0.22f, (t, d) =>
        {
            float splash = (Mathf.PerlinNoise(t * 2200f, 6.7f) * 2f - 1f) * Mathf.Exp(-t * 22f);
            float bloop = Mathf.Sin(2f * Mathf.PI * (380f - t * 400f) * t) * Mathf.Exp(-t * 28f);
            return (splash * 0.55f + bloop * 0.6f) * 0.75f;
        });

        public static AudioClip ChestOpen => GetOrCreate("sfx.chest_open", 0.32f, (t, d) =>
        {
            float creak = Mathf.Sin(2f * Mathf.PI * (220f + t * 450f) * t) * Mathf.Exp(-t * 10f);
            float latch = (Mathf.PerlinNoise(t * 1800f, 0.4f) * 2f - 1f) * Mathf.Exp(-t * 40f);
            return (creak * 0.55f + latch * 0.45f) * 0.8f;
        });

        public static AudioClip TillSoil => GetOrCreate("sfx.till_soil", 0.26f, (t, d) =>
        {
            float crunch = (Mathf.PerlinNoise(t * 1900f, 12.1f) * 2f - 1f) * Mathf.Exp(-t * 30f);
            float earth = Mathf.Sin(2f * Mathf.PI * 110f * t) * Mathf.Exp(-t * 22f);
            return (crunch * 0.65f + earth * 0.5f) * 0.9f;
        });

        public static AudioClip WaterPour => GetOrCreate("sfx.water_pour", 0.48f, (t, d) =>
        {
            float hiss = (Mathf.PerlinNoise(t * 3500f, 15.3f) * 2f - 1f);
            float trickling = Mathf.Sin(2f * Mathf.PI * (420f + Mathf.Sin(t * 60f) * 120f) * t) * 0.3f;
            float envelope = Mathf.Sin(Mathf.PI * (t / d));
            return (hiss * 0.6f + trickling) * envelope * 0.8f;
        });

        public static AudioClip PlantSeed => GetOrCreate("sfx.plant_seed", 0.20f, (t, d) =>
        {
            float rustle = (Mathf.PerlinNoise(t * 2200f, 18.7f) * 2f - 1f) * Mathf.Exp(-t * 35f);
            float drop = Mathf.Sin(2f * Mathf.PI * (520f - t * 400f) * t) * Mathf.Exp(-t * 40f);
            return (rustle * 0.5f + drop * 0.6f) * 0.8f;
        });

        public static AudioClip CropFertilize => GetOrCreate("sfx.crop_fertilize", 0.42f, (t, d) =>
        {
            float sparkle1 = Mathf.Sin(2f * Mathf.PI * 1320f * t) * Mathf.Exp(-t * 14f);
            float sparkle2 = Mathf.Sin(2f * Mathf.PI * 1760f * t) * Mathf.Exp(-t * 12f);
            float dust = (Mathf.PerlinNoise(t * 2800f, 21.2f) * 2f - 1f) * Mathf.Exp(-t * 25f) * 0.4f;
            return (sparkle1 * 0.4f + sparkle2 * 0.4f + dust) * 0.8f;
        });

        public static AudioClip CropHarvest => GetOrCreate("sfx.crop_harvest", 0.30f, (t, d) =>
        {
            float pluck = Mathf.Sin(2f * Mathf.PI * (340f + t * 600f) * t) * Mathf.Exp(-t * 25f);
            float pop = Mathf.Sin(2f * Mathf.PI * (680f - t * 800f) * t) * Mathf.Exp(-t * 45f);
            float leaf = (Mathf.PerlinNoise(t * 1600f, 24.5f) * 2f - 1f) * Mathf.Exp(-t * 30f) * 0.4f;
            return (pluck * 0.5f + pop * 0.6f + leaf) * 0.85f;
        });

        public static AudioClip SprinklerSpray => GetOrCreate("sfx.sprinkler", 0.35f, (t, d) =>
        {
            float spray = (Mathf.PerlinNoise(t * 4200f, 28.1f) * 2f - 1f);
            float click = (t % 0.08f < 0.02f) ? Mathf.Sin(2f * Mathf.PI * 1800f * (t % 0.08f)) * 0.5f : 0f;
            float env = Mathf.Sin(Mathf.PI * (t / d));
            return (spray * 0.5f + click) * env * 0.7f;
        });

        public static AudioClip CowMoo => GetOrCreate("sfx.cow_moo", 0.85f, (t, d) =>
        {
            float pitch = 95f + Mathf.Sin(t * 6f) * 15f;
            float moo = Mathf.Sin(2f * Mathf.PI * pitch * t) + 0.5f * Mathf.Sin(2f * Mathf.PI * pitch * 2f * t);
            float env = Mathf.Sin(Mathf.PI * (t / d));
            return moo * env * 0.7f;
        });

        public static AudioClip CowMilking => GetOrCreate("sfx.cow_milking", 0.32f, (t, d) =>
        {
            float squirt = (Mathf.PerlinNoise(t * 3600f, 32.4f) * 2f - 1f) * Mathf.Exp(-t * 20f);
            float tinPail = Mathf.Sin(2f * Mathf.PI * 1150f * t) * Mathf.Exp(-t * 30f);
            return (squirt * 0.6f + tinPail * 0.45f) * 0.75f;
        });

        public static AudioClip SheepBaa => GetOrCreate("sfx.sheep_baa", 0.65f, (t, d) =>
        {
            float pitch = 220f + Mathf.Sin(t * 18f) * 35f;
            float baa = Mathf.Sin(2f * Mathf.PI * pitch * t);
            float env = Mathf.Sin(Mathf.PI * (t / d));
            return baa * env * 0.65f;
        });

        public static AudioClip SheepShearing => GetOrCreate("sfx.sheep_shearing", 0.28f, (t, d) =>
        {
            float snip = Mathf.Sin(2f * Mathf.PI * (1600f - t * 1200f) * t) * Mathf.Exp(-t * 40f);
            float wool = (Mathf.PerlinNoise(t * 2200f, 36.7f) * 2f - 1f) * Mathf.Exp(-t * 25f);
            return (snip * 0.6f + wool * 0.45f) * 0.8f;
        });

        public static AudioClip ChickenCluck => GetOrCreate("sfx.chicken_cluck", 0.22f, (t, d) =>
        {
            float cluck = Mathf.Sin(2f * Mathf.PI * (480f + Mathf.Sin(t * 50f) * 160f) * t) * Mathf.Exp(-t * 25f);
            return cluck * 0.7f;
        });

        public static AudioClip EggLaid => GetOrCreate("sfx.egg_laid", 0.25f, (t, d) =>
        {
            float straw = (Mathf.PerlinNoise(t * 1800f, 40.1f) * 2f - 1f) * Mathf.Exp(-t * 25f);
            float plop = Mathf.Sin(2f * Mathf.PI * 260f * t) * Mathf.Exp(-t * 35f);
            return (straw * 0.5f + plop * 0.6f) * 0.75f;
        });

        public static AudioClip DogBark => GetOrCreate("sfx.dog_bark", 0.24f, (t, d) =>
        {
            float bark = Mathf.Sin(2f * Mathf.PI * (280f - t * 450f) * t) * Mathf.Exp(-t * 22f);
            float body = Mathf.Sin(2f * Mathf.PI * 140f * t) * Mathf.Exp(-t * 28f);
            return (bark * 0.65f + body * 0.5f) * 0.8f;
        });

        public static AudioClip CatPurr => GetOrCreate("sfx.cat_purr", 0.60f, (t, d) =>
        {
            float purr = Mathf.Sin(2f * Mathf.PI * 45f * t) * (Mathf.Sin(2f * Mathf.PI * 12f * t) * 0.5f + 0.5f);
            float env = Mathf.Sin(Mathf.PI * (t / d));
            return purr * env * 0.65f;
        });

        public static AudioClip RodCast => GetOrCreate("sfx.rod_cast", 0.35f, (t, d) =>
        {
            float whoosh = (Mathf.PerlinNoise(t * 1800f, 44.2f) * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / d));
            float whip = Mathf.Sin(2f * Mathf.PI * (800f - t * 1400f) * t) * Mathf.Exp(-t * 30f);
            return (whoosh * 0.6f + whip * 0.5f) * 0.8f;
        });

        public static AudioClip FishBite => GetOrCreate("sfx.fish_bite", 0.28f, (t, d) =>
        {
            float bell1 = Mathf.Sin(2f * Mathf.PI * 1480f * t) * Mathf.Exp(-t * 30f);
            float bell2 = t > 0.10f ? Mathf.Sin(2f * Mathf.PI * 1980f * (t - 0.10f)) * Mathf.Exp(-(t - 0.10f) * 35f) : 0f;
            return (bell1 * 0.5f + bell2 * 0.5f) * 0.85f;
        });

        public static AudioClip ReelTension => GetOrCreate("sfx.reel_tension", 0.18f, (t, d) =>
        {
            float clicks = (t % 0.04f < 0.015f) ? Mathf.Sin(2f * Mathf.PI * 2200f * (t % 0.04f)) : 0f;
            return clicks * 0.6f;
        });

        public static AudioClip FishCatchSuccess => GetOrCreate("sfx.fish_success", 0.55f, (t, d) =>
        {
            float[] jingle = { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6
            int idx = Mathf.Clamp(Mathf.FloorToInt(t / 0.12f), 0, jingle.Length - 1);
            float subT = t - idx * 0.12f;
            float tone = Mathf.Sin(2f * Mathf.PI * jingle[idx] * subT) * Mathf.Exp(-subT * 12f);
            return tone * 0.7f;
        });

        public static AudioClip BowDraw => GetOrCreate("sfx.bow_draw", 0.32f, (t, d) =>
        {
            float creak = Mathf.Sin(2f * Mathf.PI * (260f + t * 500f) * t) * Mathf.Exp(-t * 10f);
            float wood = (Mathf.PerlinNoise(t * 1400f, 48.9f) * 2f - 1f) * Mathf.Exp(-t * 18f);
            return (creak * 0.55f + wood * 0.45f) * 0.75f;
        });

        public static AudioClip ArrowFly => GetOrCreate("sfx.arrow_fly", 0.22f, (t, d) =>
        {
            float whoosh = (Mathf.PerlinNoise(t * 2600f, 52.3f) * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / d));
            return whoosh * 0.7f;
        });

        public static AudioClip ShieldBlock => GetOrCreate("sfx.shield_block", 0.25f, (t, d) =>
        {
            float clank = Mathf.Sin(2f * Mathf.PI * (880f - t * 600f) * t) * Mathf.Exp(-t * 25f);
            float heavy = Mathf.Sin(2f * Mathf.PI * 160f * t) * Mathf.Exp(-t * 30f);
            return (clank * 0.6f + heavy * 0.5f) * 0.85f;
        });

        public static AudioClip MerchantWagonBell => GetOrCreate("sfx.merchant_bell", 0.95f, (t, d) =>
        {
            float b1 = Mathf.Sin(2f * Mathf.PI * 1174.66f * t) * Mathf.Exp(-t * 5f); // D6
            float b2 = t > 0.22f ? Mathf.Sin(2f * Mathf.PI * 1479.98f * (t - 0.22f)) * Mathf.Exp(-(t - 0.22f) * 5f) : 0f; // F#6
            float b3 = t > 0.45f ? Mathf.Sin(2f * Mathf.PI * 1760.00f * (t - 0.45f)) * Mathf.Exp(-(t - 0.45f) * 4f) : 0f; // A6
            return (b1 * 0.5f + b2 * 0.45f + b3 * 0.55f) * 0.8f;
        });

        public static AudioClip CoinJingle => GetOrCreate("sfx.coin_jingle", 0.35f, (t, d) =>
        {
            float c1 = Mathf.Sin(2f * Mathf.PI * 2489f * t) * Mathf.Exp(-t * 28f);
            float c2 = t > 0.08f ? Mathf.Sin(2f * Mathf.PI * 2960f * (t - 0.08f)) * Mathf.Exp(-(t - 0.08f) * 30f) : 0f;
            return (c1 * 0.55f + c2 * 0.55f) * 0.75f;
        });

        public static AudioClip TreeCreakFall => GetOrCreate("sfx.tree_fall", 0.80f, (t, d) =>
        {
            float creak = Mathf.Sin(2f * Mathf.PI * (180f - t * 90f) * t) * Mathf.Exp(-t * 8f);
            float crash = (Mathf.PerlinNoise(t * 1100f, 56.4f) * 2f - 1f) * (t > 0.35f ? Mathf.Exp(-(t - 0.35f) * 8f) : 0f);
            return (creak * 0.5f + crash * 0.7f) * 0.85f;
        });

        public static AudioClip RockBreak => GetOrCreate("sfx.rock_break", 0.45f, (t, d) =>
        {
            float shatter = (Mathf.PerlinNoise(t * 2800f, 60.1f) * 2f - 1f) * Mathf.Exp(-t * 20f);
            float bass = Mathf.Sin(2f * Mathf.PI * 90f * t) * Mathf.Exp(-t * 18f);
            return (shatter * 0.65f + bass * 0.5f) * 0.9f;
        });

        public static AudioClip GemDiscovery => GetOrCreate("sfx.gem_discovery", 0.65f, (t, d) =>
        {
            float chime1 = Mathf.Sin(2f * Mathf.PI * 1760f * t) * Mathf.Exp(-t * 8f);
            float chime2 = t > 0.15f ? Mathf.Sin(2f * Mathf.PI * 2637f * (t - 0.15f)) * Mathf.Exp(-(t - 0.15f) * 8f) : 0f;
            return (chime1 * 0.5f + chime2 * 0.5f) * 0.75f;
        });

        public static AudioClip MachineProcess => GetOrCreate("sfx.machine_process", 0.40f, (t, d) =>
        {
            float hum = Mathf.Sin(2f * Mathf.PI * (160f + Mathf.Sin(t * 30f) * 40f) * t);
            float clatter = (Mathf.PerlinNoise(t * 1400f, 64.2f) * 2f - 1f) * 0.4f;
            float env = Mathf.Sin(Mathf.PI * (t / d));
            return (hum + clatter) * env * 0.7f;
        });

        public static AudioClip MorningRooster => GetOrCreate("sfx.rooster", 0.90f, (t, d) =>
        {
            float pitch = 700f + Mathf.Sin(t * 8f) * 150f;
            float crow = Mathf.Sin(2f * Mathf.PI * pitch * t);
            float env = Mathf.Sin(Mathf.PI * (t / d));
            return crow * env * 0.65f;
        });

        private static AudioClip GetOrLoadWav(string key, string relativeAssetPath, float durationSeconds, Func<float, float, float> generator)
        {
            if (ClipCache.TryGetValue(key, out AudioClip cached) && cached != null)
            {
                return cached;
            }

            // Try loading from actual WAV asset file on disk
            string projectRoot = Application.dataPath;
            if (projectRoot.EndsWith("/Assets") || projectRoot.EndsWith("\\Assets"))
            {
                projectRoot = projectRoot.Substring(0, projectRoot.Length - 7);
            }
            string fullPath = System.IO.Path.Combine(projectRoot, relativeAssetPath);
            if (!System.IO.File.Exists(fullPath))
            {
                fullPath = System.IO.Path.Combine(Application.dataPath, relativeAssetPath.Replace("Assets/", ""));
            }

            if (System.IO.File.Exists(fullPath))
            {
                AudioClip loadedClip = LoadWavFromFile(fullPath, key);
                if (loadedClip != null)
                {
                    ClipCache[key] = loadedClip;
                    return loadedClip;
                }
            }

            // Fallback to generator
            return GetOrCreate(key, durationSeconds, generator);
        }

        public static AudioClip LoadWavFromFile(string fullPath, string clipName)
        {
            if (!System.IO.File.Exists(fullPath)) return null;
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                if (fileBytes.Length < 44) return null;

                int channels = BitConverter.ToInt16(fileBytes, 22);
                int sampleRate = BitConverter.ToInt32(fileBytes, 24);
                int bitsPerSample = BitConverter.ToInt16(fileBytes, 34);

                int pos = 12;
                while (pos < fileBytes.Length - 8)
                {
                    string chunkId = System.Text.Encoding.ASCII.GetString(fileBytes, pos, 4);
                    int chunkSize = BitConverter.ToInt32(fileBytes, pos + 4);
                    if (chunkId == "data")
                    {
                        int dataOffset = pos + 8;
                        int totalBytes = Math.Min(chunkSize, fileBytes.Length - dataOffset);
                        int bytesPerSample = bitsPerSample / 8;
                        int sampleCount = totalBytes / bytesPerSample / channels;
                        float[] audioData = new float[sampleCount * channels];

                        if (bitsPerSample == 16)
                        {
                            for (int i = 0; i < audioData.Length; i++)
                            {
                                int byteIdx = dataOffset + i * 2;
                                if (byteIdx + 1 < fileBytes.Length)
                                {
                                    short sample = BitConverter.ToInt16(fileBytes, byteIdx);
                                    audioData[i] = sample / 32768.0f;
                                }
                            }
                        }

                        AudioClip clip = AudioClip.Create(clipName, sampleCount, channels, sampleRate, false);
                        clip.SetData(audioData, 0);
                        clip.hideFlags = HideFlags.DontSave;
                        return clip;
                    }
                    pos += 8 + chunkSize;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AudioFactory] Failed to load WAV: {ex.Message}");
            }
            return null;
        }

        private static AudioClip GetOrCreate(string key, float durationSeconds, Func<float, float, float> generator)
        {
            if (ClipCache.TryGetValue(key, out AudioClip cached) && cached != null)
            {
                return cached;
            }

            int totalSamples = Mathf.Max(1, Mathf.RoundToInt(durationSeconds * SampleRate));
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = i / (float)SampleRate;
                samples[i] = Mathf.Clamp(generator(t, durationSeconds), -1f, 1f);
            }

            AudioClip clip = AudioClip.Create(key, totalSamples, 1, SampleRate, false);
            clip.SetData(samples, 0);
            clip.hideFlags = HideFlags.DontSave;
            ClipCache[key] = clip;
            return clip;
        }
    }
}
