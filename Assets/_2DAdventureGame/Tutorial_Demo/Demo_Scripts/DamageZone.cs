using UnityEngine;

namespace Beginner2D
{
    /// <summary>
    /// This class will apply continuous damage to the Player as long as it stay inside the trigger on the same object
    /// </summary>
    public class DamageZone : MonoBehaviour 
    {
        void OnTriggerStay2D(Collider2D other)
        {
            PlayerController controller = other.GetComponent<PlayerController>();

            if (controller != null)
            {
                //the controller will take care of ignoring the damage during the invincibility time.
                controller.ChangeHealth(-1);
            }
        }
    }
}