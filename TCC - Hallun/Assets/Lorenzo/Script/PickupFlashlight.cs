using UnityEngine;

public class PickupFlashlight : MonoBehaviour
{
    private PlayerFlashlightController playerController;
    private bool playerIsNear = false;

    // Quando o jogador entra na área do trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerFlashlightController>();
            if (playerController != null && !playerController.hasFlashlight)
            {
                playerIsNear = true;
                playerController.DisplayPickupMessage(true); // Pede para o script do player mostrar a mensagem
            }
        }
    }

    // Quando o jogador sai da área do trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (playerController != null)
            {
                playerController.DisplayPickupMessage(false); // Pede para o script do player esconder a mensagem
            }
        }
    }

    // Verifica a cada frame se o jogador quer pegar o item
    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (playerController != null && !playerController.hasFlashlight)
            {
                playerController.OnFlashlightCollected(); // Avisa o player que a lanterna foi coletada
                Destroy(gameObject); // Destrói a si mesmo (a lanterna no chão)
            }
        }
    }
}