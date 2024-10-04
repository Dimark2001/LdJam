using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
   public void Mute()
   {
       AudioListener.volume = 0;
   }
}