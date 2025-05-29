using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    [Range(0.1f, 20f)] public float positionSmooth = 5f;
    [Range(0.1f, 20f)] public float rotationSmooth = 3f;

    private Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;

    void LateUpdate()
    {
        if(target == null) return;

        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateCameraPosition()
    {
        // Calcular posición objetivo con offset relativo
        Vector3 targetPosition = target.TransformPoint(offset);
        
        // Usar SmoothDamp para movimiento suave independiente del framerate
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            1f / positionSmooth
        );
    }

    void UpdateCameraRotation()
    {
        // Calcular dirección hacia el objetivo (con compensación de altura)
        Vector3 lookDirection = target.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(lookDirection);
            
            // Rotación suavizada con slerp
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmooth * Time.deltaTime
            );
        }
    }

    // Validación en el editor
    void OnValidate()
    {
        positionSmooth = Mathf.Clamp(positionSmooth, 0.1f, 20f);
        rotationSmooth = Mathf.Clamp(rotationSmooth, 0.1f, 20f);
    }
}
