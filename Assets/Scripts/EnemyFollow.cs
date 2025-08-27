using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    private Transform jugador;
    public float velocidadMovimiento = 5f;

    void Start()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
            jugador = objetoJugador.transform;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        if (jugador == null)
        {
            Debug.LogWarning("Player no asignado al enemigo.");
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            jugador.position,
            velocidadMovimiento * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}