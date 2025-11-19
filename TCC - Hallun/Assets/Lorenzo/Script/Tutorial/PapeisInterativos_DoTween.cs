// Lembre-se de adicionar esta linha no topo para usar o DoTween!
using DG.Tweening;
using UnityEngine;

public class PapeisInterativos_DoTween : MonoBehaviour
{
    [Header("Configuração da Animação")]
    [Tooltip("Quanto tempo a animação de subida/rotação deve levar.")]
    public float duracaoAnimacao = 2.0f;

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

            // Inicia a sequência de animação com DoTween
            IniciarAnimacao();
        }
    }

    void IniciarAnimacao()
    {
        // Posição alvo: um pouco na frente da câmera do jogador
        Transform cameraTransform = Camera.main.transform;
        Vector3 posicaoAlvo = cameraTransform.position + cameraTransform.forward * 2f;

        // Cria uma sequência de animação. Pense nisso como uma Timeline no código.
        Sequence minhaSequencia = DOTween.Sequence();

        // 1. Adiciona o movimento e a rotação para acontecerem AO MESMO TEMPO.
        minhaSequencia.Append(transform.DOMove(posicaoAlvo, duracaoAnimacao));
        minhaSequencia.Join(transform.DORotate(new Vector3(90, 0, 0), duracaoAnimacao));

        // 2. Adiciona o aumento de escala para acontecer DEPOIS do movimento.
        minhaSequencia.Append(transform.DOScale(Vector3.zero, 0.5f)); // Encolhe até sumir

        // 3. Define o que acontece QUANDO A SEQUÊNCIA INTEIRA TERMINAR.
        minhaSequencia.OnComplete(() => {
            // Ativa a UI do tutorial
            canvasDoTutorial.SetActive(true);

            // Desativa o objeto 3D dos papéis para sempre
            gameObject.SetActive(false);
        });
    }
}