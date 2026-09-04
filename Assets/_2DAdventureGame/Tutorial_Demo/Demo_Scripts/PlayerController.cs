using System;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Beginner2D
{
    /// <summary>
    /// Main class handling the control of the player, its movements and actions like talking and throwing projectiles
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // Variables related to player character movement
        public InputAction MoveAction;
        Rigidbody2D rigidbody2d;
        Vector2 move;
        public float speed = 3.0f;

        public InputAction LaunchAction;
        // Variables related to the health system
        public int maxHealth = 5;
        int currentHealth;

        public int health { get { return currentHealth; } }
        // Variables related to temporary invincibility
        public float timeInvincible = 2.0f;
        bool isInvincible;
        float damageCooldown;


        // Variables related to Animation
        Animator animator;
        Vector2 moveDirection = new Vector2(1, 0);

        // Variables related to Projectile
        public GameObject projectilePrefab;

        // Variables related to NPC interaction
        public InputAction TalkAction;

        // Varianles related to Audio
        AudioSource audioSource;
        public AudioClip playerHit;
        public AudioClip projectileThrow;

        //Variables related to Particle Effects
        public ParticleSystem hitParticleEffect;
        public event Action OnTalkedToNPC;

        private NonPlayerCharacter lastNonPlayerCharacter;

        private void Awake()
        {
            //DEMO purpose only, in the tutorial, this will be set in the inspector on the player prefab.
            //but to keep the layer settings empty, we have to do it dynamically through code
            Helpers.RecursiveLayerSet(transform, Helpers.PlayerLayer);
        }

        private void OnEnable()
        {
            Helpers.RecursiveLayerSet(transform, Helpers.PlayerLayer);
        }

        // Start is called before the first frame update
        void Start()
        {
            MoveAction.Enable();
            TalkAction.Enable();
            LaunchAction.Enable();
            rigidbody2d = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }


        // Update is called once per frame
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


            if (LaunchAction.WasPerformedThisFrame())
            {
                Launch();
            }

            //DEMO Purpose, we built the layer mask here, where in the tutorial, LayerMask is used
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, 1<<Helpers.NPCLayer);
            if (hit.collider != null)
            {
                
                NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
                if (npc != null)
                {
                    //if the raycast hit an NPC, we activate its dialogue bubble and store it in the class, so we can
                    //disable the bubble when it's not hit by a raycast anymore
                    npc.dialogueBubble.SetActive(true);
                    lastNonPlayerCharacter = npc;
                }
                else
                {
                    //what we hit don't have a non player character script, check if we had one last frame
                    //if yes, we hide the dialog bubble from that last one as it wasn't hit this time
                    if (lastNonPlayerCharacter != null)
                    {
                        lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                        lastNonPlayerCharacter = null;
                    }
                }

                //This will check if the player pressed the dialog button, and show the dialog if they did
                FindFriend();
            }
            else
            {
                //we didn't hit anything this frame, if we had a npc last frame, we now need to hide its
                //dialog bubble as it's not in front of us anymore
                if (lastNonPlayerCharacter != null)
                {
                    lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                    lastNonPlayerCharacter = null;
                }
            }
        }




        // FixedUpdate has the same call rate as the physics system 
        void FixedUpdate()
        {
            Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
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
                PlaySound(playerHit);
                Instantiate(hitParticleEffect, transform.position + Vector3.up * 0.6f, Quaternion.identity);
                isInvincible = true;
                damageCooldown = timeInvincible;
                animator.SetTrigger("Hit");
            }
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }


        void Launch()
        {
            PlaySound(projectileThrow);
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(moveDirection, 300);
            animator.SetTrigger("Launch");
        }


        void FindFriend()
        {
            if (TalkAction.WasPressedThisFrame())
            {
                OnTalkedToNPC?.Invoke();
            }

        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

    }
}