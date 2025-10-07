using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerFlashlightController : MonoBehaviour
{
    [Header("Componentes Essenciais")]
    [Tooltip("Arraste o objeto que contém a sua 'Light' da lanterna para cá.")]
    public Light flashlightSource;
    [Tooltip("Arraste o objeto da sua Câmera Principal ('Main Camera') para cá. Essencial para o som e movimento.")]
    public Transform cameraTransform;
    [Tooltip("Arraste o seu objeto de texto da UI (TextMeshPro) para cá.")]
    public TextMeshProUGUI infoText;

    [Header("Sons da Lanterna")]
    [Tooltip("O som que toca quando a lanterna LIGA.")]
    public AudioClip turnOnSound;
    [Tooltip("O som que toca quando a lanterna DESLIGA.")]
    public AudioClip turnOffSound;

    [Header("Efeitos de Realismo da Lanterna")]
    [Tooltip("Quão suavemente a lanterna seguirá a câmera. Valores menores = mais delay.")]
    public float smoothFactor = 4f;
    [Tooltip("A intensidade do balanço da lanterna.")]
    public float swayIntensity = 0.05f;
    [Tooltip("A velocidade do balanço da lanterna.")]
    public float swaySpeed = 2f;

    [Header("Efeitos de Pisca Programada")]
    [Tooltip("Habilita o sistema de pisca programada.")]
    public bool programmedFlickerEnabled = true;
    [Tooltip("Tempo MÍNIMO em segundos entre as sequências de piscadas.")]
    public float minTimeBetweenFlickers = 15f;
    [Tooltip("Tempo MÁXIMO em segundos entre as sequências de piscadas.")]
    public float maxTimeBetweenFlickers = 40f;
    [Tooltip("Quantas vezes a luz vai piscar em uma sequência.")]
    public int numberOfFlickers = 2;
    [Tooltip("A duração que a luz fica apagada em cada piscada.")]
    public float flickerOffDuration = 0.1f;

    [Header("Controle de Bateria (Opcional)")]
    [Tooltip("Habilita o sistema de bateria.")]
    public bool hasBatterySystem = true;
    public float maxBatteryLife = 100f;
    public float batteryDrainRate = 1f;

    // Variável restaurada para evitar erros.
    [Header("UI e Mensagens")]
    public float instructionDisplayTime = 4f;

    [HideInInspector]
    public bool hasFlashlight = false;
    private float currentBatteryLife;
    private AudioSource audioSource;
    private const string PICKUP_MESSAGE = "Pressione 'E' para pegar a lanterna";
    private const string CONTROLS_MESSAGE = "Pressione 'Q' para ligar/desliggar";


    void Awake()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("ERRO: A 'Camera Transform' não foi definida no Inspector! O som e o movimento da lanterna não funcionarão.");
            return;
        }

        audioSource = cameraTransform.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = cameraTransform.gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
    }

    void Start()
    {
        if (flashlightSource != null) flashlightSource.enabled = false;
        if (infoText != null) infoText.text = "";
        currentBatteryLife = maxBatteryLife;
    }

    void Update()
    {
        if (hasFlashlight)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleFlashlight();
            }

            if (flashlightSource.enabled)
            {
                HandleBattery();
            }
        }
    }

    void LateUpdate()
    {
        if (flashlightSource != null && hasFlashlight && cameraTransform != null)
        {
            Quaternion targetRotation = cameraTransform.rotation;
            flashlightSource.transform.rotation = Quaternion.Slerp(flashlightSource.transform.rotation, targetRotation, smoothFactor * Time.deltaTime);
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
            float swayY = Mathf.Cos(Time.time * swaySpeed) * swayIntensity;
            Vector3 swayOffset = new Vector3(swayX, swayY, 0);
            flashlightSource.transform.localRotation *= Quaternion.Euler(swayOffset);
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlightSource == null) return;
        if (currentBatteryLife <= 0 && !flashlightSource.enabled) return;

        bool wasEnabled = flashlightSource.enabled;
        flashlightSource.enabled = !wasEnabled;

        if (!wasEnabled && flashlightSource.enabled)
        {
            if (turnOnSound != null) audioSource.PlayOneShot(turnOnSound);
        }
        else if (wasEnabled && !flashlightSource.enabled)
        {
            if (turnOffSound != null) audioSource.PlayOneShot(turnOffSound);
        }
    }

    public void OnFlashlightCollected()
    {
        hasFlashlight = true;
        ToggleFlashlight();
        StartCoroutine(ShowAndHideControlsMessage());
        if (programmedFlickerEnabled) { StartCoroutine(FlickerController()); }
    }

    private void HandleBattery()
    {
        if (!hasBatterySystem) return;
        if (currentBatteryLife > 0)
        {
            currentBatteryLife -= batteryDrainRate * Time.deltaTime;
        }
        else
        {
            currentBatteryLife = 0;
            if (flashlightSource.enabled)
            {
                ToggleFlashlight();
            }
        }
    }

    private IEnumerator FlickerController()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers);
            yield return new WaitForSeconds(waitTime);
            if (hasFlashlight && flashlightSource != null && flashlightSource.enabled)
            {
                StartCoroutine(ExecuteFlickerSequence());
            }
        }
    }

    private IEnumerator ExecuteFlickerSequence()
    {
        for (int i = 0; i < numberOfFlickers; i++)
        {
            if (flashlightSource == null) yield break;
            flashlightSource.enabled = false;
            yield return new WaitForSeconds(flickerOffDuration);
            if (flashlightSource == null) yield break;
            flashlightSource.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        if (flashlightSource != null) flashlightSource.enabled = true;
    }

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
            infoText.text = "";
        }
    }
}