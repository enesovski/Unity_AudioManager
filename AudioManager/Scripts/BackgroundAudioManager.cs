using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Solivagant.Audio
{
    public class BackgroundAudioManager : MonoBehaviour
    {
        private static BackgroundAudioManager instance;
        public static BackgroundAudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("BackgroundAudioManager");
                    instance = obj.AddComponent<BackgroundAudioManager>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }

        private Dictionary<string, AudioClipData> musicClips;
        private Dictionary<string, AudioClipData> environmentalClips;
        private AudioSource musicSourceA, musicSourceB, environmentSourceA, environmentSourceB;
        private bool isPlayingMusicA = true;
        private bool isPlayingEnvA = true;

        private string currentEnvironment = "Default";

        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup environmentalMixer;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            musicClips = new Dictionary<string, AudioClipData>();
            environmentalClips = new Dictionary<string, AudioClipData>();

            foreach (var clip in Resources.LoadAll<AudioClipData>("Audio/Musics"))
                musicClips[clip.clipName] = clip;

            foreach (var clip in Resources.LoadAll<AudioClipData>("Audio/Environmental"))
                environmentalClips[clip.clipName] = clip;

            musicSourceA = gameObject.AddComponent<AudioSource>();
            musicSourceB = gameObject.AddComponent<AudioSource>();
            musicSourceA.loop = true;
            musicSourceB.loop = true;

            environmentSourceA = gameObject.AddComponent<AudioSource>();
            environmentSourceB = gameObject.AddComponent<AudioSource>();
            environmentSourceA.loop = true;
            environmentSourceB.loop = true;

            environmentSourceA.outputAudioMixerGroup = environmentalMixer;
            environmentSourceB.outputAudioMixerGroup = environmentalMixer;

            PlayDefaultBackgroundAudio();
        }

        private void PlayDefaultBackgroundAudio()
        {
            if (musicClips.Count > 0)
                PlayMusic(musicClips.Keys.GetEnumerator().Current);

            if (environmentalClips.Count > 0)
                PlayEnvironmentSound("Default");
        }

        public void PlayMusic(string musicName)
        {
            if (!musicClips.TryGetValue(musicName, out AudioClipData clipData) || clipData.clip == null) return;
            StartCoroutine(FadeMusic(clipData));
        }

        private IEnumerator FadeMusic(AudioClipData newMusic)
        {
            AudioSource activeSource = isPlayingMusicA ? musicSourceA : musicSourceB;
            AudioSource nextSource = isPlayingMusicA ? musicSourceB : musicSourceA;

            nextSource.clip = newMusic.clip;
            nextSource.volume = 0;
            nextSource.Play();

            float transitionDuration = 2f;
            float timer = 0f;

            while (timer < transitionDuration)
            {
                timer += Time.deltaTime;
                activeSource.volume = Mathf.Lerp(newMusic.volume, 0, timer / transitionDuration);
                nextSource.volume = Mathf.Lerp(0, newMusic.volume, timer / transitionDuration);
                yield return null;
            }

            activeSource.Stop();
            isPlayingMusicA = !isPlayingMusicA;
        }

        public void PlayEnvironmentSound(string environmentType)
        {
            if (currentEnvironment == environmentType) return;
            currentEnvironment = environmentType;

            if (!environmentalClips.TryGetValue(environmentType, out AudioClipData clipData) || clipData.clip == null) return;
            StartCoroutine(FadeEnvironmentSound(clipData));
        }

        private IEnumerator FadeEnvironmentSound(AudioClipData newEnvSound)
        {
            AudioSource activeSource = isPlayingEnvA ? environmentSourceA : environmentSourceB;
            AudioSource nextSource = isPlayingEnvA ? environmentSourceB : environmentSourceA;

            nextSource.clip = newEnvSound.clip;
            nextSource.volume = 0;
            nextSource.Play();

            float transitionDuration = 2f;
            float timer = 0f;

            while (timer < transitionDuration)
            {
                timer += Time.deltaTime;
                activeSource.volume = Mathf.Lerp(newEnvSound.volume, 0, timer / transitionDuration);
                nextSource.volume = Mathf.Lerp(0, newEnvSound.volume, timer / transitionDuration);
                yield return null;
            }

            activeSource.Stop();
            isPlayingEnvA = !isPlayingEnvA;
        }

        public void StopEnvironmentSound()
        {
            environmentSourceA.Stop();
            environmentSourceB.Stop();
        }

        public void SetReverbLevel(float level)
        {
            environmentalMixer.audioMixer.SetFloat("ReverbLevel", level);
        }

        public void SetEchoLevel(float level)
        {
            environmentalMixer.audioMixer.SetFloat("EchoLevel", level);
        }

        public void SetVolume(float volume)
        {
            musicSourceA.volume = volume;
            musicSourceB.volume = volume;
            environmentSourceA.volume = volume;
            environmentSourceB.volume = volume;
        }
    }
}
