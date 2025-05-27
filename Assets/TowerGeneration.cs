using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    public GameObject salientePrefab;
    public int totalSteps = 30;
    public float stepHeight = 1.0f;

    void Start()
    {
        GenerateTower();
    }

    void GenerateTower()
    {
        for (int i = 0; i < totalSteps; i++)
        {
            // Posición vertical solamente
            float y = i * stepHeight;
            
            Vector3 spawnPos = new Vector3(0, y, 0);
            GameObject saliente = Instantiate(
                salientePrefab, 
                spawnPos, 
                Quaternion.Euler(0, 0, 0), // Rotación neutra
                this.transform
            );
            saliente.name = "Saliente_" + i;
        }
    }
}