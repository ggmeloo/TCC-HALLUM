using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Painéis do Menu")]
    public GameObject pressAnyKeyPanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    private bool anyKeyPressed = false;

    void Start()
    {
        // Garante que o painel inicial esteja ativo e os outros desativados
        pressAnyKeyPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        // Verifica se qualquer tecla foi pressionada e o menu principal ainda não foi ativado
        if (!anyKeyPressed && Input.anyKeyDown)
        {
            anyKeyPressed = true;
            ShowMainMenu();
        }
    }

    private void ShowMainMenu()
    {
        pressAnyKeyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void StartGame()
    {
        // Carrega a cena principal do jogo
        // Certifique-se de que "GameScene" está adicionada em File > Build Settings
        SceneManager.LoadScene("GameScene");
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
        // Fecha a aplicação
        Debug.Log("Saindo do jogo..."); // Mensagem de depuração para o editor da Unity
        Application.Quit();
    }
}