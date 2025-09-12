using UnityEngine;
using System.Collections.Generic; // Necessário para Listas

public class DiagnosticoAnomalias : MonoBehaviour
{
    [Header("Arraste os seus containers aqui")]
    public GameObject anomaliasTutorialContainer;
    public GameObject anomaliasAleatoriasContainer;

    void Start()
    {
        Debug.LogWarning("--- INICIANDO DIAGNÓSTICO DE ANOMALIAS ---");

        // --- TESTE 1: CONTAINER DE TUTORIAL ---
        if (anomaliasTutorialContainer == null)
        {
            Debug.LogError("ERRO: O Container de Anomalias de TUTORIAL não foi arrastado para o Inspetor!");
        }
        else
        {
            // O '(true)' é crucial, pois força a busca em filhos inativos.
            Anomalia[] anomaliasFixas = anomaliasTutorialContainer.GetComponentsInChildren<Anomalia>(true);
            if (anomaliasFixas.Length > 0)
            {
                Debug.Log("SUCESSO: Encontradas " + anomaliasFixas.Length + " anomalias de TUTORIAL no container.");
            }
            else
            {
                Debug.LogError("ERRO: Nenhuma anomalia de TUTORIAL foi encontrada dentro do container '" + anomaliasTutorialContainer.name + "'. Verifique se os objetos filhos têm o script 'Anomalia'.");
            }
        }

        // --- TESTE 2: CONTAINER ALEATÓRIO ---
        if (anomaliasAleatoriasContainer == null)
        {
            Debug.LogError("ERRO: O Container de Anomalias ALEATÓRIAS não foi arrastado para o Inspetor!");
        }
        else
        {
            Anomalia[] anomaliasAleatorias = anomaliasAleatoriasContainer.GetComponentsInChildren<Anomalia>(true);
            if (anomaliasAleatorias.Length > 0)
            {
                Debug.Log("SUCESSO: Encontradas " + anomaliasAleatorias.Length + " anomalias ALEATÓRIAS no container.");
            }
            else
            {
                Debug.LogError("ERRO: Nenhuma anomalia ALEATÓRIA foi encontrada dentro do container '" + anomaliasAleatoriasContainer.name + "'. Verifique se os objetos filhos têm o script 'Anomalia'.");
            }
        }

        Debug.LogWarning("--- DIAGNÓSTICO CONCLUÍDO ---");
    }
}