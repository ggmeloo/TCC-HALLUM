using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    // Crie variáveis públicas para arrastar os objetos da Unity no Inspector
    public GameObject gameContent;      // O painel que contém todos os elementos da sua tela de jogo (botões, texto, etc.)
    public RawImage videoScreen;        // O Raw Image que exibe o vídeo
    public VideoPlayer videoPlayer;     // O componente Video Player

    private void Start()
    {
        // Certifica de que a tela de vídeo está escondida no início do jogo
        videoScreen.gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        // 1. Desativa o conteúdo do jogo
        gameContent.SetActive(false);

        // 2. Ativa o objeto que exibe o vídeo
        videoScreen.gameObject.SetActive(true);

        // 3. Toca o vídeo
        videoPlayer.Play();

        // Adiciona um evento que será chamado quando o vídeo terminar
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Você pode substituir a linha acima por "SceneManager.LoadScene("NomeDaSuaCena");" se quiser carregar outra tela
        SceneManager.LoadScene(1);
    }
}