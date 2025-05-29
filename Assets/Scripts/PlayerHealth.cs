using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool invulnerableOnDamage = false;
    [SerializeField] private float invulnerabilityDuration = 1f;
    
    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    public UnityEvent OnHealed;

    private int currentHealth;
    private bool isDead = false;
    private float lastDamageTime = Mathf.NegativeInfinity;

    // Propiedad pública para acceso seguro
    public int CurrentHealth => currentHealth;
    public float HealthPercent => (float)currentHealth / maxHealth;

    private void Start() => InitializeHealth();

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        // Validaciones de seguridad
        if (isDead) return;
        if (invulnerableOnDamage && Time.time < lastDamageTime + invulnerabilityDuration) return;
        if (damage <= 0) return;

        lastDamageTime = Time.time;
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke();

        if (currentHealth <= 0) 
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        OnHealed?.Invoke();
    }

    public void FullHeal() => Heal(maxHealth);

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
    
        // Usar el Singleton para activar el panel de Game Over
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ActivateGameOverPanel();
        }
        else
        {
            Debug.LogError("UIManager.Instance es null. Asegúrate de tener un UIManager en la escena.");
        }
    }

    // Método para debug en el editor
    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        invulnerabilityDuration = Mathf.Max(0, invulnerabilityDuration);
    }
}