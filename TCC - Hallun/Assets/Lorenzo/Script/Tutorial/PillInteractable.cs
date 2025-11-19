using UnityEngine;

public class PillInteractable : MonoBehaviour
{
    [Header("Configuração da Interação")]
    [Tooltip("Arraste o Canvas com o texto 'Pressione E' para este campo.")]
    public GameObject interactionCanvas; // O Canvas que mostra a tecla

    [Tooltip("A tecla que o jogador deve pressionar para coletar.")]
    public KeyCode interactionKey = KeyCode.E;

    // Variáveis de controle interno
    private bool playerIsNearby = false;
    private SanidadeController playerSanityController;

    void Start()
    {
        // Garante que o canvas de interação comece o jogo desligado.
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(false);
        }
    }

    // Este método é chamado quando o jogador entra no Collider (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o jogador pela tag
        if (other.CompareTag("Player"))
        {
            // Ativa o flag de que o jogador está perto
            playerIsNearby = true;

            // Guarda a referência ao script de sanidade do jogador para uso posterior
            playerSanityController = other.GetComponent<SanidadeController>();

            // Mostra o canvas "Pressione E"
            if (interactionCanvas != null)
            {
                interactionCanvas.SetActive(true);
            }
        }
    }

    // Este método é chamado quando o jogador sai do Collider (Trigger)
    private void OnTriggerExit(Collider other)
    {
        // Verifica se é o jogador pela tag
        if (other.CompareTag("Player"))
        {
            // Desativa o flag, pois o jogador se afastou
            playerIsNearby = false;

            // Limpa a referência ao script de sanidade
            playerSanityController = null;

            // Esconde o canvas "Pressione E"
            if (interactionCanvas != null)
            {
                interactionCanvas.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Esta verificação acontece a todo frame:
        // 1. O jogador está perto?
        // 2. O jogador pressionou a tecla de interação?
        // 3. A referência ao script de sanidade é válida?
        if (playerIsNearby && Input.GetKeyDown(interactionKey) && playerSanityController != null)
        {
            // Se todas as condições são verdadeiras, chama a função de recuperar sanidade.
            playerSanityController.RecuperarSanidade();

            // A pílula agora simplesmente se destrói, pois a responsabilidade de criar
            // uma nova leva é do BatchPillSpawner, que é chamado por outro evento do jogo.
            Destroy(gameObject);
        }
    }
}