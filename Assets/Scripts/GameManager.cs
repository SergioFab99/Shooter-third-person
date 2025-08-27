using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoEnemigos;
    public Slider sliderEstamina;
    public Slider sliderDash;

    private float tiempoNivel = 120f;
    private float timer;
    private bool nivelTerminado = false;
    private bool jugadorMuerto = false;

    private PlayerMovement playerMovement;
    private EnemySpawner enemySpawner;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InicializarManager();
    }

    void Start()
    {
        InicializarManager();
    }

    private void InicializarManager()
    {
        timer = tiempoNivel;
        nivelTerminado = false;
        jugadorMuerto = false;
        
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();

        if (GameObject.Find("TextoTiempo") != null)
            textoTiempo = GameObject.Find("TextoTiempo").GetComponent<TextMeshProUGUI>();
        
        if (GameObject.Find("TextoEnemigos") != null)
            textoEnemigos = GameObject.Find("TextoEnemigos").GetComponent<TextMeshProUGUI>();

        if (GameObject.Find("SliderEstamina") != null)
            sliderEstamina = GameObject.Find("SliderEstamina").GetComponent<Slider>();
        
        if (GameObject.Find("SliderDash") != null)
            sliderDash = GameObject.Find("SliderDash").GetComponent<Slider>();
    }

    void Update()
    {
        ActualizarUI();

        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (nivelTerminado || jugadorMuerto) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            NivelCompletado();
        }
    }

    private void ActualizarUI()
    {
        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.Ceil(timer).ToString("F0");

        if (textoEnemigos != null && enemySpawner != null)
        {
            int enemigosRestantes = FindObjectsByType<EnemyFollow>(FindObjectsSortMode.None).Length;
            textoEnemigos.text = "Enemigos: " + enemigosRestantes + " / " + enemySpawner.maxEnemigosEnEscena;
        }

        if (playerMovement != null)
        {
            if (sliderEstamina != null)
            {
                sliderEstamina.maxValue = playerMovement.ObtenerEstaminaMaxima();
                sliderEstamina.value = playerMovement.ObtenerEstaminaActual();
            }
            if (sliderDash != null)
            {
                sliderDash.maxValue = playerMovement.ObtenerTiempoEsperaDashTotal();
                sliderDash.value = playerMovement.ObtenerCooldownDash();
            }
        }
    }

    public void JugadorMurio()
    {
        if (jugadorMuerto) return;
        jugadorMuerto = true;
        
        AudioSource musica = FindFirstObjectByType<AudioSource>();
        if (musica != null)
            musica.Stop();
            
        Debug.Log("¡El jugador ha muerto! Game Over.");
    }

    public void NivelCompletado()
    {
        if (nivelTerminado) return;
        nivelTerminado = true;
        Debug.Log("¡Nivel completado! Siguiente nivel...");
        int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;
        if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(siguienteEscena);
        else
            Debug.Log("¡Has completado todos los niveles!");
    }

    public float ObtenerTiempoRestante()
    {
        return timer;
    }
}
