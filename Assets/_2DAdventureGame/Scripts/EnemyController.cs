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
    private Animator animator; // НОВЕ: поле для аніматора
    bool broken = true;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // НОВЕ: отримуємо Animator
        timer = changeTime; // Ініціалізація таймера
    }

    void Update()
    {
        // Відлік таймера патрулювання
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction; // Зміна напрямку на протилежний
            timer = changeTime;     // Скидання таймера
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
            animator.SetFloat("Move X", 0);         // НОВЕ
            animator.SetFloat("Move Y", direction); // НОВЕ
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction); // НОВЕ
            animator.SetFloat("Move Y", 0);         // НОВЕ
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
