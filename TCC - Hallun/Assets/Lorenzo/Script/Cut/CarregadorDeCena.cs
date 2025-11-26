using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para carregar cenas

public class CarregadorDeCena : MonoBehaviour
{
    // Esta função pública será chamada pela Timeline no final da cutscene.
    // O nomeDaCena será o nome da sua cena de gameplay.
    public void CarregarCena(string nomeDaCena)
    {
        // Verifica se o nome da cena não está vazio
        if (!string.IsNullOrEmpty(nomeDaCena))
        {
            Debug.Log($"Carregando cena: {nomeDaCena}");
            SceneManager.LoadScene(nomeDaCena);
        }
        else
        {
            Debug.LogError("O nome da cena para carregar não foi especificado!");
        }
    }
}