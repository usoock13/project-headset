using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource effectAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;

    [SerializeField] public AudioClip skillAudioClip;
    
    public float VolumeEffect {
        get => Mathf.Clamp(effectAudioSource.volume, 0, 1);
        set => effectAudioSource.volume = Mathf.Clamp(value, 0, 1);
    }

    public float VolumeBGM {
        get => Mathf.Clamp(bgmAudioSource.volume, 0, 1);
        set => bgmAudioSource.volume = Mathf.Clamp(value, 0, 1);
    }

    public void PlayEffect(AudioClip clip) {
        effectAudioSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip) {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }
}