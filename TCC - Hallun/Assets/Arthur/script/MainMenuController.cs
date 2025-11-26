using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necessário para Image
using UnityEngine.Video;
using TMPro; // Mantenho aqui caso queira usar texto no futuro, mas é opcional
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Painéis da UI")]
    public GameObject pressAnyKeyPanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Configuração do Vídeo (Intro)")]
    public RawImage telaDoVideo;
    public VideoPlayer videoPlayer;

    [Header("Tela de Carregamento (Visual)")]
    public GameObject loadingPanel;

    // MUDANÇA AQUI: Trocamos Slider por Image
    [Tooltip("Arraste aqui a IMAGEM que está configurada como 'Filled'")]
    public Image imagemDeCarregamento;

    // Deixei o texto opcional, caso queira remover basta não arrastar nada
    public TextMeshProUGUI loadingText;

    public string nomeDaCenaDoJogo = "GameScene";

    [Header("Ajustes de Tempo")]
    public float tempoMinimoDeCarregamento = 4.0f;

    private bool anyKeyPressed = false;
    private bool videoTerminou = false;

    void Start()
    {
        if (pressAnyKeyPanel) pressAnyKeyPanel.SetActive(true);
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
        if (telaDoVideo) telaDoVideo.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!anyKeyPressed && Input.anyKeyDown)
        {
            anyKeyPressed = true;
            ShowMainMenu();
        }
    }

    private void ShowMainMenu()
    {
        if (pressAnyKeyPanel) pressAnyKeyPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    public void StartGameSequence()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);

        if (videoPlayer != null && telaDoVideo != null)
        {
            StartCoroutine(TocarVideoEDepoisCarregar());
        }
        else
        {
            StartCoroutine(CarregarCenaComTempoMinimo());
        }
    }

    IEnumerator TocarVideoEDepoisCarregar()
    {
        telaDoVideo.gameObject.SetActive(true);
        videoPlayer.isLooping = false;
        videoTerminou = false;
        videoPlayer.loopPointReached += QuandoOVideoAcabar;
        videoPlayer.Play();

        while (!videoTerminou)
        {
            yield return null;
        }

        videoPlayer.loopPointReached -= QuandoOVideoAcabar;
        telaDoVideo.gameObject.SetActive(false);
        StartCoroutine(CarregarCenaComTempoMinimo());
    }

    void QuandoOVideoAcabar(VideoPlayer vp)
    {
        videoTerminou = true;
    }

    IEnumerator CarregarCenaComTempoMinimo()
    {
        loadingPanel.SetActive(true);

        // Garante que a imagem comece vazia
        if (imagemDeCarregamento) imagemDeCarregamento.fillAmount = 0f;

        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeDaCenaDoJogo);
        operacao.allowSceneActivation = false;

        float tempoDecorrido = 0f;

        while (!operacao.isDone)
        {
            tempoDecorrido += Time.deltaTime;

            float progressoReal = Mathf.Clamp01(operacao.progress / 0.9f);
            float progressoTempo = Mathf.Clamp01(tempoDecorrido / tempoMinimoDeCarregamento);
            float progressoFinal = Mathf.Min(progressoReal, progressoTempo);

            // MUDANÇA AQUI: Atualiza o preenchimento da imagem
            if (imagemDeCarregamento != null)
            {
                imagemDeCarregamento.fillAmount = progressoFinal;
            }

            // Se ainda tiver o texto, atualiza ele, senão ignora
            if (loadingText != null)
            {
                loadingText.text = (progressoFinal * 100).ToString("F0") + "%";
            }

            if (operacao.progress >= 0.9f && tempoDecorrido >= tempoMinimoDeCarregamento)
            {
                if (imagemDeCarregamento) imagemDeCarregamento.fillAmount = 1f;
                if (loadingText) loadingText.text = "100%";

                yield return new WaitForSeconds(0.5f);
                operacao.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}