using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Solivagant.Audio
{
    public class FootstepAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup footstepMixer;
        public static FootstepAudioHandler Instance { get; private set; }

        private Dictionary<string, FootstepAudioData> footstepData;
        private AudioSource footstepSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            footstepData = new Dictionary<string, FootstepAudioData>();

            foreach (var data in Resources.LoadAll<FootstepAudioData>("Audio/Footsteps"))
                footstepData[data.surfaceType] = data;

            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.spatialBlend = 1f; 
        }

        public void PlayFootstep(string surfaceType, Vector3 position)
        {
            if (!footstepData.TryGetValue(surfaceType, out FootstepAudioData data) || data.footstepSounds.Length == 0)
                return;

            AudioClip clip = data.footstepSounds[Random.Range(0, data.footstepSounds.Length)];

            footstepSource.transform.position = position;
            footstepSource.clip = clip;
            footstepSource.volume = data.volume;
            footstepSource.Play();
        }
    }
}
