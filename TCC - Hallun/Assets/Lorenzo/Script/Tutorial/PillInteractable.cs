using UnityEngine;

// Lembre-se, este script deve estar no seu objeto "molde" da pílula (Pill Template).
public class PillInteractable : MonoBehaviour
{
    [Header("Configuração da Interação")]
    [Tooltip("Arraste o Canvas com o texto 'Pressione E' para este campo.")]
    public GameObject interactionCanvas;

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

    // A função Update modificada
    void Update()
    {
        // Esta verificação acontece a todo frame:
        // 1. O jogador está perto?
        // 2. O jogador pressionou a tecla de interação?
        // 3. A referência ao script de sanidade é válida?
        if (playerIsNearby && Input.GetKeyDown(interactionKey) && playerSanityController != null)
        {
            // Dá a sanidade para o jogador.
            playerSanityController.RecuperarSanidade();

            // Avisa ao spawner que uma pílula foi coletada.
            if (BatchPillSpawner.instance != null)
            {
                BatchPillSpawner.instance.PillWasCollected();
            }

            // Destrói o objeto da pílula, pois ela foi coletada.
            Destroy(gameObject);
        }
    }
} // <--- Esta chave final é crucial. Ela fecha a classe "PillInteractable".