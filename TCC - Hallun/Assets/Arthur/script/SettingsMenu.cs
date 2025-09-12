using System.Collections.Generic;
using System.Linq; // Usado para pegar a última resolução
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    // Isso garante que exista apenas um SettingsManager e que ele seja facilmente acessível.
    public static SettingsManager Instance { get; private set; }

    [Header("Controles de Áudio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Controles de Vídeo")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;

    private Resolution[] resolutions;

    // O Awake é chamado antes de qualquer Start, ideal para inicialização crítica.
    void Awake()
    {
        // Implementação do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Garante que o manager não seja destruído ao carregar novas cenas.

        // A função mais importante: carrega as configurações salvas ou define os padrões na primeira vez.
        LoadSettingsOrDefault();
    }

    // O Start é usado para configurar a UI para refletir as configurações já carregadas no Awake.
    void Start()
    {
        SetupUI();
    }

    private void LoadSettingsOrDefault()
    {
        // PlayerPrefs são pequenos arquivos de salvamento. Verificamos se já rodamos o jogo antes.
        if (PlayerPrefs.HasKey("HasSavedSettings"))
        {
            // --- CARREGAR CONFIGURAÇÕES EXISTENTES ---
            // Qualidade
            int qualityIndex = PlayerPrefs.GetInt("QualityLevel");
            QualitySettings.SetQualityLevel(qualityIndex, true);

            // Resolução
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutions = Screen.resolutions.Distinct().ToArray(); // Pega resoluções únicas
            if (resolutionIndex < resolutions.Length)
            {
                Resolution res = resolutions[resolutionIndex];
                Screen.SetResolution(res.width, res.height, (FullScreenMode)PlayerPrefs.GetInt("FullscreenMode"));
            }

            // Áudio
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume");
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            // --- DEFINIR CONFIGURAÇÕES PADRÃO NA PRIMEIRA VEZ ---
            Debug.Log("Primeira inicialização: Definindo configurações padrão.");

            // Qualidade (define para o nível mais alto disponível)
            int defaultQualityIndex = QualitySettings.names.Length - 1;
            QualitySettings.SetQualityLevel(defaultQualityIndex, true);
            PlayerPrefs.SetInt("QualityLevel", defaultQualityIndex);

            // Resolução (tenta encontrar 1920x1080 ou a mais alta disponível)
            resolutions = Screen.resolutions.Distinct().ToArray();
            int defaultResolutionIndex = resolutions.Length - 1; // Padrão para a mais alta
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == 1920 && resolutions[i].height == 1080)
                {
                    defaultResolutionIndex = i;
                    break;
                }
            }
            Resolution defaultRes = resolutions[defaultResolutionIndex];
            Screen.SetResolution(defaultRes.width, defaultRes.height, FullScreenMode.ExclusiveFullScreen);
            PlayerPrefs.SetInt("ResolutionIndex", defaultResolutionIndex);

            // Tela cheia
            PlayerPrefs.SetInt("FullscreenMode", (int)FullScreenMode.ExclusiveFullScreen);

            // Áudio
            musicSource.volume = 0.8f;
            sfxSource.volume = 0.8f;
            PlayerPrefs.SetFloat("MusicVolume", 0.8f);
            PlayerPrefs.SetFloat("SFXVolume", 0.8f);

            // Marca que já salvamos as configurações para não rodar isso de novo.
            PlayerPrefs.SetInt("HasSavedSettings", 1);
            PlayerPrefs.Save(); // Salva tudo no disco.
        }
    }

    // Popula a UI com os valores atuais do jogo.
    private void SetupUI()
    {
        // Desliga os listeners para evitar que as funções sejam chamadas durante a configuração.
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        fullscreenToggle.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        qualityDropdown.onValueChanged.RemoveAllListeners();

        // --- Configura Áudio ---
        musicSlider.value = musicSource.volume;
        sfxSlider.value = sfxSource.volume;

        // --- Configura Vídeo ---
        fullscreenToggle.isOn = Screen.fullScreen;

        // Qualidade
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Resolução
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions.Distinct().ToArray();
        int currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Liga os listeners novamente para que o jogador possa fazer alterações.
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    // --- Funções Públicas para serem chamadas pela UI ---

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        FullScreenMode mode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.fullScreenMode = mode;
        PlayerPrefs.SetInt("FullscreenMode", (int)mode);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }



    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }
}