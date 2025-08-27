using UnityEngine;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{
    public GameObject prefabBala;
    public float fuerzaBala = 20f;

    public int maximoBalas = 10;

    public AudioClip sonidoDisparo;

    private AudioSource fuenteAudio;
    private List<GameObject> balasActivas = new List<GameObject>();

    public Transform transformCamara;

    void Start()
    {
        fuenteAudio = GetComponent<AudioSource>();
        if (fuenteAudio == null)
            fuenteAudio = gameObject.AddComponent<AudioSource>();

        if (transformCamara == null)
            transformCamara = Camera.main ? Camera.main.transform : transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Disparar();
        }
    }

    void Disparar()
    {
        Vector3 direccionDisparo = transformCamara.forward;
        Vector3 horizontalDir = new Vector3(direccionDisparo.x, 0f, direccionDisparo.z);
        if (horizontalDir.sqrMagnitude < 0.0001f)
        {
            horizontalDir = new Vector3(transform.forward.x, 0f, transform.forward.z);
            if (horizontalDir.sqrMagnitude < 0.0001f) horizontalDir = Vector3.forward;
        }
        horizontalDir.Normalize();

        GameObject bala = Instantiate(prefabBala, transform.position + horizontalDir * 1.5f, Quaternion.LookRotation(horizontalDir));

        bala.transform.Rotate(Vector3.right, 90f, Space.World);

        Rigidbody rb = bala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = horizontalDir * fuerzaBala;
        }

        Destroy(bala, 3f);

        balasActivas.RemoveAll(b => b == null);

        balasActivas.Add(bala);
        if (balasActivas.Count > maximoBalas)
        {
            Destroy(balasActivas[0]);
            balasActivas.RemoveAt(0);
        }

        if (sonidoDisparo != null && fuenteAudio != null)
        {
            fuenteAudio.PlayOneShot(sonidoDisparo);
        }
    }
}
