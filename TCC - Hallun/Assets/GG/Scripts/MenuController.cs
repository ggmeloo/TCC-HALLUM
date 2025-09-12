using UnityEngine;
using UnityEngine.SceneManagement; // Importe o namespace SceneManagement

public class NewGameButton : MonoBehaviour
{
    // Método público para ser chamado pelo botão
    public void StartNewGame()
    {
        // Carrega a cena com o nome especificado
        // Certifique-se de que a cena "GameScene" (ou o nome que você escolher)
        // esteja adicionada nas Build Settings (File > Build Settings...)
        SceneManager.LoadScene("GameScene");
    }
}