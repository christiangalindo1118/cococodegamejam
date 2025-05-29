using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private const string VOLUME_KEY = "MasterVolume";
    private const float DEFAULT_VOLUME = 0.8f;

    private void Awake()
    {
        // Configuración inicial de volumen
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        AudioListener.volume = savedVolume;
        
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(savedVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Configurar listeners de botones
        playButton?.onClick.AddListener(PlayGame);
        optionsButton?.onClick.AddListener(ToggleOptions);
        quitButton?.onClick.AddListener(QuitGame);

        // Asegurar estado inicial del panel
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    private void PlayGame()
    {
        // Prevenir múltiples clics durante carga
        playButton.interactable = false;
        SceneManager.LoadScene("Level1");
    }

    public void ToggleOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        
        // Opcional: Pausar el juego cuando el menú está abierto
        Time.timeScale = optionsPanel.activeSelf ? 0f : 1f;
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // Limpieza de event listeners
    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        
        playButton?.onClick.RemoveListener(PlayGame);
        optionsButton?.onClick.RemoveListener(ToggleOptions);
        quitButton?.onClick.RemoveListener(QuitGame);
    }
}