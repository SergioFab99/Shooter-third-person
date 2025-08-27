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
    [SerializeField] bool mostrarGizmo = true;
    [SerializeField] Color colorGizmo = new Color(0f, 1f, 0f, 0.15f);

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
        if (enemigosActuales >= maxEnemigosEnEscena) return;

        Vector3 posicionGenerar = Vector3.zero;
        bool posicionValidaEncontrada = false;
        Vector3 centro = transform.position; // ahora el centro de spawn es el spawner

        for (int intentos = 0; intentos < intentosMaximos; intentos++)
        {
            Vector2 circuloAleatorio = Random.insideUnitCircle * radioAparicion;
            Vector3 posicionCandidato = new Vector3(
                centro.x + circuloAleatorio.x,
                centro.y + 10f,
                centro.z + circuloAleatorio.y
            );

            if (Physics.Raycast(posicionCandidato, Vector3.down, out RaycastHit hit, 30f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    posicionCandidato = hit.point + Vector3.up * 0.1f;

                    if (jugador != null)
                    {
                        float distanciaAlJugador = Vector3.Distance(posicionCandidato, jugador.position);
                        if (distanciaAlJugador < distanciaMinimaDelJugador)
                            continue;
                    }

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

        if (!posicionValidaEncontrada) return;

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

    public void RemoveEnemyPosition(Vector3 pos)
    {
        if (posicionesEnemigosActivos.Count == 0) return;
        int indexToRemove = -1;
        float bestDist = float.MaxValue;
        for (int i = 0; i < posicionesEnemigosActivos.Count; i++)
        {
            float d = Vector3.Distance(pos, posicionesEnemigosActivos[i]);
            if (d < bestDist)
            {
                bestDist = d;
                indexToRemove = i;
            }
        }
        if (indexToRemove != -1 && bestDist <= 2f)
            posicionesEnemigosActivos.RemoveAt(indexToRemove);
    }

    void OnDrawGizmos()
    {
        if (!mostrarGizmo) return;
        Vector3 pos = transform.position; // siempre el spawner
        Color borde = new Color(colorGizmo.r, colorGizmo.g, colorGizmo.b, Mathf.Clamp01(colorGizmo.a + 0.3f));
        Gizmos.color = colorGizmo;
        Gizmos.DrawSphere(pos, radioAparicion * 0.02f);
        Gizmos.color = borde;
        Gizmos.DrawWireSphere(pos, radioAparicion);
        if (jugador != null)
        {
            // Línea opcional hacia el jugador para referencia visual
            Gizmos.DrawLine(pos, jugador.position);
        }
    }
}