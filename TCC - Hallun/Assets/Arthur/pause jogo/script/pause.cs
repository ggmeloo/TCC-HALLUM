using UnityEngine;

public class pause : MonoBehaviour
{
    // A variável que controlará se o jogo está pausado ou não
    public static bool isGamePaused = false;

    // A referência ao nosso painel do menu de pausa
    public GameObject pauseMenuUI;

    // A função de Update é chamada a cada frame
    void Update()
    {
        // Verifica se a tecla "Escape" foi pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Função para continuar o jogo
    public void Resume()
    {
        // Esconde o menu de pausa
        pauseMenuUI.SetActive(false);
        // Retoma o tempo normal do jogo (velocidade 1)
        Time.timeScale = 1f;
        // Atualiza o estado da pausa
        isGamePaused = false;
    }

    // Função para pausar o jogo
    void Pause()
    {
        // Mostra o menu de pausa
        pauseMenuUI.SetActive(true);
        // Para o tempo do jogo (velocidade 0)
        Time.timeScale = 0f;
        // Atualiza o estado da pausa
        isGamePaused = true;
    }

    // Função para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo..."); // Apenas para debug no editor
        Application.Quit(); // Esta linha só funciona em builds executáveis
    }
}