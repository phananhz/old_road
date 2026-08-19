using UnityEngine;

namespace TheOldRoad.Audio
{
    /// <summary>
    /// Central audio manager managing BGM, Ambient Day/Night, Rain, and SFX channels.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        private const string PrefKeyMasterVolume = "the_old_road.audio.master";
        private const string PrefKeyMusicVolume = "the_old_road.audio.music";
        private const string PrefKeySfxVolume = "the_old_road.audio.sfx";
        private const string PrefKeyMuted = "the_old_road.audio.muted";

        private static AudioManager instance;

        private AudioSource musicSource;
        private AudioSource musicNightSource;
        private AudioSource ambientDaySource;
        private AudioSource ambientNightSource;
        private AudioSource rainSource;
        private AudioSource sfxSource;

        private float masterVolume = 1f;
        private float musicVolume = 0.8f;
        private float sfxVolume = 0.9f;
        private bool muted = false;

        private float footstepTimer;
        private float targetNightBlend;
        private float currentNightBlend;
        private float targetRainIntensity;
        private float currentRainIntensity;

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<AudioManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        instance = go.AddComponent<AudioManager>();
                    }
                }
                return instance;
            }
        }

        public static float MasterVolume
        {
            get => Instance.masterVolume;
            set => Instance.SetMasterVolume(value);
        }

        public static float MusicVolume
        {
            get => Instance.musicVolume;
            set => Instance.SetMusicVolume(value);
        }

        public static float SfxVolume
        {
            get => Instance.sfxVolume;
            set => Instance.SetSfxVolume(value);
        }

        public static bool IsMuted
        {
            get => Instance.muted;
            set => Instance.SetMuted(value);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureInitialized();
        }

        public void EnsureInitialized()
        {
            EnsureAudioListener();
            LoadPreferences();
            EnsureSources();
            StartAmbientAndMusic();
        }

        public void EnsureAudioListener()
        {
            if (FindAnyObjectByType<AudioListener>() == null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    cam.gameObject.AddComponent<AudioListener>();
                }
                else
                {
                    gameObject.AddComponent<AudioListener>();
                }
            }
        }

        private void Update()
        {
            currentNightBlend = Mathf.MoveTowards(currentNightBlend, targetNightBlend, UnityEngine.Time.deltaTime * 0.5f);
            currentRainIntensity = Mathf.MoveTowards(currentRainIntensity, targetRainIntensity, UnityEngine.Time.deltaTime * 0.4f);
            ApplyVolumes();
        }

        public static void SetNightBlend(float night01)
        {
            if (instance != null)
            {
                instance.targetNightBlend = Mathf.Clamp01(night01);
            }
        }

        public static void SetRainIntensity(float rain01)
        {
            if (instance != null)
            {
                instance.targetRainIntensity = Mathf.Clamp01(rain01);
            }
        }

        public static void PlaySfx(AudioClip clip, float volumeScale = 1f, float pitchVariance = 0.05f)
        {
            if (clip == null || Instance == null || Instance.muted) return;

            Instance.EnsureSources();
            if (pitchVariance > 0f)
            {
                Instance.sfxSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
            }
            else
            {
                Instance.sfxSource.pitch = 1f;
            }

            float finalVolume = Instance.masterVolume * Instance.sfxVolume * volumeScale;
            Instance.sfxSource.PlayOneShot(clip, finalVolume);
        }

        public static void PlayFootstep()
        {
            if (UnityEngine.Time.unscaledTime < Instance.footstepTimer) return;
            Instance.footstepTimer = UnityEngine.Time.unscaledTime + 0.30f;
            PlaySfx(PrototypeAudioFactory.Footstep, 0.6f, 0.08f);
        }

        public static void PlayChopWood() => PlaySfx(PrototypeAudioFactory.ChopWood, 0.95f, 0.06f);
        public static void PlayMineStone() => PlaySfx(PrototypeAudioFactory.MineStone, 0.95f, 0.06f);
        public static void PlayForage() => PlaySfx(PrototypeAudioFactory.ForageHerb, 0.85f, 0.05f);
        public static void PlayLoot() => PlaySfx(PrototypeAudioFactory.LootPickup, 0.9f, 0.02f);
        public static void PlayCraft() => PlaySfx(PrototypeAudioFactory.CraftHammer, 0.95f, 0.04f);
        public static void PlayCook() => PlaySfx(PrototypeAudioFactory.CookSizzle, 0.95f, 0.02f);
        public static void PlayBuildPlace() => PlaySfx(PrototypeAudioFactory.BuildPlace, 0.95f, 0.04f);
        public static void PlayBuildComplete() => PlaySfx(PrototypeAudioFactory.BuildComplete, 0.95f, 0.02f);
        public static void PlaySwordSlash() => PlaySfx(PrototypeAudioFactory.SwordSlash, 0.9f, 0.08f);
        public static void PlayHitImpact() => PlaySfx(PrototypeAudioFactory.HitImpact, 0.95f, 0.08f);
        public static void PlayEnemyGrowl() => PlaySfx(PrototypeAudioFactory.WolfGrowl, 0.85f, 0.05f);
        public static void PlayEnemyDefeated() => PlaySfx(PrototypeAudioFactory.EnemyDefeated, 0.95f, 0.04f);
        public static void PlayPlayerHurt() => PlaySfx(PrototypeAudioFactory.PlayerHurt, 0.95f, 0.05f);
        public static void PlayDoor() => PlaySfx(PrototypeAudioFactory.DoorLatch, 0.85f, 0.03f);
        public static void PlayDoorTransition() => PlayDoor();
        public static void PlaySleepMorning() => PlaySfx(PrototypeAudioFactory.SleepMorning, 0.95f, 0f);
        public static void PlayUiClick() => PlaySfx(PrototypeAudioFactory.UiClick, 0.7f, 0.04f);
        public static void PlayQuestComplete() => PlaySfx(PrototypeAudioFactory.QuestComplete, 1.0f, 0f);
        public static void PlayThunder() => PlaySfx(PrototypeAudioFactory.Thunder, 0.95f, 0.04f);
        public static void PlayWaterSplash() => PlaySfx(PrototypeAudioFactory.WaterSplash, 0.75f, 0.08f);
        public static void PlayChestOpen() => PlaySfx(PrototypeAudioFactory.ChestOpen, 0.85f, 0.04f);
        public static void PlayGatherSuccess() => PlayForage();
        public static void PlayMiningImpact() => PlayMineStone();
        public static void PlayAttackImpact() => PlayHitImpact();
        public static void PlayAttackSwing() => PlaySwordSlash();
        public static void PlayItemPickup() => PlayLoot();
        public static void PlayHarvestProduce() => PlayCropHarvest();
        public static void PlaySlashVfx() => PlaySwordSlash();
        public static void PlayLandmarkDiscovered() => PlayQuestComplete();

        // Nông nghiệp & Trồng trọt
        public static void PlayTillSoil() => PlaySfx(PrototypeAudioFactory.TillSoil, 0.95f, 0.06f);
        public static void PlayWaterPour() => PlaySfx(PrototypeAudioFactory.WaterPour, 0.90f, 0.05f);
        public static void PlayPlantSeed() => PlaySfx(PrototypeAudioFactory.PlantSeed, 0.85f, 0.05f);
        public static void PlayCropFertilize() => PlaySfx(PrototypeAudioFactory.CropFertilize, 0.90f, 0.04f);
        public static void PlayCropHarvest() => PlaySfx(PrototypeAudioFactory.CropHarvest, 0.95f, 0.05f);
        public static void PlaySprinkler() => PlaySfx(PrototypeAudioFactory.SprinklerSpray, 0.70f, 0.03f);

        // Chăn nuôi & Thú cưng
        public static void PlayCowMoo() => PlaySfx(PrototypeAudioFactory.CowMoo, 0.90f, 0.04f);
        public static void PlayCowMilking() => PlaySfx(PrototypeAudioFactory.CowMilking, 0.85f, 0.04f);
        public static void PlaySheepBaa() => PlaySfx(PrototypeAudioFactory.SheepBaa, 0.85f, 0.05f);
        public static void PlaySheepShearing() => PlaySfx(PrototypeAudioFactory.SheepShearing, 0.85f, 0.04f);
        public static void PlayChickenCluck() => PlaySfx(PrototypeAudioFactory.ChickenCluck, 0.80f, 0.06f);
        public static void PlayEggLaid() => PlaySfx(PrototypeAudioFactory.EggLaid, 0.85f, 0.04f);
        public static void PlayDogBark() => PlaySfx(PrototypeAudioFactory.DogBark, 0.85f, 0.05f);
        public static void PlayCatPurr() => PlaySfx(PrototypeAudioFactory.CatPurr, 0.75f, 0.03f);

        // Câu cá
        public static void PlayRodCast() => PlaySfx(PrototypeAudioFactory.RodCast, 0.85f, 0.05f);
        public static void PlayFishBite() => PlaySfx(PrototypeAudioFactory.FishBite, 0.95f, 0.03f);
        public static void PlayReelTension() => PlaySfx(PrototypeAudioFactory.ReelTension, 0.75f, 0.04f);
        public static void PlayFishSuccess() => PlaySfx(PrototypeAudioFactory.FishCatchSuccess, 0.95f, 0f);

        // Chiến đấu & Vũ khí
        public static void PlayBowDraw() => PlaySfx(PrototypeAudioFactory.BowDraw, 0.85f, 0.04f);
        public static void PlayArrowFly() => PlaySfx(PrototypeAudioFactory.ArrowFly, 0.80f, 0.06f);
        public static void PlayShieldBlock() => PlaySfx(PrototypeAudioFactory.ShieldBlock, 0.95f, 0.05f);

        // Thương nhân, Máy móc & Môi trường
        public static void PlayMerchantBell() => PlaySfx(PrototypeAudioFactory.MerchantWagonBell, 1.0f, 0f);
        public static void PlayCoinJingle() => PlaySfx(PrototypeAudioFactory.CoinJingle, 0.90f, 0.04f);
        public static void PlayTreeFall() => PlaySfx(PrototypeAudioFactory.TreeCreakFall, 0.95f, 0.04f);
        public static void PlayRockBreak() => PlaySfx(PrototypeAudioFactory.RockBreak, 0.95f, 0.05f);
        public static void PlayGemDiscovery() => PlaySfx(PrototypeAudioFactory.GemDiscovery, 0.95f, 0f);
        public static void PlayMachineProcess() => PlaySfx(PrototypeAudioFactory.MachineProcess, 0.80f, 0.04f);
        public static void PlayRooster() => PlaySfx(PrototypeAudioFactory.MorningRooster, 0.85f, 0.03f);

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(PrefKeyMasterVolume, masterVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(PrefKeyMusicVolume, musicVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(PrefKeySfxVolume, sfxVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void SetMuted(bool isMuted)
        {
            muted = isMuted;
            PlayerPrefs.SetInt(PrefKeyMuted, muted ? 1 : 0);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        private void LoadPreferences()
        {
            masterVolume = PlayerPrefs.GetFloat(PrefKeyMasterVolume, 1f);
            musicVolume = PlayerPrefs.GetFloat(PrefKeyMusicVolume, 0.8f);
            sfxVolume = PlayerPrefs.GetFloat(PrefKeySfxVolume, 0.9f);
            muted = PlayerPrefs.GetInt(PrefKeyMuted, 0) == 1;
        }

        private void EnsureSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.spatialBlend = 0f;
            }

            if (musicNightSource == null)
            {
                musicNightSource = gameObject.AddComponent<AudioSource>();
                musicNightSource.loop = true;
                musicNightSource.playOnAwake = false;
                musicNightSource.spatialBlend = 0f;
            }

            if (ambientDaySource == null)
            {
                ambientDaySource = gameObject.AddComponent<AudioSource>();
                ambientDaySource.loop = true;
                ambientDaySource.playOnAwake = false;
                ambientDaySource.spatialBlend = 0f;
            }

            if (ambientNightSource == null)
            {
                ambientNightSource = gameObject.AddComponent<AudioSource>();
                ambientNightSource.loop = true;
                ambientNightSource.playOnAwake = false;
                ambientNightSource.spatialBlend = 0f;
            }

            if (rainSource == null)
            {
                rainSource = gameObject.AddComponent<AudioSource>();
                rainSource.loop = true;
                rainSource.playOnAwake = false;
                rainSource.spatialBlend = 0f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f;
            }
        }

        private void StartAmbientAndMusic()
        {
            if (musicSource != null && !musicSource.isPlaying)
            {
                musicSource.clip = PrototypeAudioFactory.OverworldMusic;
                musicSource.Play();
            }

            if (musicNightSource != null && !musicNightSource.isPlaying)
            {
                musicNightSource.clip = PrototypeAudioFactory.OverworldMusicNight;
                musicNightSource.Play();
            }

            if (ambientDaySource != null && !ambientDaySource.isPlaying)
            {
                ambientDaySource.clip = PrototypeAudioFactory.AmbientDay;
                ambientDaySource.Play();
            }

            if (ambientNightSource != null && !ambientNightSource.isPlaying)
            {
                ambientNightSource.clip = PrototypeAudioFactory.AmbientNight;
                ambientNightSource.Play();
            }

            if (rainSource != null && !rainSource.isPlaying)
            {
                rainSource.clip = PrototypeAudioFactory.RainLoop;
                rainSource.Play();
            }

            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            if (muted)
            {
                if (musicSource != null) musicSource.volume = 0f;
                if (musicNightSource != null) musicNightSource.volume = 0f;
                if (ambientDaySource != null) ambientDaySource.volume = 0f;
                if (ambientNightSource != null) ambientNightSource.volume = 0f;
                if (rainSource != null) rainSource.volume = 0f;
                return;
            }

            float effectiveMaster = masterVolume;
            if (musicSource != null)
            {
                musicSource.volume = effectiveMaster * musicVolume * (1f - currentNightBlend) * 0.65f;
            }

            if (musicNightSource != null)
            {
                musicNightSource.volume = effectiveMaster * musicVolume * currentNightBlend * 0.65f;
            }

            if (ambientDaySource != null)
            {
                ambientDaySource.volume = effectiveMaster * (1f - currentNightBlend) * 0.45f;
            }

            if (ambientNightSource != null)
            {
                ambientNightSource.volume = effectiveMaster * currentNightBlend * 0.45f;
            }

            if (rainSource != null)
            {
                rainSource.volume = effectiveMaster * currentRainIntensity * 0.55f;
            }
        }
    }
}
