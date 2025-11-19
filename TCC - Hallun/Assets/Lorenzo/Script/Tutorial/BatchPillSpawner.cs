using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BatchPillSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    [Tooltip("Arraste o GameObject da sua pílula 'molde' que está na cena para este campo.")]
    public GameObject pillTemplateObject;

    [Tooltip("TODOS os GameObjects que marcam os locais possíveis de spawn.")]
    public List<Transform> spawnPoints;

    [Tooltip("Quantas pílulas devem aparecer a cada rodada?")]
    [Range(1, 50)]
    public int numberOfPillsToSpawn = 3;

    public static BatchPillSpawner instance;

    private List<GameObject> activePills = new List<GameObject>();
    private List<Transform> lastUsedSpawnPoints = new List<Transform>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Esta verificação é importante para evitar erros durante o jogo.
        if (pillTemplateObject == null || spawnPoints.Count == 0)
        {
            Debug.LogError("ERRO: O Objeto Molde (Pill Template) ou a lista de Spawn Points não foi configurada no BatchPillSpawner!");
            return;
        }

        // Desativa o molde original para que ele sirva apenas como template.
        pillTemplateObject.SetActive(false);

        // O spawner espera o comando do SanidadeController. Nenhuma pílula é criada aqui.
    }

    // Esta é a função que o SanidadeController chama para iniciar o spawn.
    public void SpawnNewBatch()
    {
        foreach (GameObject pill in activePills)
        {
            if (pill != null)
            {
                Destroy(pill);
            }
        }
        activePills.Clear();

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
            Transform chosenPoint = availablePoints[randomIndex];

            chosenPointsThisRound.Add(chosenPoint);
            availablePoints.RemoveAt(randomIndex);
        }

        foreach (Transform point in chosenPointsThisRound)
        {
            GameObject newPill = Instantiate(pillTemplateObject, point.position, point.rotation);
            newPill.SetActive(true);
            activePills.Add(newPill);
        }

        lastUsedSpawnPoints = new List<Transform>(chosenPointsThisRound);
        Debug.Log($"{chosenPointsThisRound.Count} pílulas de gameplay foram criadas.");
    }
}