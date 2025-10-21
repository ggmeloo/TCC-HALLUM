using UnityEngine;

public class IconLookAtCamera : MonoBehaviour
{
    // Referência para a câmera principal do jogador.
    private Camera mainCamera;

    void Start()
    {
        // Encontra e armazena a referência da câmera principal no início do jogo.
        // Isso é mais eficiente do que procurar a câmera a cada frame.
        mainCamera = Camera.main;

        // Adiciona um aviso no console caso a câmera não seja encontrada.
        // Isso geralmente acontece se a câmera não tiver a tag "MainCamera".
        if (mainCamera == null)
        {
            Debug.LogError("Erro: Nenhuma câmera com a tag 'MainCamera' foi encontrada na cena. O ícone não saberá para onde olhar.", this.gameObject);
        }
    }

    // Usamos LateUpdate para garantir que a rotação do ícone aconteça
    // DEPOIS que a câmera do jogador já se moveu naquele frame.
    // Isso evita qualquer tremor ou atraso visual (jitter).
    void LateUpdate()
    {
        // Se não tivermos uma referência da câmera, não faz nada.
        if (mainCamera == null) return;

        // Esta é a linha mágica. Ela calcula a rotação necessária para que
        // a frente (forward) deste objeto aponte diretamente para a câmera.
        // O segundo argumento (mainCamera.transform.up) garante que o ícone não vire de cabeça para baixo.
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.up);
    }
}