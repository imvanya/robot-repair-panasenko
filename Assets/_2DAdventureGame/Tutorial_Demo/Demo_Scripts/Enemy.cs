using System; //---- MTTT counter
using UnityEngine;

namespace Beginner2D
{
	/// <summary>
	/// This class handle Enemy behaviour. It make them walk back & forth as long as they aren't fixed, and then just idle
	/// without being able to interact with the player anymore once fixed.
	/// </summary>
	public class Enemy : MonoBehaviour
	{
		// ====== ENEMY MOVEMENT ========
		public float speed;
	
		public bool vertical;

		public float changeTime = 3.0f;
		Rigidbody2D rigidbody2d;
		float timer;
		int direction = 1;
		bool broken = true;
		// ===== ANIMATION ========
		Animator animator;

		// ====== AUDIO ========
		AudioSource audioSource;
		public AudioClip fixedSound;
	
		// ====== PARTICLE EFFECTS ========
		public ParticleSystem smokeParticleEffect;
		public ParticleSystem fixedParticleEffect;

		// ====== BREAKING AND FIXING =======
		public bool isBroken { get { return broken; }}
		public event Action OnFixed;

		private void Awake()
		{
			//DEMO purpose only. In the tutorial, this is set in the inspector on the prefab, but to keep the layer
			//settings empty, we have to do it dynamically in the demo
			Helpers.RecursiveLayerSet(transform, Helpers.EnemyLayer);
		}

		void Start ()
		{
			rigidbody2d = GetComponent<Rigidbody2D>();
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();
			timer = changeTime;
		}
	
		void Update()
		{
			timer -= Time.deltaTime;


			if (timer < 0)
			{
				direction = -direction;
				timer = changeTime;
			}
		}

		void FixedUpdate()
		{
			if(!broken)
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

			// Audio
			audioSource.Stop();
			audioSource.PlayOneShot(fixedSound);

			//Particles
			smokeParticleEffect.Stop();
			Instantiate(fixedParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);

			OnFixed?.Invoke();
		}
	}
}