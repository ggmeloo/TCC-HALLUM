using UnityEngine;
using System.Collections.Generic;

public class PillSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    [Tooltip("Arraste o Prefab da sua pílula para este campo.")]
    public GameObject pillPrefab;

    // MUDANÇA AQUI: Agora usamos uma lista de SpawnZone.
    [Tooltip("Arraste aqui os objetos da cena que representam as ZONAS de spawn.")]
    public List<SpawnZone> spawnZones;

    public static PillSpawner instance;
    private SpawnZone lastSpawnZone;

    private void Awake()
    {
        // ----> ADICIONE ESTA LINHA AQUI <----
        Debug.Log("PILLSPAWNER ACORDOU E ESTÁ ATIVO!");

        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (pillPrefab == null)
        {
            Debug.LogError("ERRO: O Prefab da pílula não foi atribuído no PillSpawner!");
            return;
        }
        if (spawnZones.Count == 0)
        {
            Debug.LogError("ERRO: A lista de Spawn Zones está vazia no PillSpawner!");
            return;
        }

        SpawnPill();
    }

    public void SpawnPill()
    {
        List<SpawnZone> availableZones = new List<SpawnZone>(spawnZones);
        if (lastSpawnZone != null && spawnZones.Count > 1)
        {
            availableZones.Remove(lastSpawnZone);
        }

        int randomIndex = Random.Range(0, availableZones.Count);
        SpawnZone chosenZone = availableZones[randomIndex];

        // MUDANÇA AQUI: Pegamos um ponto aleatório DENTRO da zona escolhida.
        Vector3 spawnPosition = chosenZone.GetRandomPoint();

        // Combinando com a Rotação Aleatória do Nível 2 para o melhor efeito!
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        Instantiate(pillPrefab, spawnPosition, randomRotation);

        lastSpawnZone = chosenZone;
        Debug.Log($"Pílula criada na zona: {chosenZone.name}");
    }
}