using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------Audio Source -------")]
    [SerializeField] AudioSource SFXSource;

    [Header("-------Audio Clip -------")]
    public AudioClip Placing;
    public AudioClip Gather;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
