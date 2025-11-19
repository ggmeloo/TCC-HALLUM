using UnityEngine;

// Garante que este script só possa ser adicionado a um objeto que tenha um BoxCollider.
[RequireComponent(typeof(BoxCollider))]
public class SpawnZone : MonoBehaviour
{
    private BoxCollider area;

    private void Awake()
    {
        // Pega a referência do BoxCollider no mesmo objeto.
        area = GetComponent<BoxCollider>();
    }

    // Esta função pública retorna um ponto aleatório dentro dos limites do BoxCollider.
    public Vector3 GetRandomPoint()
    {
        // Pega os limites (bounds) do collider no espaço do mundo.
        Bounds bounds = area.bounds;

        // Calcula um ponto aleatório dentro desses limites.
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}