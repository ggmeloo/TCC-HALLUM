using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections; // Necessário

public class GameManager : MonoBehaviour
{
    [Header("Referências de Vídeo")]
    public GameObject gameContent;
    public RawImage videoScreen;
    public VideoPlayer videoPlayer;

    [Header("Tela de Carregamento (Pós-Vídeo)")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public string nomeDaCenaDoJogo = "GameScene";

    private void Start()
    {
        if (videoScreen) videoScreen.gameObject.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
    }

    public void StartNewGameWithVideo()
    {
        // Desativa o menu/conteúdo
        if (gameContent) gameContent.SetActive(false);

        // Ativa e toca o vídeo
        if (videoScreen) videoScreen.gameObject.SetActive(true);
        if (videoPlayer)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            // Se não tiver vídeo, carrega direto
            StartCoroutine(CarregarCenaAsync());
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Quando o vídeo acaba, inicia o carregamento
        StartCoroutine(CarregarCenaAsync());
    }

    IEnumerator CarregarCenaAsync()
    {
        if (videoScreen) videoScreen.gameObject.SetActive(false); // Esconde o vídeo
        if (loadingPanel) loadingPanel.SetActive(true); // Mostra loading

        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeDaCenaDoJogo);

        while (!operacao.isDone)
        {
            float progresso = Mathf.Clamp01(operacao.progress / 0.9f);
            if (loadingBar != null) loadingBar.value = progresso;
            yield return null;
        }
    }
}