using UnityEngine;
using TMPro;

public class PickupFlashlight : MonoBehaviour
{
    [Tooltip("Arraste o objeto de texto da sua UI para cá.")]
    public TextMeshProUGUI textoDeColeta;

    private PlayerActions playerActions; // Agora a referência é para o script do Player
    private bool playerIsNear = false;

    void Start()
    {
        if (textoDeColeta != null)
        {
            textoDeColeta.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o script PlayerActions no objeto que colidiu
            playerActions = other.GetComponent<PlayerActions>();

            // Verifica se o player realmente tem o script e se a lanterna ainda não foi coletada
            if (playerActions != null && playerActions.lanternaController != null && !playerActions.lanternaController.temLanterna)
            {
                playerIsNear = true;
                if (textoDeColeta != null) textoDeColeta.text = "Pressione 'E' para pegar a lanterna";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (textoDeColeta != null) textoDeColeta.text = "";
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (playerActions != null)
            {
                // Chama o método no Player, que vai repassar o comando
                playerActions.ColetouLanterna();

                // Limpa o texto e se autodestrói
                if (textoDeColeta != null) textoDeColeta.text = "";
                Destroy(gameObject);
            }
        }
    }
}