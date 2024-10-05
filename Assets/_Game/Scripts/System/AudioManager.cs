using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public void PlayAudio()
    {
        AudioListener.pause = false;
    }

    public void PauseAudio()
    {
        AudioListener.pause = true;
    }
}