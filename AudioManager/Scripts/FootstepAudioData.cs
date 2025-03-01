using UnityEngine;

[CreateAssetMenu(fileName = "New Footstep Data", menuName = "Audio Data/Footstep Audio Data")]
public class FootstepAudioData : ScriptableObject
{
    public string surfaceType;
    public AudioClip[] footstepSounds;
    public float volume = 1f;
}
