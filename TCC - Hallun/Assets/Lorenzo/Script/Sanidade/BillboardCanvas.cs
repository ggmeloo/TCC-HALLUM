using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Encontra a câmera principal da cena e a guarda para usar depois.
        // Fazer isso no Start é mais eficiente do que procurar a cada frame.
        mainCamera = Camera.main;
    }

    // LateUpdate é chamado depois de todos os outros Updates.
    // É o melhor lugar para fazer ajustes de câmera e UI que segue a câmera,
    // pois garante que a câmera já terminou de se mover naquele frame.
    void LateUpdate()
    {
        // Se a câmera foi encontrada...
        if (mainCamera != null)
        {
            // ...faz este objeto (o Canvas) ter exatamente a mesma rotação da câmera.
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}