using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BatchPillSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    [Tooltip("Arraste o GameObject da sua pílula 'molde' que está na cena para este campo.")]
    public GameObject pillTemplateObject;

    [Tooltip("Todos os GameObjects que marcam os locais possíveis de spawn.")]
    public List<Transform> spawnPoints;

    [Tooltip("Quantas pílulas devem aparecer a cada rodada?")]
    public int numberOfPillsToSpawn = 3;

    public static BatchPillSpawner instance;

    // --- NOVAS VARIÁVEIS ---
    private int pillsRemainingInBatch; // Contador de pílulas restantes
    private List<Transform> lastUsedSpawnPoints = new List<Transform>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (pillTemplateObject == null || spawnPoints.Count == 0)
        {
            Debug.LogError("ERRO: O Objeto Molde (Pill Template) ou a lista de Spawn Points não foi configurada!");
            return;
        }
        pillTemplateObject.SetActive(false);
        // O spawner começa esperando o comando.
    }

    // A função principal para criar um novo lote
    public void SpawnNewBatch()
    {
        Debug.Log("Criando um novo lote de pílulas...");

        List<Transform> availablePoints = spawnPoints.Except(lastUsedSpawnPoints).ToList();
        if (availablePoints.Count < numberOfPillsToSpawn)
        {
            availablePoints = new List<Transform>(spawnPoints);
        }

        List<Transform> chosenPointsThisRound = new List<Transform>();
        for (int i = 0; i < numberOfPillsToSpawn; i++)
        {
            if (availablePoints.Count == 0) break;
            int randomIndex = Random.Range(0, availablePoints.Count);
            chosenPointsThisRound.Add(availablePoints[randomIndex]);
            availablePoints.RemoveAt(randomIndex);
        }

        foreach (Transform point in chosenPointsThisRound)
        {
            GameObject newPill = Instantiate(pillTemplateObject, point.position, point.rotation);
            newPill.SetActive(true);
        }

        // --- LÓGICA DE CONTAGEM ---
        // Define o número de pílulas que precisam ser coletadas nesta rodada
        pillsRemainingInBatch = chosenPointsThisRound.Count;

        lastUsedSpawnPoints = new List<Transform>(chosenPointsThisRound);
    }

    // --- NOVA FUNÇÃO CHAMADA PELA PÍLULA ---
    public void PillWasCollected()
    {
        // Diminui o contador
        pillsRemainingInBatch--;

        Debug.Log($"Pílula coletada! Restam {pillsRemainingInBatch} neste lote.");

        // Se o contador chegar a zero, é hora de criar um novo lote!
        if (pillsRemainingInBatch <= 0)
        {
            // Opcional: Adicionar um pequeno delay para a próxima rodada
            // Invoke("SpawnNewBatch", 2f); // Espera 2 segundos antes de criar o próximo lote

            // Ou criar imediatamente:
            SpawnNewBatch();
        }
    }
}