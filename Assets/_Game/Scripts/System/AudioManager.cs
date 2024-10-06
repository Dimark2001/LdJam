using UnityEngine;

public static class AudioManager
{
    private static AudioSource _audioSource;
    public static void PlayAudioClip(AudioClip clip, bool isLoop = false)
    {
        if (Camera.main != null && _audioSource == null)
        {
            _audioSource = Camera.main.transform.GetComponent<AudioSource>();
        }
        
        _audioSource.clip = clip;
        _audioSource.loop = isLoop;
        _audioSource.Play();
    }

    
    public static void StopAudio()
    {
        _audioSource?.Stop();
    }
    
    public static void ResumeAudio()
    {
        AudioListener.pause = false;
    }

    public static void PauseAudio()
    {
        AudioListener.pause = true;
    }
}

public static class LoadFromResource
{
    public static readonly string SelectOther = "Audio/selectOther";
    public static readonly string SelectMany = "Audio/selectMany";
    public static readonly string CatDead = "Audio/catDead";
    public static readonly string Grab = "Audio/grab";
    public static readonly string Jump = "Audio/jump";
    public static readonly string Collect = "Audio/collect";
    
    public static AudioClip LoadAudioClip(string path)
    {
        return Resources.Load<AudioClip>(path);
    }
}