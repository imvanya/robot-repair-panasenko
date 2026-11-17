using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    // Налаштування руху
    public float speed = 1.0f;
    public bool vertical;

    // Налаштування таймера патрулювання
    public float changeTime = 3.0f;
    private float timer;
    private int direction = 1;

    // Компоненти
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    bool broken = true;

    // Звуки
    public AudioClip fixedClip;      // Звук при ремонті робота
    private AudioSource audioSource; // Компонент для відтворення звуків на самому ворозі

    // НОВЕ (крок 17): GameManager питає, чи ворог ще зламаний
    public bool isBroken { get { return broken; } }
    public event System.Action OnFixed;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Отримуємо AudioSource з об'єкта ворога
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.enabled = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        timer = changeTime;
    }

    void Update()
    {
        // Якщо ворог полагодився, він більше не рухається і не чіпає таймер
        if (!broken)
        {
            return;
        }

        EnsureWalkingAudio();

        // Відлік таймера патрулювання
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

        // Вибір осі руху (вертикально чи горизонтально)
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

    // Animation events can arrive from blended directional clips. They only
    // ensure one loop is active; they never replay or stack the long clip.
    public void PlayFootstep()
    {
        // Intentionally empty. The clip contains the full walking sequence.
    }

    // Нанесення шкоди при першому дотику
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    // Метод ремонту робота
    public void Fix()
    {
        if (!broken)
            return;

        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
        audioSource.Stop();
        audioSource.loop = false;

        // Відтворюємо звук лагодження, якщо він призначений і є AudioSource
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
