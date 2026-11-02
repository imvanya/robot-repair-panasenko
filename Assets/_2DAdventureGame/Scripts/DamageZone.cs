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
            controller.ChangeHealth(-damage);
        }
    }
}