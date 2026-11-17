using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public float speed = 3.0f;

    public float fireCooldown = 0.5f; // Час затримки в секундах
    private float nextFireTime = 0f;  // Змінна для відстеження часу наступного пострілу
    
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth; 
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public InputAction LaunchAction;

    public InputAction TalkAction;

    private NonPlayerCharacter lastNonPlayerCharacter;

    // Звуки гравця
    public AudioClip hitSound;      // Звук отримання шкоди
    public AudioClip launchSound;   // Звук запуску шестерінки (НОВЕ)
    AudioSource audioSource;
    private float nextFootstepTime;

    void Start()
    {   
        LaunchAction.Enable();
        animator = GetComponent<Animator>();
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        TalkAction.Enable();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
        
        if (LaunchAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            Launch();
        }
        
        RaycastHit2D hit = Physics2D.Raycast(
            rigidbody2d.position + Vector2.up * 0.2f,
            moveDirection, 1.5f, LayerMask.GetMask("NPC"));
            
        if (hit.collider != null)
        {
            NonPlayerCharacter npc =
                hit.collider.GetComponent<NonPlayerCharacter>();
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend();
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
            }
        }
    }   

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position
            + move * speed * Time.deltaTime; 
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(hitSound, AudioManager.Instance != null
                ? AudioManager.Instance.playerHitVolume
                : 1f);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip, volume);
        }
    }

    // Called by footstep events on the four run animation clips.
    public void PlayFootstep()
    {
        // Ignore an event that lands during the run-to-idle transition.
        if (move.sqrMagnitude <= 0.01f || AudioManager.Instance == null || audioSource == null)
            return;

        // Blend trees can emit the same event from two directional clips at once.
        if (Time.time < nextFootstepTime)
            return;
        nextFootstepTime = Time.time + 0.15f;

        AudioClip footstep = AudioManager.Instance.playerWalk;
        if (footstep != null)
            audioSource.PlayOneShot(footstep, AudioManager.Instance.playerWalkVolume);
    }
    
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f,
            Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);

        animator.SetTrigger("Launch");
        
        // Відтворюємо звук кидка шестерінки
        PlaySound(AudioManager.Instance != null ? AudioManager.Instance.projectile : launchSound,
            AudioManager.Instance != null ? AudioManager.Instance.projectileVolume : 1f);
    }

    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue();
        }
    }
}
