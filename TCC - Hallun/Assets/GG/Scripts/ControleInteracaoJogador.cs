using System.Collections;
using UnityEngine;

public class ControleInteracaoJogador : MonoBehaviour
{
    private Rigidbody rb;

    // Flag para evitar que múltiplos teleports sejam acionados ao mesmo tempo.
    private bool isTeleporting = false;
    public GameObject[] positionsDestination;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Só executa se não estivermos já no meio de um teleporte.
        if (!isTeleporting && other.CompareTag("GatilhoDeTeleporte"))
        {
            PontoDeTeleporteInfo info = other.GetComponent<PontoDeTeleporteInfo>();
            int pos = info.destinationNumber;

            Debug.Log(pos);
            if (info != null)
            {
                // Inicia o processo de teleporte usando a corrotina.
                StartCoroutine(TeleportRoutine(positionsDestination[pos].transform.position, positionsDestination[pos].transform.rotation));
            }
        }
    }

    // IEnumerator marca esta função como uma Corrotina.
    // Ela pode ser "pausada" e "retomada".
    private IEnumerator TeleportRoutine(Vector3 destinationPosition, Quaternion destinationRotation)
    {
        // 1. Marca que estamos teleportando para evitar bugs.
        isTeleporting = true;

        // 2. A LINHA MAIS IMPORTANTE: Pausa a função e espera pelo próximo passo da simulação de física.
        // Isso dá tempo para o OnTriggerEnter terminar completamente.
        yield return new WaitForFixedUpdate();

        // 3. AGORA, com o motor de física pronto, executamos o teleporte.
        if (rb != null)
        {
            transform.localPosition = destinationPosition;
            transform.localRotation = destinationRotation;
        }
        else // Fallback caso não haja Rigidbody (não deveria acontecer, mas é seguro).
        {
            transform.localPosition = destinationPosition;
            transform.localRotation = destinationRotation;
        }

        Debug.Log("Teleporte executado para: " + destinationPosition);

        // 4. Libera a flag para que possamos teleportar novamente no futuro.
        isTeleporting = false;
    }
}