using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);
    public float positionSmooth = 5f;
    public float rotationSmooth = 3f;

    void LateUpdate()
    {
        if(target == null) return;

        // Seguimiento de posición
        Vector3 targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmooth * Time.deltaTime);

        // Seguimiento de rotación
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth * Time.deltaTime);
    }
}
