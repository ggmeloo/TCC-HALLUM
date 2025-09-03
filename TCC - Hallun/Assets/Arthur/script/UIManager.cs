using UnityEngine;
using UnityEngine.UI; // Necessário para acessar componentes de UI
using TMPro; // Use se você estiver usando TextMeshPro

public class UIManager : MonoBehaviour
{
    // Crie uma referência pública para a imagem
    public Image logoImage;

    // Crie uma referência pública para o botão
    public Button optionButton;

    // A função Start é chamada no início, quando o script é ativado
    void Start()
    {
        // Adiciona um listener (ouvinte) para o evento de clique do botão
        // Quando o botão for clicado, a função OnOptionButtonClick será chamada
        optionButton.onClick.AddListener(OnOptionButtonClick);
    }

    // Esta função será chamada toda vez que o botão for clicado
    void OnOptionButtonClick()
    {
        // Alterna a visibilidade da imagem. Se ela estiver visível, ela será desativada
        // Se ela estiver invisível, ela será ativada
        // Para fazer ela simplesmente desaparecer e não voltar, use:
        // logoImage.gameObject.SetActive(false);
        logoImage.gameObject.SetActive(!logoImage.gameObject.activeSelf);

        Debug.Log("O botão 'Option' foi clicado!");
    }
}