using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Clip Data", menuName = "Audio Data/AudioClipData")]
public class AudioClipData : ScriptableObject
{
    public string clipName;
    public AudioClip clip;
    public bool is3D;
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
}
