using UnityEngine;

public class PickupFlashlight : MonoBehaviour
{
    // --- NOVO ---
    [Header("UI de Interação")]
    [Tooltip("Arraste aqui o objeto do Canvas que contém o ícone da tecla 'E'.")]
    public GameObject pickupIconCanvas; // Referência para o nosso Canvas com o ícone.

    private PlayerFlashlightController playerController;
    private bool playerIsNear = false;

    // --- NOVO ---
    // A função Start é chamada uma vez quando o jogo começa.
    // Vamos usá-la para garantir que o ícone esteja invisível no início.
    private void Start()
    {
        if (pickupIconCanvas != null)
        {
            // Desativa o Canvas do ícone para que ele não apareça antes do jogador se aproximar.
            pickupIconCanvas.SetActive(false);
        }
        else
        {
            // Um aviso caso você esqueça de arrastar o Canvas no Inspector.
            Debug.LogWarning("O 'Pickup Icon Canvas' não foi atribuído no Inspector deste objeto.", this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerFlashlightController>();
            if (playerController != null && !playerController.hasFlashlight)
            {
                playerIsNear = true;

                // --- ALTERADO ---
                // Em vez de mostrar uma mensagem de texto, agora ativamos o nosso ícone 3D.
                if (pickupIconCanvas != null)
                {
                    pickupIconCanvas.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;

            // --- ALTERADO ---
            // Quando o jogador se afasta, escondemos o ícone novamente.
            if (pickupIconCanvas != null)
            {
                pickupIconCanvas.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Esta parte continua igual. A lógica de pegar o item não muda.
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (playerController != null && !playerController.hasFlashlight)
            {
                playerController.OnFlashlightCollected();
                // Ao destruir o objeto da lanterna, o Canvas (que é filho dele) também será destruído.
                Destroy(gameObject);
            }
        }
    }
}