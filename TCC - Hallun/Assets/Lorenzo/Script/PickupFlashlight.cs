using UnityEngine;

public class PickupFlashlight : MonoBehaviour
{
    private PlayerFlashlightController playerController;
    private bool playerIsNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerFlashlightController>();
            if (playerController != null && !playerController.hasFlashlight)
            {
                playerIsNear = true;
                playerController.DisplayPickupMessage(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (playerController != null)
            {
                playerController.DisplayPickupMessage(false);
            }
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (playerController != null && !playerController.hasFlashlight)
            {
                playerController.OnFlashlightCollected();
                Destroy(gameObject);
            }
        }
    }
}