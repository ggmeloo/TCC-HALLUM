using UnityEngine;

public class ItemPilula : MonoBehaviour
{
    // Variável para encontrar o script do jogador.
    // Você pode arrastar o objeto do Jogador aqui pelo Inspector
    // ou deixar que o script o encontre automaticamente.
    public SanidadeController sanidadeController;

    // Este método é chamado quando outro Collider entra no trigger.
    // Certifique-se de que sua pílula tenha um Collider (como BoxCollider)
    // e que a opção "Is Trigger" esteja marcada.
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é o Jogador (pela tag).
        if (other.CompareTag("Player"))
        {
            // Tenta encontrar o SanidadeController no objeto do jogador, se não foi atribuído.
            if (sanidadeController == null)
            {
                sanidadeController = other.GetComponent<SanidadeController>();
            }

            // Se encontrou o controller, chama a função para recuperar a sanidade.
            if (sanidadeController != null)
            {
                Debug.Log("Jogador pegou a pílula. Recuperando sanidade.");
                sanidadeController.RecuperarSanidade();

                // Destrói o objeto da pílula após o uso.
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Não foi possível encontrar o SanidadeController no jogador!");
            }
        }
    }
}