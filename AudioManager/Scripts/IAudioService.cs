public interface IAudioService
{
    void PlaySound(string soundName);
    void PlayMusic(string musicName);
    void StopMusic();
    void SetVolume(float volume);
}
