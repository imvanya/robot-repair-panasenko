using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TimedSwitch : MonoBehaviour
{
    public GameObject door;
    public float openTime = 4.0f;
    public float frameDuration = 0.08f;
    public Sprite[] doorAnimationFrames;
    public Sprite switchOffSprite;
    public Sprite switchOnSprite;

    [Header("Звуки кнопки")]
    public AudioClip buttonPressClip;
    public AudioClip buttonReleaseClip;
    [Range(0f, 1f)] public float interactionVolume = 0.8f;

    private SpriteRenderer switchRenderer;
    private AudioSource audioSource;
    private bool isPressed;

    void Awake()
    {
        switchRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null || door == null || isPressed)
            return;

        isPressed = true;
        SetSwitchState(true);
        PlaySound(buttonPressClip);
        StartCoroutine(OpenDoorFor(openTime));
    }

    IEnumerator OpenDoorFor(float seconds)
    {
        yield return new WaitForSeconds(0.1f);
        DoorAudio doorAudio = door.GetComponent<DoorAudio>();
        if (doorAudio != null)
            doorAudio.PlayOpen();

        SpriteRenderer doorRenderer = door.GetComponent<SpriteRenderer>();
        Collider2D doorCollider = door.GetComponent<Collider2D>();

        if (doorRenderer == null || doorAnimationFrames == null || doorAnimationFrames.Length == 0)
        {
            door.SetActive(false);
            yield return new WaitForSeconds(seconds);
            if (doorAudio != null)
                doorAudio.PlayClose();
            door.SetActive(true);
            PlaySound(buttonReleaseClip);
            SetSwitchState(false);
            isPressed = false;
            yield break;
        }

        door.SetActive(true);
        int lastFrame = doorAnimationFrames.Length - 1;
        int frame = System.Array.IndexOf(doorAnimationFrames, doorRenderer.sprite);
        if (frame < 0)
        {
            frame = 0;
            doorRenderer.sprite = doorAnimationFrames[frame];
        }

        while (frame < lastFrame)
        {
            frame++;
            doorRenderer.sprite = doorAnimationFrames[frame];
            yield return new WaitForSeconds(frameDuration);
        }

        if (doorCollider != null)
            doorCollider.enabled = false;

        yield return new WaitForSeconds(seconds);
        if (doorAudio != null)
            doorAudio.PlayClose();

        while (frame > 0)
        {
            frame--;
            doorRenderer.sprite = doorAnimationFrames[frame];
            yield return new WaitForSeconds(frameDuration);
        }

        if (doorCollider != null)
            doorCollider.enabled = true;

        PlaySound(buttonReleaseClip);
        SetSwitchState(false);
        isPressed = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip, interactionVolume);
    }

    void SetSwitchState(bool isOn)
    {
        Sprite stateSprite = isOn ? switchOnSprite : switchOffSprite;
        if (switchRenderer != null && stateSprite != null)
            switchRenderer.sprite = stateSprite;
    }
}
