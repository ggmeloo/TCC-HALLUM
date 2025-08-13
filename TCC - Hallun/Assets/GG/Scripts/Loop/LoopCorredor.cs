using UnityEngine;

public class LoopCorredor : MonoBehaviour
{
    public Transform destinoDoLoop;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o jogador
        if (other.CompareTag("Player"))
        {
            // Verifica se o destino está configurado
            if (destinoDoLoop != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Zera a velocidade para um teleporte limpo
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    // AÇÃO PRINCIPAL - Move o jogador e ajusta sua rotação
                    rb.MovePosition(destinoDoLoop.position);
                    rb.MoveRotation(destinoDoLoop.rotation); // Forma mais segura de rotacionar um Rigidbody

                    Debug.Log("Teleporte concluído. Jogador olhando na direção de 'DestinoDoLoop'");
                }
            }
            else
            {
                Debug.LogError("O DESTINO DO LOOP NÃO FOI DEFINIDO NO INSPETOR!");
            }
        }
    }
}