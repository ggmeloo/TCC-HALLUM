using UnityEngine;

public class LoopCorredor : MonoBehaviour
{
    public Transform destinoDoLoop;

    // Adicionamos esta variável para garantir que o gatilho só dispare uma vez por passagem
    private bool jaFoiAtivado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !jaFoiAtivado)
        {
            // Marca como ativado para evitar múltiplos disparos
            jaFoiAtivado = true;

            if (destinoDoLoop != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // A lógica de teleporte permanece, como no seu script
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.MovePosition(destinoDoLoop.position);
                    rb.MoveRotation(destinoDoLoop.rotation);

                    // --- ESTA É A LINHA CORRIGIDA ---
                    // Agora ele chama o método pelo nome correto: "ProcessarMudancaDeVolta"
                    AnomalyManager.instance.ProcessarMudancaDeVolta();

                    Debug.Log("Teleporte concluído. Processando a próxima volta.");
                }
            }
            else
            {
                Debug.LogError("O DESTINO DO LOOP NÃO FOI DEFINIDO NO INSPETOR!");
            }
        }
    }

    // O AnomalyManager pode chamar este método para "rearmar" o gatilho.
    // Embora na lógica atual não seja estritamente necessário, é uma boa prática.
    public void ResetarGatilho()
    {
        jaFoiAtivado = false;
    }
}