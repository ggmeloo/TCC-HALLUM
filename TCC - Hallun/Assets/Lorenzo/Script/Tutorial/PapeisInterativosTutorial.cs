using DG.Tweening;
using UnityEngine;

public class PapeisInterativosTutorial : MonoBehaviour
{
    [Header("UI de Interação")]
    [Tooltip("Arraste o Canvas/Painel que mostra 'Pressione E' para este campo.")]
    public GameObject interactionCanvas;

    [Header("Alvo da Animação")]
    public Transform alvoDePosicao;
    public Vector3 rotacaoFinalEmGraus;

    [Header("Configuração da Animação")]
    public float alturaLevitacao = 0.5f;
    public float duracaoLevitacao = 0.75f;
    public float duracaoMovimentoFinal = 1.5f;

    [Header("Referências da UI de Tutorial")]
    public GameObject canvasDoTutorial;

    // Variáveis de controle de estado
    private bool podeInteragir = false;
    private bool tutorialEstaAberto = false;

    // Guarda a posição e rotação inicial para poder resetar
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;

    void Start()
    {
        // Salva o estado inicial do objeto
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;

        // Garante que a UI de interação "Pressione E" comece ativa
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(true);
        }
    }

    // Usamos OnTrigger para saber se o player está perto o suficiente
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) podeInteragir = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) podeInteragir = false;
    }

    void Update()
    {
        // Só podemos interagir se o player estiver perto E o tutorial não estiver aberto
        if (podeInteragir && Input.GetKeyDown(KeyCode.E) && !tutorialEstaAberto)
        {
            tutorialEstaAberto = true; // Trava para não interagir de novo
            interactionCanvas.SetActive(false); // Some com o "E"
            IniciarAnimacao();
        }
    }

    void IniciarAnimacao()
    {
        Vector3 posicaoLevitando = transform.position + Vector3.up * alturaLevitacao;
        Vector3 posicaoFinal = alvoDePosicao.position;
        Quaternion rotacaoFinal = Quaternion.Euler(rotacaoFinalEmGraus);

        Sequence minhaSequencia = DOTween.Sequence();
        minhaSequencia.Append(transform.DOMove(posicaoLevitando, duracaoLevitacao));
        minhaSequencia.Append(transform.DOMove(posicaoFinal, duracaoMovimentoFinal));
        minhaSequencia.Join(transform.DORotateQuaternion(rotacaoFinal, duracaoMovimentoFinal));

        minhaSequencia.OnComplete(() => {
            canvasDoTutorial.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // O objeto 3D dos papéis agora fica INVISÍVEL, mas não desativado
            GetComponent<MeshRenderer>().enabled = false;
            // Se seus papéis tiverem muitos filhos, desative o objeto pai
            // gameObject.SetActive(false); // Você pode usar este se o reset for feito pelo outro script
        });
    }

    // ESTA É A FUNÇÃO MÁGICA QUE O PAINEL DO TUTORIAL VAI CHAMAR
    public void ResetarInteracao()
    {
        // Reseta o estado para permitir nova interação
        tutorialEstaAberto = false;

        // Mostra o "Pressione E" novamente
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(true);
        }

        // Devolve os papéis 3D à sua posição e rotação original
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
        GetComponent<MeshRenderer>().enabled = true; // Torna os papéis visíveis de novo
        // Ou, se você desativou o objeto:
        // gameObject.SetActive(true);
    }
}