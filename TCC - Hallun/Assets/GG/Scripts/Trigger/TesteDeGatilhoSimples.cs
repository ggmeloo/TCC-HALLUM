using UnityEngine;

public class TesteDeGatilhoSimples : MonoBehaviour
{
    // Esta função é chamada pelo motor da Unity sempre que um colisor entra neste gatilho.
    private void OnTriggerEnter(Collider other)
    {
        // Ele vai imprimir o nome do objeto que colidiu e o nome deste gatilho.
        // Não há 'if', não há verificação de tag, não há nada que possa falhar.
        // Se a física estiver funcionando, esta mensagem VAI aparecer.
        Debug.LogWarning("!!! COLISÃO DETECTADA !!! Objeto '" + other.name + "' entrou no gatilho '" + this.name + "'");
    }
}