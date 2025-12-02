using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
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

    [Header("Tela de Carregamento (Barra/Imagem)")]
    public GameObject loadingPanel;

    [Tooltip("A imagem PEQUENA que vai se preenchendo (Filled)")]
    public Image imagemDeProgresso;

    [Tooltip("Texto de porcentagem (Opcional)")]
    public TextMeshProUGUI loadingText;

    [Header("Slideshow de Fundo")]
    [Tooltip("A imagem GRANDE que serve de fundo para a tela de loading")]
    public Image imagemDeFundoLoading;

    [Tooltip("Lista de imagens que vão passar no fundo")]
    public Sprite[] imagensDoSlideshow;

    [Tooltip("Tempo (em segundos) que a imagem fica parada totalmente visível")]
    public float tempoPorImagem = 3.0f;

    [Tooltip("Tempo (em segundos) que leva para a imagem aparecer/desaparecer")]
    public float tempoDeTransicao = 1.0f;

    [Header("Configuração de Destino")]
    [Tooltip("Coloque aqui o nome da CENA DE TUTORIAL (que vem depois do loading)")]
    public string nomeDaCenaDeDestino = "TutorialScene";

    public float tempoMinimoDeCarregamento = 5.0f;

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

        // --- CORREÇÃO DO VÍDEO PULANDO ---
        videoPlayer.Stop(); // Garante que parou
        videoPlayer.frame = 0; // Volta pro começo
        videoPlayer.isLooping = false;
        videoTerminou = false;

        // Prepara o vídeo na memória antes de tocar
        videoPlayer.Prepare();

        // Espera a Unity carregar o vídeo
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        telaDoVideo.texture = videoPlayer.texture; // Garante que a imagem apareça

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
        if (imagemDeProgresso) imagemDeProgresso.fillAmount = 0f;

        Coroutine slideshow = StartCoroutine(RodarSlideshowDeFundo());

        // Carrega a cena definida (no seu caso, o TUTORIAL)
        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeDaCenaDeDestino);
        operacao.allowSceneActivation = false;

        float tempoDecorrido = 0f;

        while (!operacao.isDone)
        {
            tempoDecorrido += Time.deltaTime;

            float progressoReal = Mathf.Clamp01(operacao.progress / 0.9f);
            float progressoTempo = Mathf.Clamp01(tempoDecorrido / tempoMinimoDeCarregamento);
            float progressoFinal = Mathf.Min(progressoReal, progressoTempo);

            if (imagemDeProgresso != null)
                imagemDeProgresso.fillAmount = progressoFinal;

            if (loadingText != null)
                loadingText.text = (progressoFinal * 100).ToString("F0") + "%";

            if (operacao.progress >= 0.9f && tempoDecorrido >= tempoMinimoDeCarregamento)
            {
                if (imagemDeProgresso) imagemDeProgresso.fillAmount = 1f;
                if (loadingText) loadingText.text = "100%";

                StopCoroutine(slideshow);

                yield return new WaitForSeconds(0.5f);
                operacao.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator RodarSlideshowDeFundo()
    {
        if (imagemDeFundoLoading == null || imagensDoSlideshow == null || imagensDoSlideshow.Length == 0)
        {
            yield break;
        }

        int index = 0;

        // Setup inicial transparente
        Color cor = imagemDeFundoLoading.color;
        cor.a = 0f;
        imagemDeFundoLoading.color = cor;

        while (true)
        {
            imagemDeFundoLoading.sprite = imagensDoSlideshow[index];
            yield return StartCoroutine(FadeImagem(0f, 1f));
            yield return new WaitForSeconds(tempoPorImagem);
            yield return StartCoroutine(FadeImagem(1f, 0f));

            index++;
            if (index >= imagensDoSlideshow.Length) index = 0;
        }
    }

    IEnumerator FadeImagem(float alphaInicial, float alphaFinal)
    {
        float tempoPassado = 0f;
        Color corAtual = imagemDeFundoLoading.color;

        while (tempoPassado < tempoDeTransicao)
        {
            tempoPassado += Time.deltaTime;
            float novoAlpha = Mathf.Lerp(alphaInicial, alphaFinal, tempoPassado / tempoDeTransicao);
            corAtual.a = novoAlpha;
            imagemDeFundoLoading.color = corAtual;
            yield return null;
        }
        corAtual.a = alphaFinal;
        imagemDeFundoLoading.color = corAtual;
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