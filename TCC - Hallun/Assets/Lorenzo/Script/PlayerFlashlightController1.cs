using UnityEngine;
using TMPro; // Para a UI de texto
using System.Collections; // Para as Coroutines

// Este componente requer um AudioSource. Se não houver um, será adicionado automaticamente.
[RequireComponent(typeof(AudioSource))]
public class PlayerFlashlightController : MonoBehaviour
{
    // --- SEÇÃO DE CONFIGURAÇÃO ---
    [Header("Componentes e Estado")]
    [Tooltip("Arraste o objeto de Luz da sua lanterna aqui.")]
    public Light flashlightSource;
    [Tooltip("Este campo é controlado pelo script. Indica se o jogador já coletou a lanterna.")]
    public bool hasFlashlight = false; // Começa como falso

    [Header("Efeitos Sonoros")]
    [Tooltip("O som de clique ao ligar a lanterna.")]
    public AudioClip soundOn;
    [Tooltip("O som de clique ao desligar a lanterna.")]
    public AudioClip soundOff;

    [Header("UI e Mensagens")]
    [Tooltip("Arraste o objeto de texto do seu Canvas para cá.")]
    public TextMeshProUGUI infoText;
    [Tooltip("Quanto tempo (em segundos) a instrução 'Pressione Q' fica na tela.")]
    public float instructionDisplayTime = 4f;

    // --- VARIÁVEIS PRIVADAS ---
    private AudioSource audioSource;
    private const string PICKUP_MESSAGE = "Pressione 'E' para pegar a lanterna";
    private const string CONTROLS_MESSAGE = "Pressione 'Q' para ligar/desligar";


    // O método Start é chamado antes do primeiro frame
    void Start()
    {
        // 1. Validação Crítica: Garante que a luz foi atribuída no Inspector
        if (flashlightSource == null)
        {
            Debug.LogError("ERRO CRÍTICO: A referência 'Flashlight Source' não foi atribuída no Inspector!", this);
            this.enabled = false; // Desativa este script para evitar mais erros.
            return;
        }

        // 2. Inicialização do Áudio
        audioSource = GetComponent<AudioSource>();

        // 3. Estado Inicial da Lanterna e UI
        // Garante que o GameObject da luz está ativo, mas o componente de luz está desligado.
        flashlightSource.gameObject.SetActive(true);
        flashlightSource.enabled = false;

        // Garante que a UI de texto comece vazia.
        if (infoText != null)
        {
            infoText.text = "";
        }
    }

    // O método Update é chamado a cada frame
    void Update()
    {
        // A CONDIÇÃO MAIS IMPORTANTE:
        // Só verifica a tecla 'Q' se o jogador JÁ TIVER a lanterna.
        if (hasFlashlight && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleFlashlight();
        }
    }

    // Método que alterna o estado da lanterna (liga/desliga)
    private void ToggleFlashlight()
    {
        // 1. Inverte o estado da luz
        flashlightSource.enabled = !flashlightSource.enabled;

        // 2. Toca o som correspondente
        if (flashlightSource.enabled)
        {
            if (soundOn != null) audioSource.PlayOneShot(soundOn);
        }
        else
        {
            if (soundOff != null) audioSource.PlayOneShot(soundOff);
        }

        Debug.Log("Estado da Lanterna alterado para: " + (flashlightSource.enabled ? "LIGADA" : "DESLIGADA"));
    }

    // --- MÉTODOS PÚBLICOS (para serem chamados pelo script de coleta) ---

    // Mostra ou esconde a mensagem "Pressione E para pegar"
    public void DisplayPickupMessage(bool show)
    {
        if (infoText != null)
        {
            infoText.text = show ? PICKUP_MESSAGE : "";
        }
    }

    // Chamado pelo script da lanterna no chão quando ela é coletada
    public void OnFlashlightCollected()
    {
        hasFlashlight = true; // A "chave" é virada! Agora o Update pode usar a tecla 'Q'.
        flashlightSource.enabled = true; // Liga a lanterna na primeira vez.
        if (soundOn != null) audioSource.PlayOneShot(soundOn); // Toca o som de ligar.

        // Mostra a instrução de como usar e a faz desaparecer depois de um tempo.
        StartCoroutine(ShowAndHideControlsMessage());
    }

    // A rotina que mostra e esconde a mensagem de controle
    private IEnumerator ShowAndHideControlsMessage()
    {
        if (infoText != null)
        {
            infoText.text = CONTROLS_MESSAGE;
            yield return new WaitForSeconds(instructionDisplayTime);
            infoText.text = "";
        }
    }
}