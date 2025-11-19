// Lembre-se desta linha para usar o DoTween!
using DG.Tweening;
using UnityEngine;

public class PapeisEmDirecaoAoPlayer : MonoBehaviour
{
    [Header("Configuração da Animação")]
    [Tooltip("A altura inicial que os papéis vão subir da mesa antes de avançar.")]
    public float alturaLevitacaoInicial = 0.5f;

    [Tooltip("A que distância da câmera os papéis vão parar (movimento para frente).")]
    public float distanciaDoPlayer = -1.5f;

    [Space]

    [Tooltip("Tempo total da animação do início ao fim.")]
    public float duracaoTotalAnimacao = 2.5f;

    [Header("Referências")]
    [Tooltip("Arraste o Canvas do tutorial para cá.")]
    public GameObject canvasDoTutorial;

    private bool podeInteragir = false;
    private bool jaInteragiu = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !jaInteragiu) podeInteragir = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) podeInteragir = false;
    }

    void Update()
    {
        if (podeInteragir && Input.GetKeyDown(KeyCode.E) && !jaInteragiu)
        {
            jaInteragiu = true;
            IniciarAnimacao();
        }
    }

    void IniciarAnimacao()
    {
        // 1. Encontra a câmera principal do jogador no momento da interação.
        // Camera.main é uma forma rápida de encontrar a câmera com a tag "MainCamera".
        Transform cameraTransform = Camera.main.transform;

        // 2. Calcula a Posição Final.
        // Começa na posição da câmera e avança na direção que a câmera está olhando.
        Vector3 posicaoFinal = cameraTransform.position + cameraTransform.forward * distanciaDoPlayer;

        // 3. Calcula a Rotação Final.
        // Queremos que os papéis encarem a câmera.
        Quaternion rotacaoFinal = Quaternion.LookRotation(transform.position - cameraTransform.position);

        // --- A SEQUÊNCIA DE ANIMAÇÃO COM DOTWEEN ---
        Sequence minhaSequencia = DOTween.Sequence();

        // Etapa A: Levitar um pouco para cima.
        // Usamos Append para adicionar esta animação ao início da sequência.
        minhaSequencia.Append(transform.DOMoveY(transform.position.y + alturaLevitacaoInicial, duracaoTotalAnimacao / 3).SetEase(Ease.OutSine));

        // Etapa B: Mover e Rotacionar em direção ao jogador ao mesmo tempo.
        // Usamos Append de novo, então esta etapa começa APÓS a levitação terminar.
        // DOMove e DORotateQuaternion são adicionados juntos para acontecerem simultaneamente.
        minhaSequencia.Append(transform.DOMove(posicaoFinal, duracaoTotalAnimacao * 2 / 3).SetEase(Ease.InOutSine));
        minhaSequencia.Join(transform.DORotateQuaternion(rotacaoFinal, duracaoTotalAnimacao * 2 / 3));

        // OnComplete() é chamado QUANDO TODA a sequência (Levitação + Movimento/Rotação) terminar.
        minhaSequencia.OnComplete(() => {
            // Ativa a UI do tutorial.
            canvasDoTutorial.SetActive(true);

            // Desativa o objeto 3D dos papéis.
            gameObject.SetActive(false);
        });
    }
}