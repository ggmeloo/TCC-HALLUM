using UnityEngine;

public class Teleportador : MonoBehaviour
{
    // Arraste o GameObject de destino para este campo no Inspector.
    // Pode ser outro bloco visível ou um "Empty GameObject" posicionado onde você quer que o jogador apareça.
    [Tooltip("O ponto de destino para onde o jogador será teletransportado.")]
    public Transform pontoDeDestino;

    // --- Ponto de Melhoria Opcional ---
    // Se o seu jogador for um CharacterController, ele pode ter problemas com a mudança brusca de posição.
    // Descomente as linhas abaixo e a seção no código se estiver usando um CharacterController.
    // private bool teleportouRecentemente = false;


    /// <summary>
    /// Esta função é chamada pela Unity quando outro Collider entra neste trigger.
    /// Certifique-se de que o Collider deste objeto está marcado como "Is Trigger".
    /// </summary>
    /// <param name="other">O Collider do objeto que entrou no trigger (no caso, o jogador).</param>
    private void OnTriggerEnter(Collider other)
    {
        // 1. VERIFICAR SE O DESTINO FOI DEFINIDO
        // Se o pontoDeDestino não foi arrastado no Inspector, mostra um aviso e não faz nada.
        if (pontoDeDestino == null)
        {
            Debug.LogWarning("Ponto de destino do teletransportador não foi definido!", this.gameObject);
            return;
        }

        // 2. VERIFICAR SE QUEM ENTROU FOI O JOGADOR
        // É uma boa prática usar Tags para identificar o jogador.
        if (other.CompareTag("Player"))
        {
            // Se estiver usando um CharacterController, descomente a linha abaixo.
            // StartCoroutine(TeleportarJogador(other.gameObject));

            // Se estiver usando um Rigidbody, a linha abaixo é suficiente. Comente-a se for usar a Coroutine.
            TeleportarComRigidbody(other.gameObject);
        }
    }

    /// <summary>
    /// Método padrão para teletransportar objetos com Rigidbody.
    /// </summary>
    private void TeleportarComRigidbody(GameObject jogador)
    {
        Debug.Log($"Jogador tocou em {this.name}, teleportando para {pontoDeDestino.name}.");

        // Simplesmente move o jogador para a posição do destino.
        jogador.transform.position = pontoDeDestino.position;

        // Opcional, mas recomendado: Alinha a rotação do jogador com a rotação do ponto de destino.
        // Assim, você pode definir para onde o jogador estará olhando ao sair do teleporte.
        jogador.transform.rotation = pontoDeDestino.rotation;
    }

    /*
    // --- FUNÇÃO AVANÇADA PARA CHARACTER CONTROLLER ---
    // O CharacterController da Unity pode ignorar a mudança de 'transform.position' em um único frame.
    // Usamos uma Coroutine para desativá-lo, mover e reativá-lo, garantindo que o teleporte funcione.
    private System.Collections.IEnumerator TeleportarJogador(GameObject jogador)
    {
        if (teleportouRecentemente) yield break; // Impede teletransporte duplo

        teleportouRecentemente = true;

        CharacterController controller = jogador.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false; // Desativa o controller
        }

        // Move o jogador
        jogador.transform.position = pontoDeDestino.position;
        jogador.transform.rotation = pontoDeDestino.rotation;

        // Espera um pequeno instante para a engine processar a mudança
        yield return new WaitForEndOfFrame();

        if (controller != null)
        {
            controller.enabled = true; // Reativa o controller
        }
        
        // Previne que o jogador entre em um loop de teleporte infinito entre dois portais
        yield return new WaitForSeconds(0.5f);
        teleportouRecentemente = false;
    }
    */

}