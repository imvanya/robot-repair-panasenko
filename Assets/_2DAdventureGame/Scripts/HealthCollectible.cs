using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    public int amount = 1;
    private bool collected;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (!collected && controller != null && controller.health < controller.maxHealth)
        {
            collected = true;
            controller.ChangeHealth(amount);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.collectible,
                    AudioManager.Instance.collectibleVolume);
            }
            else if (collectedClip != null)
            {
                controller.PlaySound(collectedClip);
            }
            
            Destroy(gameObject);
        }
    }
}
