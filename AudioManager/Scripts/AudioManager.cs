using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Solivagant.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup soundEffectMixer;

        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    instance = obj.AddComponent<AudioManager>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }

        private Dictionary<string, AudioClipData> audioClips;
        private AudioSource sfxSource;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            audioClips = new Dictionary<string, AudioClipData>();

            AudioClipData[] clips = Resources.LoadAll<AudioClipData>("SoundEffects");
            foreach (var clip in clips)
            {
                audioClips[clip.clipName] = clip;
            }

            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlaySound(string soundName, Vector3 position = default)
        {
            if (!audioClips.TryGetValue(soundName, out AudioClipData clipData) || clipData.clip == null) return;

            if (clipData.is3D)
            {
                AudioSource.PlayClipAtPoint(clipData.clip, position, clipData.volume);
            }
            else
            {
                sfxSource.pitch = clipData.pitch;
                sfxSource.volume = clipData.volume;
                sfxSource.PlayOneShot(clipData.clip);
            }
        }

        public void SetVolume(float volume)
        {
            sfxSource.volume = volume;
        }
    }
}
