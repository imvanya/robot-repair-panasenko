using System;
using UnityEngine;

namespace Beginner2D
{
    /// <summary>
    /// Handle the projectile launched by the player to fix the robots.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        Rigidbody2D rigidbody2d;
    
        void Awake()
        {
            //DEMO purpose only. In the tutorial, this is set in the inspector on the prefab, but to keep the layer
            //settings empty, we have to do it dynamically in the demo
            Helpers.RecursiveLayerSet(transform, Helpers.ProjectileLayer);

            rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            Helpers.RecursiveLayerSet(transform, Helpers.ProjectileLayer);
        }

        void Update()
        {
            //destroy the projectile when it reach a distance of 1000.0f from the origin
            if(transform.position.magnitude > 1000.0f)
                Destroy(gameObject);
        }

        //called by the player controller after it instantiate a new projectile to launch it.
        public void Launch(Vector2 direction, float force)
        {
            rigidbody2d.AddForce(direction * force);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Fix();
            }
            Destroy(gameObject);
        }
    
        void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }

    }
}