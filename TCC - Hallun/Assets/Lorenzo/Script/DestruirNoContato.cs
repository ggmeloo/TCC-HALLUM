using UnityEngine;

public class DestruirNoContato : MonoBehaviour
{
    // Esta função é chamada quando este objeto colide com outro
    private void OnCollisionEnter(Collision colisão)
    {
        // Verifica se o objeto com o qual colidimos é o 'Player'
        if (colisão.gameObject.CompareTag("Player"))
        {
            // Se for o Player, destrói este objeto (o inimigo)
            Destroy(gameObject);
        }
    }
}