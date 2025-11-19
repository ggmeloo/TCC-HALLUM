// Exemplo de script para o objeto da Pílula (PillPickup.cs)
using UnityEngine;

public class PillPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // LOG 1: Este é o primeiro teste. Ele nos diz se a colisão aconteceu.
        Debug.Log("OnTriggerEnter ativado por: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            // LOG 2: Se esta mensagem aparecer, a Tag do jogador está correta.
            Debug.Log("O objeto tem a tag 'Player'. Verificando o SanidadeController...");

            SanidadeController sanidade = other.GetComponent<SanidadeController>();

            if (sanidade != null)
            {
                // LOG 3: Se esta mensagem aparecer, o script SanidadeController foi encontrado.
                Debug.Log("SanidadeController encontrado! Chamando a função para recuperar sanidade...");

                sanidade.RecuperarSanidade();
                Destroy(gameObject);
            }
            else
            {
                // LOG 4: Se ESTA mensagem aparecer, o script não foi encontrado no objeto do jogador.
                Debug.LogError("ERRO: O objeto com a tag 'Player' NÃO tem o script 'SanidadeController' nele!");
            }
        }
    }
}