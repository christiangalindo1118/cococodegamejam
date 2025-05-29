using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Settings")]
    [SerializeField] private bool showText = true;
    [SerializeField] private bool smoothChanges = true;
    [SerializeField] private float changeSpeed = 5f;

    private void Start()
    {
        // Configurar el slider para trabajar con porcentajes (0-1)
        healthSlider.minValue = 0;
        healthSlider.maxValue = 1;
        healthSlider.value = playerHealth.HealthPercent; // Valor inicial
        
        // Suscribirse al evento de cambio de salud CORRECTAMENTE
        playerHealth.OnHealthChanged.AddListener(OnHealthChangedHandler);
        
        UpdateHealthText();
    }

    // Nuevo método para manejar el evento con parámetro int
    private void OnHealthChangedHandler(int newHealth)
    {
        // No necesitamos el parámetro newHealth porque usaremos HealthPercent
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (!playerHealth) return;
        
        float targetPercent = playerHealth.HealthPercent;
        
        if (smoothChanges)
        {
            StartCoroutine(SmoothHealthChange(targetPercent));
        }
        else
        {
            healthSlider.value = targetPercent;
        }
        
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (!showText || !healthText || !playerHealth) return;
        
        healthText.text = $"{Mathf.RoundToInt(playerHealth.HealthPercent * 100)}%";
    }

    private System.Collections.IEnumerator SmoothHealthChange(float targetPercent)
    {
        float startPercent = healthSlider.value;
        float elapsed = 0f;
        
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * changeSpeed;
            healthSlider.value = Mathf.Lerp(startPercent, targetPercent, elapsed);
            yield return null;
        }
        
        healthSlider.value = targetPercent;
    }

    private void OnDestroy()
    {
        // Importante: Desuscribirse al destruir
        if (playerHealth)
        {
            playerHealth.OnHealthChanged.RemoveListener(OnHealthChangedHandler);
        }
    }
}