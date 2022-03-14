using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public float masterVolPercent = 1f, sfxVolPercent = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, masterVolPercent * sfxVolPercent);
        }
    }
}
