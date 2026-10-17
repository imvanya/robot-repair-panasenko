using UnityEngine;

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

    // НОВЕ (крок 17): GameManager питає, чи ворог ще зламаний
    public bool isBroken { get { return broken; } }

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    void Update()
    {
        // Відлік таймера патрулювання
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
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

    // Нанесення шкоди при першому дотику
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
    }
}