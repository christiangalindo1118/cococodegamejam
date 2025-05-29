using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private GameObject salientePrefab;
    [SerializeField] [Range(5, 200)] private int totalSteps = 30;
    [SerializeField] [Min(0.1f)] private float stepHeight = 1.0f;
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool randomizeRotation = false;
    
    [Header("Optimization")]
    [SerializeField] private bool combineMeshes = true;
    [SerializeField] private bool staticTower = true;
    
    private Transform towerContainer;
    
    void Start()
    {
        if (generateOnStart) 
            GenerateTower();
    }
    
    public void GenerateTower()
    {
        CleanExistingTower();
        CreateTowerContainer();
        
        for (int i = 0; i < totalSteps; i++)
        {
            CreateSaliente(i);
        }
        
        OptimizeTower();
    }
    
    private void CleanExistingTower()
    {
        if (towerContainer != null)
        {
            if (Application.isPlaying)
                Destroy(towerContainer.gameObject);
            else
                DestroyImmediate(towerContainer.gameObject);
        }
    }
    
    private void CreateTowerContainer()
    {
        towerContainer = new GameObject("Tower_Container").transform;
        towerContainer.SetParent(transform);
        towerContainer.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    
    private void CreateSaliente(int index)
    {
        Vector3 spawnPos = new Vector3(0, index * stepHeight, 0);
        Quaternion rotation = randomizeRotation ? 
            Quaternion.Euler(0, Random.Range(0, 360), 0) : 
            Quaternion.identity;
            
        GameObject saliente = Instantiate(
            salientePrefab, 
            spawnPos, 
            rotation,
            towerContainer
        );
        
        saliente.name = $"Saliente_{index}";
        
        if (staticTower)
        {
            saliente.isStatic = true;
            saliente.gameObject.hideFlags = HideFlags.NotEditable;
        }
    }
    
    private void OptimizeTower()
    {
        if (combineMeshes)
        {
            CombineMeshes();
        }
    }
    
    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = towerContainer.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);
        
        GameObject combinedObject = new GameObject("Combined_Tower");
        combinedObject.transform.SetParent(towerContainer);
        combinedObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
        combinedObject.AddComponent<MeshRenderer>().sharedMaterial = 
            salientePrefab.GetComponent<Renderer>().sharedMaterial;
        
        combinedObject.isStatic = true;
        combinedObject.hideFlags = HideFlags.NotEditable;
    }
    
    // Context menu para generación manual en el editor
    [ContextMenu("Generate Tower")]
    private void GenerateTowerEditor()
    {
        GenerateTower();
    }
    
    [ContextMenu("Clear Tower")]
    private void ClearTower()
    {
        CleanExistingTower();
    }
    
    private void OnValidate()
    {
        totalSteps = Mathf.Max(1, totalSteps);
        stepHeight = Mathf.Max(0.1f, stepHeight);
    }
}