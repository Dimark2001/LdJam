using UnityEngine;

public class JumpPads : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MovableObject movableObject))
        {
            Debug.Log("JumpPads - AddForce");
            movableObject.AddForce();
            AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.Jump));
        }

        if (other.TryGetComponent(out CatController catController))
        {
            Debug.Log("JumpPads - AddForce");
            catController.AddForce();
            AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.Jump));
        }
    }
}