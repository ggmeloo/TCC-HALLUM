using UnityEngine;

public class InimigoDesaparece : MonoBehaviour
{
    // Este método é chamado quando um Collider "Is Trigger"
    // entra em contato com outro Collider.
    private void OnTriggerEnter2D(Collider2D other) // Para jogos 2D
    // private void OnTriggerEnter(Collider other) // Para jogos 3D
    {
        // Verifica se o objeto com o qual colidimos tem a Tag "Player"
        if (other.CompareTag("Player"))
        {
            // O inimigo "some" ou é "destruído"
            // Você pode desativá-lo ou destruí-lo completamente.

            // Opção 1: Desativar o GameObject (o objeto existe, mas não é renderizado nem atualizado)
            gameObject.SetActive(false);

            // Opção 2: Destruir o GameObject (remove-o completamente da cena)
            // Destroy(gameObject);

            Debug.Log("Inimigo colidiu com o Player e desapareceu!");
        }
    }
}