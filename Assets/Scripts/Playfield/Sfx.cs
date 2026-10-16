using UnityEngine;

namespace MarioPie
{
    public static class Sfx
    {
        const float MatchVolume = 0.5f;
        const float WinVolume = 0.62f;

        static PieSfx bank;
        static SfxHost host;
        static AudioSource loop;
        static AudioSource music;
        static bool warnedBank;
        static bool warnedMatch;
        static bool warnedWin;

        public static void Grab()
        {
            if (!Ready())
                return;

            Play(bank.grab, 1.18f, 0.48f, 0f);
        }

        public static void Throw()
        {
            if (!Ready())
                return;

            Play(bank.throwWhoosh, 1f, 0.72f, 0.45f);
        }

        public static void Hit()
        {
            if (!Ready())
                return;

            Play(bank.faceHit, 1f, 0.95f, 0f);
            Play(bank.stun, 1f, 0.42f, 0f);
        }

        public static void Floor()
        {
            if (!Ready())
                return;

            Play(bank.floorSplat, 0.94f, 0.7f, 0f);
        }

        public static void SetHurry(bool on)
        {
            if (!Ready())
                return;

            if (!on)
            {
                if (loop.isPlaying)
                    loop.Stop();
                return;
            }

            if (bank.hurry == null || loop.isPlaying)
                return;

            loop.clip = bank.hurry;
            loop.volume = 0.28f;
            loop.pitch = 1f;
            loop.Play();
        }

        public static void PlayMatch()
        {
            PlayMusic(true);
        }

        public static void PlayWin()
        {
            PlayMusic(false);
        }

        public static void StopMusic()
        {
            if (music == null)
                return;

            music.Stop();
            music.clip = null;
        }

        static void PlayMusic(bool matchBed)
        {
            if (!Ready())
                return;

            var clip = matchBed ? bank.match : bank.win;
            if (clip == null)
            {
                if (!matchBed)
                    StopMusic();
                WarnMissingMusic(matchBed);
                return;
            }

            music.Stop();
            music.clip = clip;
            music.volume = matchBed ? MatchVolume : WinVolume;
            music.pitch = 1f;
            music.loop = matchBed;
            music.Play();
        }

        static void WarnMissingMusic(bool matchBed)
        {
            if (matchBed)
            {
                if (warnedMatch)
                    return;

                warnedMatch = true;
                Debug.LogWarning("Falta a música da partida no asset PieSfx.");
                return;
            }

            if (warnedWin)
                return;

            warnedWin = true;
            Debug.LogWarning("Falta a música de vitória no asset PieSfx.");
        }

        static void Play(AudioClip clip, float pitch, float volume, float maxSeconds)
        {
            if (!Ready() || clip == null)
                return;

            var go = new GameObject("Shot");
            go.transform.SetParent(host.transform, false);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.pitch = Mathf.Max(0.05f, pitch);
            source.volume = volume;
            source.clip = clip;
            source.Play();

            var life = clip.length / source.pitch;
            if (maxSeconds > 0f)
                life = Mathf.Min(life, maxSeconds);
            Object.Destroy(go, life + 0.05f);
        }

        static bool Ready()
        {
            if (host != null)
                return true;
            if (!Application.isPlaying)
                return false;

            bank = Resources.Load<PieSfx>("PieSfx");
            if (bank == null)
            {
                if (!warnedBank)
                {
                    warnedBank = true;
                    Debug.LogWarning("Falta o asset PieSfx.");
                }

                return false;
            }

            var root = new GameObject("Sfx");
            host = root.AddComponent<SfxHost>();
            loop = root.AddComponent<AudioSource>();
            loop.playOnAwake = false;
            loop.spatialBlend = 0f;
            loop.loop = true;
            music = root.AddComponent<AudioSource>();
            music.playOnAwake = false;
            music.spatialBlend = 0f;
            music.loop = true;
            return true;
        }
    }
}
