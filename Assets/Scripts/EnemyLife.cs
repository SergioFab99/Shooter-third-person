using UnityEngine;
using System.Collections;

public class EnemyLife : MonoBehaviour
{
    public int maxHits = 3;
    int currentHits = 0;
    bool isFlashing = false;
    Renderer[] rends;
    Color[] originalColors;

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++)
            originalColors[i] = rends[i].material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        HandleBulletHit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleBulletHit(collision.collider);
    }

    void HandleBulletHit(Collider other)
    {
        if (!other || !other.CompareTag("Bullet")) return;
        Destroy(other.gameObject);
        currentHits++;
        if (currentHits < maxHits)
        {
            if (!isFlashing) StartCoroutine(FlashDamage());
        }
        if (currentHits >= maxHits) Die();
    }

    IEnumerator FlashDamage()
    {
        isFlashing = true;
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = new Color(1f, 0f, 0f, 0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < rends.Length; i++)
            rends[i].material.color = originalColors[i];
        isFlashing = false;
    }

    void Die()
    {
        EnemySpawner spawner = UnityEngine.Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
            spawner.RemoveEnemyPosition(transform.position);
        Destroy(gameObject);
    }
}