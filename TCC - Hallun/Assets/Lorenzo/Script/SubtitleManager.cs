using UnityEngine;
using TMPro; // Necessário para usar TextMeshPro
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    // Singleton: Uma forma fácil de acessar este script de qualquer outro lugar
    public static SubtitleManager instance;

    [Header("Componentes da UI")]
    [Tooltip("Arraste o objeto Panel que serve de fundo para a legenda.")]
    public GameObject subtitlePanel;
    [Tooltip("Arraste o objeto de texto (TextMeshPro) da sua legenda.")]
    public TextMeshProUGUI subtitleText;

    private void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Garante que a legenda comece invisível
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }
        if (subtitleText != null)
        {
            subtitleText.text = "";
        }
    }

    // A função principal que outros scripts chamarão
    public void ShowSubtitle(string message, float duration)
    {
        // Inicia o processo de mostrar e esconder a legenda
        StartCoroutine(DisplaySubtitleCoroutine(message, duration));
    }

    // Coroutine para controlar o tempo de exibição
    private IEnumerator DisplaySubtitleCoroutine(string message, float duration)
    {
        // Define o texto e ativa o painel
        subtitleText.text = message;
        subtitlePanel.SetActive(true);

        // Espera pela duração definida
        yield return new WaitForSeconds(duration);

        // Limpa o texto e desativa o painel
        subtitleText.text = "";
        subtitlePanel.SetActive(false);
    }
}