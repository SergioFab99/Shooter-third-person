using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefabEnemigo;
    public float radioAparicion = 15f;
    public float intervaloGeneracion = 2f;
    public float distanciaMinima = 3f;
    public float distanciaMinimaDelJugador = 8f;
    public int intentosMaximos = 15;
    public int maxEnemigosEnEscena = 50;

    private System.Collections.Generic.List<Vector3> posicionesEnemigosActivos = new System.Collections.Generic.List<Vector3>();
    private Transform jugador;
    private int enemigosActuales = 0;

    void Start()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
            jugador = objetoJugador.transform;

        InvokeRepeating(nameof(GenerarEnemigo), 0f, intervaloGeneracion);
        InvokeRepeating(nameof(LimpiarPosicionesMuertas), 5f, 5f);
    }

    void GenerarEnemigo()
    {
        enemigosActuales = FindObjectsByType<EnemyFollow>(FindObjectsSortMode.None).Length;
        if (jugador == null || enemigosActuales >= maxEnemigosEnEscena) return;

        Vector3 posicionGenerar = Vector3.zero;
        bool posicionValidaEncontrada = false;

        for (int intentos = 0; intentos < intentosMaximos; intentos++)
        {
            Vector2 circuloAleatorio = Random.insideUnitCircle * radioAparicion;
            Vector3 posicionCandidato = new Vector3(
                jugador.position.x + circuloAleatorio.x,
                jugador.position.y + 10f,
                jugador.position.z + circuloAleatorio.y
            );

            if (Physics.Raycast(posicionCandidato, Vector3.down, out RaycastHit hit, 20f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    posicionCandidato = hit.point + Vector3.up * 0.1f;
                    
                    float distanciaAlJugador = Vector3.Distance(posicionCandidato, jugador.position);
                    if (distanciaAlJugador < distanciaMinimaDelJugador)
                        continue;

                    bool muyCerca = false;
                    foreach (Vector3 posicionEnemigo in posicionesEnemigosActivos)
                    {
                        if (Vector3.Distance(posicionCandidato, posicionEnemigo) < distanciaMinima)
                        {
                            muyCerca = true;
                            break;
                        }
                    }

                    if (!muyCerca)
                    {
                        posicionGenerar = posicionCandidato;
                        posicionValidaEncontrada = true;
                        break;
                    }
                }
            }
        }

        if (!posicionValidaEncontrada)
        {
            return;
        }

        if (prefabEnemigo != null)
        {
            GameObject nuevoEnemigo = Instantiate(prefabEnemigo, posicionGenerar, Quaternion.identity);
            posicionesEnemigosActivos.Add(posicionGenerar);
        }
        else
        {
            Debug.LogError("EnemyPrefab no asignado en EnemySpawner.");
        }
    }

    void LimpiarPosicionesMuertas()
    {
        posicionesEnemigosActivos.RemoveAll(pos => 
        {
            GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemigo in enemigos)
            {
                if (Vector3.Distance(enemigo.transform.position, pos) < 1f)
                    return false;
            }
            return true;
        });
    }
}