using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerFlashlightController : MonoBehaviour
{
    [Header("Componentes e Estado")]
    [Tooltip("Arraste o objeto de Luz da sua lanterna (que está na câmera) aqui.")]
    public Light flashlightSource;
    [Tooltip("Controlado pelo script. Indica se o jogador já coletou a lanterna.")]
    public bool hasFlashlight = false;

    [Header("Efeitos Sonoros")]
    public AudioClip soundOn;
    public AudioClip soundOff;

    [Header("UI e Mensagens")]
    public TextMeshProUGUI infoText;
    public float instructionDisplayTime = 4f;

    private AudioSource audioSource;
    private const string PICKUP_MESSAGE = "Pressione 'E' para pegar a lanterna";
    private const string CONTROLS_MESSAGE = "Pressione 'Q' para ligar/desligar";

    void Start()
    {
        if (flashlightSource == null)
        {
            Debug.LogError("ERRO CRÍTICO: A referência 'Flashlight Source' não foi atribuída no Inspetor!", this);
            this.enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();

        // Garante que o GameObject da luz está ativo, mas o componente de luz está desligado.
        flashlightSource.gameObject.SetActive(true);
        flashlightSource.enabled = false;

        if (infoText != null) infoText.text = "";
    }

    void Update()
    {
        // Só verifica a tecla 'Q' se o jogador JÁ TIVER a lanterna.
        if (hasFlashlight && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        flashlightSource.enabled = !flashlightSource.enabled;
        AudioClip clipToPlay = flashlightSource.enabled ? soundOn : soundOff;
        if (clipToPlay != null) audioSource.PlayOneShot(clipToPlay);
    }

    // Chamado pelo script da lanterna no chão quando ela é coletada
    public void OnFlashlightCollected()
    {
        hasFlashlight = true;
        flashlightSource.enabled = true;
        if (soundOn != null) audioSource.PlayOneShot(soundOn);

        StartCoroutine(ShowAndHideControlsMessage());
    }

    // Mostra ou esconde a mensagem "Pressione E para pegar"
    public void DisplayPickupMessage(bool show)
    {
        if (infoText != null)
        {
            infoText.text = show ? PICKUP_MESSAGE : "";
        }
    }

    private IEnumerator ShowAndHideControlsMessage()
    {
        if (infoText != null)
        {
            infoText.text = CONTROLS_MESSAGE;
            yield return new WaitForSeconds(instructionDisplayTime);
            if (infoText.text == CONTROLS_MESSAGE) // Só apaga se a mensagem ainda for a mesma
            {
                infoText.text = "";
            }
        }
    }
}