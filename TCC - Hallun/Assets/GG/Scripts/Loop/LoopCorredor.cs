using UnityEngine;

public class LoopCorredor : MonoBehaviour
{
    public Transform destinoDoLoop;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se � o jogador
        if (other.CompareTag("Player"))
        {
            // Verifica se o destino est� configurado
            if (destinoDoLoop != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Zera a velocidade para um teleporte limpo
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    // A��O PRINCIPAL - Move o jogador e ajusta sua rota��o
                    rb.MovePosition(destinoDoLoop.position);
                    rb.MoveRotation(destinoDoLoop.rotation); // Forma mais segura de rotacionar um Rigidbody

                    Debug.Log("Teleporte conclu�do. Jogador olhando na dire��o de 'DestinoDoLoop'");
                }
            }
            else
            {
                Debug.LogError("O DESTINO DO LOOP N�O FOI DEFINIDO NO INSPETOR!");
            }
        }
    }
}