using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sound
{
    [SerializeField]
    private AudioClip[] clips;

    public AudioClip Clip
    {
        get
        {
            if (clips != null && clips.Length != 0) return clips[Random.Range(0, clips.Length)];
          // Debug.Log("Clips list empty");
            return null;
        }
    }
}
