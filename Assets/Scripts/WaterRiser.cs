using UnityEngine;

public class WaterRiser : MonoBehaviour
{
    [Header("Configuración")]
    public float riseSpeed = 0.1f;
    public float maxHeight = 10f;
    public int damagePerSecond = 10; // Daño por segundo al jugador
    public float damageInterval = 0.5f; // Intervalo entre daños

    private MeshRenderer meshRenderer;
    private Vector3 meshOffset;
    private Vector3 startPosition;
    private float damageTimer;
    private bool playerInWater;

    void Start()
    {
        startPosition = transform.position;
        meshRenderer = GetComponent<MeshRenderer>();

        // Asegurar que tenemos un collider de trigger
        if (!GetComponent<Collider>())
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
        }

        if (meshRenderer != null)
        {
            meshOffset = meshRenderer.transform.position - transform.position;
        }
    }

    void Update()
    {
        // Mover el agua hacia arriba
        float currentY = transform.position.y;
        float nextY = Mathf.Min(currentY + riseSpeed * Time.deltaTime, maxHeight);
        transform.position = new Vector3(transform.position.x, nextY, transform.position.z);

        if (meshRenderer != null)
        {
            meshRenderer.transform.position = transform.position + meshOffset;
        }

        // Aplicar daño continuo si el jugador está en el agua
        if (playerInWater)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyWaterDamage();
                damageTimer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = true;
            damageTimer = damageInterval; // Dañar inmediatamente al entrar
            Debug.Log("Jugador entró en el agua");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = false;
            Debug.Log("Jugador salió del agua");
        }
    }

    private void ApplyWaterDamage()
    {
        PlayerHealth playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            // Calcular daño proporcional al intervalo
            int damage = Mathf.RoundToInt(damagePerSecond * damageInterval);
            playerHealth.TakeDamage(damage);
            Debug.Log($"Aplicando {damage} de daño por agua");
        }
    }
}