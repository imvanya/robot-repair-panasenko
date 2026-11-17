using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorAudio : MonoBehaviour
{
    public AudioClip openClip;
    public AudioClip closeClip;
    [Range(0f, 1f)] public float volume = 0.8f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlayOpen()
    {
        Play(openClip);
    }

    public void PlayClose()
    {
        Play(closeClip);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip, volume);
    }
}
