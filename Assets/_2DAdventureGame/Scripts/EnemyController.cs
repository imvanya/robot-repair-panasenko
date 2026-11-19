using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    public float speed = 1.0f;
    public bool vertical;

    public float changeTime = 3.0f;
    private float timer;
    private int direction = 1;
    public float damageCooldown = 1.0f;
    private float nextDamageTime = 0f;

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    bool broken = true;

    public AudioClip fixedClip;      
    private AudioSource audioSource; 

    public bool isBroken { get { return broken; } }
    public event System.Action OnFixed;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.enabled = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        timer = changeTime;
    }

    void Update()
    {
        if (!broken)
        {
            return;
        }

        EnsureWalkingAudio();

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    private void EnsureWalkingAudio()
    {
        if (audioSource == null || AudioManager.Instance == null
            || AudioManager.Instance.enemyWalk == null)
            return;

        AudioClip walkClip = AudioManager.Instance.enemyWalk;
        if (audioSource.clip != walkClip)
        {
            audioSource.Stop();
            audioSource.clip = walkClip;
        }

        audioSource.volume = AudioManager.Instance.enemyWalkVolume;
        audioSource.loop = true;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2d.MovePosition(position);
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (!broken || Time.time < nextDamageTime)
        return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
        player.ChangeHealth(-1);
        nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void Fix()
    {
        if (!broken)
            return;

        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
        audioSource.Stop();
        audioSource.loop = false;

        if (AudioManager.Instance != null && audioSource != null
            && AudioManager.Instance.enemyFix != null)
        {
            audioSource.PlayOneShot(AudioManager.Instance.enemyFix,
                AudioManager.Instance.enemyFixVolume);
        }
        else if (audioSource != null && fixedClip != null)
        {
            audioSource.PlayOneShot(fixedClip);
        }

        OnFixed?.Invoke();
    }
}
