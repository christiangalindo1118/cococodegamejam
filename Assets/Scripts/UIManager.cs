using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject optionsPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Audio Settings")]
    public Slider volumeSlider;

    private const string VOLUME_KEY = "MasterVolume";
    private const float DEFAULT_VOLUME = 0.7f;
    
    private bool isPaused;
    private bool isGameOver;

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Opcional si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Configuración inicial
        Time.timeScale = 1f;
        HideAllPanels();
        
        // Cargar configuración de volumen
        AudioListener.volume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        if (volumeSlider) volumeSlider.value = AudioListener.volume;
    }

    void Update()
    {
        // Detección de tecla P para pausa
        if (Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    private void HideAllPanels()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, AudioListener.volume);
        PlayerPrefs.Save();
    }

    // Métodos públicos para botones UI
    public void LoadKhriscodeScene() => SceneManager.LoadScene("khriscode");
    
    public void SetVolume(float volume) => AudioListener.volume = volume;
    
    public void ToggleOptions()
    {
        if (!optionsPanel) return;
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
    
    public void TogglePause()
    {
        if (isGameOver) return;
        
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        
        if (pausePanel) pausePanel.SetActive(isPaused);
    }
    
    public void ActivateGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void ReturnToMainMenu()
    {
        SaveSettings();
        Time.timeScale = 1;
        SceneManager.LoadScene("Opening");
    }
    
    public void QuitGame()
    {
        SaveSettings();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}