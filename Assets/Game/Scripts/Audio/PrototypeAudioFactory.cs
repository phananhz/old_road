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

        public static AudioClip OverworldMusic => GetOrCreate("bgm.overworld", 16.0f, (t, d) =>
        {
            // Gentle Celtic / medieval chord progression in D dorian (D, F, G, A)
            float[] progression = { 293.66f, 349.23f, 392.00f, 440.00f, 392.00f, 349.23f, 293.66f, 220.00f };
            int chordIdx = Mathf.Clamp(Mathf.FloorToInt((t / d) * progression.Length), 0, progression.Length - 1);
            float rootFreq = progression[chordIdx];
            float chordT = (t % (d / progression.Length));

            float bass = Mathf.Sin(2f * Mathf.PI * (rootFreq * 0.5f) * t) * 0.25f;
            float melody = Mathf.Sin(2f * Mathf.PI * rootFreq * t) * Mathf.Exp(-chordT * 1.2f) * 0.35f;
            float harmony = Mathf.Sin(2f * Mathf.PI * (rootFreq * 1.5f) * t) * 0.15f;
            float wind = (Mathf.PerlinNoise(t * 15f, 0f) * 2f - 1f) * 0.04f;

            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (bass + melody + harmony + wind) * loopFade * 0.55f;
        });

        public static AudioClip AmbientDay => GetOrCreate("ambient.day", 8.0f, (t, d) =>
        {
            float wind = (Mathf.PerlinNoise(t * 12f, 2.5f) * 2f - 1f) * 0.12f;
            float breeze = Mathf.Sin(2f * Mathf.PI * 0.4f * t) * 0.05f;
            float chirp = (t % 2.5f < 0.08f) ? Mathf.Sin(2f * Mathf.PI * 2800f * (t % 2.5f)) * 0.08f : 0f;
            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (wind + breeze + chirp) * loopFade * 0.6f;
        });

        public static AudioClip AmbientNight => GetOrCreate("ambient.night", 8.0f, (t, d) =>
        {
            float wind = (Mathf.PerlinNoise(t * 8f, 7.1f) * 2f - 1f) * 0.14f;
            float cricketPeriod = 0.45f;
            float cricketT = t % cricketPeriod;
            float cricket = (cricketT < 0.12f)
                ? Mathf.Sin(2f * Mathf.PI * 4500f * cricketT) * (Mathf.PerlinNoise(t * 80f, 0f) * 0.5f + 0.5f) * 0.09f
                : 0f;
            float loopFade = Mathf.Sin(Mathf.PI * (t / d));
            return (wind + cricket) * loopFade * 0.65f;
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
