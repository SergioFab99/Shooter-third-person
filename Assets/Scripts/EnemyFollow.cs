using UnityEngine;
using UnityEngine.SceneManagement;

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

        Vector3 pos = transform.position;
        float nuevoZ = Mathf.MoveTowards(pos.z, jugador.position.z, velocidadMovimiento * Time.deltaTime);
        transform.position = new Vector3(pos.x, pos.y, nuevoZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("EditorOnly"))
        {
            CargarSiguienteEscena();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("EditorOnly"))
        {
            CargarSiguienteEscena();
        }
    }

    void CargarSiguienteEscena()
    {
        int idx = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        if (total <= 1) return;
        int siguiente = idx + 1;
        if (siguiente >= total) siguiente = 0;
        SceneManager.LoadScene(siguiente);
    }
}