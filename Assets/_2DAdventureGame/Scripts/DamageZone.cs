using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller =
            other.GetComponent<PlayerController>();
        if (controller != null)
        {
            // PlayerController's invincibility window makes this one damage/audio event
            // per hit, even though this callback runs every physics frame.
            controller.ChangeHealth(-damage);
        }
    }
}
