using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIRegistrar : MonoBehaviour
{
    void Start()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.textoTiempo = GetComponentInChildren<TextMeshProUGUI>(true);
            gm.textoEnemigos = transform.Find("TextoEnemigos")?.GetComponent<TextMeshProUGUI>();
            gm.sliderEstamina = GetComponentInChildren<Slider>(true);
            gm.sliderDash = transform.Find("SliderDash")?.GetComponent<Slider>();

            if (gm.textoTiempo == null)
                gm.textoTiempo = GameObject.Find("TextoTiempo")?.GetComponent<TextMeshProUGUI>();
            if (gm.textoEnemigos == null)
                gm.textoEnemigos = GameObject.Find("TextoEnemigos")?.GetComponent<TextMeshProUGUI>();
            if (gm.sliderEstamina == null)
                gm.sliderEstamina = GameObject.Find("SliderEstamina")?.GetComponent<Slider>();
            if (gm.sliderDash == null)
                gm.sliderDash = GameObject.Find("SliderDash")?.GetComponent<Slider>();
        }
        else
        {
            Debug.LogError("No se pudo encontrar la instancia de GameManager.");
        }
    }
}
