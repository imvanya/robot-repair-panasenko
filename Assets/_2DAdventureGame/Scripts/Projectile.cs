using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    
    [Header("Audio")]
    public AudioClip hitClip; 
    private bool hasImpacted;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleImpact(other.GetComponentInParent<EnemyController>());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact(collision.collider.GetComponentInParent<EnemyController>());
    }

    private void HandleImpact(EnemyController enemy)
    {
        if (hasImpacted)
            return;

        hasImpacted = true;
        if (enemy != null)
        {
            enemy.Fix();
        }
        else
        {
            PlayHitSound();
        }

        Destroy(gameObject);
    }

    void PlayHitSound()
    {
        if (hitClip != null)
        {
            AudioSource.PlayClipAtPoint(hitClip, transform.position);
        }
    }
}
