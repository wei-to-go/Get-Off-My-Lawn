using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    public float splashRadius = 5f;
    public int damage = 50;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    bool hasExploded = false;

    void Start()
    {
        Invoke("Explode", 4f);
    }

    void OnTriggerEnter(Collider other)
    {
        // ignore the player
        if (other.CompareTag("Player")) return;
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // stop all movement immediately
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (explosionSound)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, splashRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                SunDwellerBehavior enemy = hit.GetComponent<SunDwellerBehavior>();
                if (enemy) enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}