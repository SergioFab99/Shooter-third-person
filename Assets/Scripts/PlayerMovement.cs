using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private float velocidadMovimiento = 5f;
    private float fuerzaSalto = 6f;
    private float distanciaChequeoSuelo = 1f;

    private float multiplicadorSprint = 3f;
    private float duracionSprint = 5f;
    private float tiempoEsperaSprint = 20f;
    private float timerSprint = 0f;
    private float timerEspera = 0f;
    private float estaminaMaxima = 5f;
    private float estaminaActual;
    private float tasaRegeneracionEstamina = 1.5f;
    private bool estaHaciendoSprint = false;

    private float distanciaDash = 8f;
    private float tiempoEsperaDash = 3f;
    private float timerEsperaDash = 0f;
    private bool puedeHacerDash = true;

    private Rigidbody rb;
    private bool estaEnSuelo;
    private Transform camara;

    public AudioClip sonidoSprint;
    public AudioClip sonidoDash;
    public AudioClip sonidoJump;
    private AudioSource fuenteAudio;

    [SerializeField] bool permitirSalto = true; // nuevo flag

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camara = Camera.main.transform;
        estaminaActual = estaminaMaxima;
        fuenteAudio = GetComponent<AudioSource>();
        if (fuenteAudio == null)
            fuenteAudio = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && estaminaActual > 0)
        {
            fuenteAudio.PlayOneShot(sonidoSprint);
        }
        if (Input.GetKey(KeyCode.LeftShift) && estaminaActual > 0)
        {
            estaHaciendoSprint = true;
            estaminaActual -= Time.deltaTime;
        }
        else
        {
            estaHaciendoSprint = false;
            if (estaminaActual < estaminaMaxima)
            {
                estaminaActual += tasaRegeneracionEstamina * Time.deltaTime;
            }
        }
        estaminaActual = Mathf.Clamp(estaminaActual, 0, estaminaMaxima);

        if (timerEsperaDash > 0f)
        {
            timerEsperaDash -= Time.deltaTime;
            if (timerEsperaDash <= 0f)
            {
                timerEsperaDash = 0f;
                puedeHacerDash = true;
            }
        }

        Vector3 origenRayo = transform.position + Vector3.up * 0.1f;
        estaEnSuelo = false;
        if (Physics.Raycast(origenRayo, Vector3.down, out RaycastHit hit, distanciaChequeoSuelo + 0.2f))
            estaEnSuelo = hit.collider.CompareTag("Ground");

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direccion = new Vector3(x, 0f, z);
        if (direccion.sqrMagnitude > 1f) direccion.Normalize();

        Vector3 adelante = camara.forward; adelante.y = 0f; adelante.Normalize();
        Vector3 derecha = camara.right; derecha.y = 0f; derecha.Normalize();

        if (Input.GetMouseButtonDown(1) && puedeHacerDash && direccion.sqrMagnitude > 0.1f)
        {
            fuenteAudio.PlayOneShot(sonidoDash);
            Vector3 direccionDash = (adelante * direccion.z + derecha * direccion.x).normalized;
            Vector3 posicionDestino = transform.position + direccionDash * distanciaDash;
            
            StartCoroutine(EjecutarDash(posicionDestino, direccionDash));
            
            puedeHacerDash = false;
            timerEsperaDash = tiempoEsperaDash;
        }

        float velocidad = estaHaciendoSprint ? velocidadMovimiento * multiplicadorSprint : velocidadMovimiento;
        Vector3 movimiento = (adelante * direccion.z + derecha * direccion.x) * velocidad;

        rb.linearVelocity = new Vector3(movimiento.x, rb.linearVelocity.y, movimiento.z);

        if (permitirSalto && Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            fuenteAudio.PlayOneShot(sonidoJump);
            if (rb.linearVelocity.y < 0f)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }

    public float ObtenerEstaminaActual()
    {
        return estaminaActual;
    }

    public float ObtenerEstaminaMaxima()
    {
        return estaminaMaxima;
    }

    public float ObtenerCooldownDash()
    {
        return timerEsperaDash;
    }

    public float ObtenerTiempoEsperaDashTotal()
    {
        return tiempoEsperaDash;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.JugadorMurio();
            CargarSiguienteEscena();
            Destroy(gameObject);
        }
    }

    void CargarSiguienteEscena()
    {
        int idx = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        if (total <= 1) return;
        int siguiente = idx + 1;
        if (siguiente >= total) siguiente = 0; // reinicia si es la última
        SceneManager.LoadScene(siguiente);
    }

    public void Morir()
    {
        CargarSiguienteEscena();
        Destroy(gameObject);
    }

    private void CrearEfectoTeleport(Vector3 posicion)
    {
        GameObject efectoTeleport = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        efectoTeleport.transform.position = posicion;
        efectoTeleport.transform.localScale = Vector3.one * 2f;
        
        Renderer renderer = efectoTeleport.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.cyan;
        }
        
        Destroy(efectoTeleport.GetComponent<Collider>());
        Destroy(efectoTeleport, 0.5f);
    }

    private System.Collections.IEnumerator EjecutarDash(Vector3 posicionDestino, Vector3 direccion)
    {
        Vector3 escalaOriginal = transform.localScale;
        Vector3 posicionOriginal = transform.position;
        
        float duracionDash = 0.1f;
        float tiempoTranscurrido = 0f;
        
        while (tiempoTranscurrido < duracionDash)
        {
            float progreso = tiempoTranscurrido / duracionDash;
            
            Vector3 escalaEstirada = new Vector3(
                escalaOriginal.x * (1f + direccion.x * 3f * (1f - progreso)),
                escalaOriginal.y,
                escalaOriginal.z * (1f + direccion.z * 3f * (1f - progreso))
            );
            
            transform.localScale = escalaEstirada;
            transform.position = Vector3.Lerp(posicionOriginal, posicionDestino, progreso);
            
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        transform.position = posicionDestino;
        transform.localScale = escalaOriginal;
    }
}