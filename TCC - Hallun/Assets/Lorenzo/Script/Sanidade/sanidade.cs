using UnityEngine;
using UnityEngine.UI; // Necessário para componentes de UI como Image

// Garante que o objeto terá um Collider, que é essencial para o clique funcionar
[RequireComponent(typeof(Collider))]
public class ControladorDePainelPorClique : MonoBehaviour
{
    public enum Acao { Mostrar, Esconder, Alternar }

    [Header("Configuração do Painel")]
    [Tooltip("Arraste para cá o painel (ou qualquer objeto da UI) que você quer controlar.")]
    public GameObject painelAlvo;

    [Tooltip("O que deve acontecer quando o objeto for clicado?")]
    public Acao acaoAoClicar = Acao.Alternar;

    [Header("Configuração da Transição")]
    [Tooltip("A velocidade com que o painel aparece ou desaparece.")]
    public float velocidadeTransicao = 3f;

    [Tooltip("A opacidade máxima que o painel atingirá ao ser mostrado (1 = 100% visível).")]
    [Range(0, 1)]
    public float opacidadeMaxima = 1f;

    // Referência para o componente CanvasGroup do painel
    private CanvasGroup canvasGroupAlvo;
    private float alfaAlvo; // O valor de transparência que queremos atingir (0 ou opacidadeMaxima)

    void Awake()
    {
        // Validação inicial para evitar erros
        if (painelAlvo == null)
        {
            Debug.LogError($"O objeto '{gameObject.name}' não tem um 'Painel Alvo' configurado!", this);
            this.enabled = false; // Desativa o script para não causar mais erros
            return;
        }

        // Pega o componente CanvasGroup do painel. Se não existir, adiciona um.
        // Isso torna o script muito mais fácil de usar!
        canvasGroupAlvo = painelAlvo.GetComponent<CanvasGroup>();
        if (canvasGroupAlvo == null)
        {
            canvasGroupAlvo = painelAlvo.AddComponent<CanvasGroup>();
        }

        // Define o estado inicial baseado se o painel está visível ou não
        alfaAlvo = canvasGroupAlvo.alpha;
    }

    // Este método é chamado automaticamente pela Unity quando o Collider do objeto é clicado
    private void OnMouseDown()
    {
        // Executa a ação configurada no Inspector
        switch (acaoAoClicar)
        {
            case Acao.Mostrar:
                alfaAlvo = opacidadeMaxima;
                break;
            case Acao.Esconder:
                alfaAlvo = 0f;
                break;
            case Acao.Alternar:
                // Se já estiver visível, o alvo é ficar invisível, e vice-versa
                alfaAlvo = (alfaAlvo > 0) ? 0f : opacidadeMaxima;
                break;
        }

        Debug.Log($"Objeto '{gameObject.name}' clicado. Alfa alvo do painel definido para: {alfaAlvo}");
    }

    void Update()
    {
        // Se o alfa atual for diferente do alfa alvo, faz a transição suave
        if (canvasGroupAlvo.alpha != alfaAlvo)
        {
            // Usa Mathf.Lerp para mover o valor atual em direção ao alvo suavemente
            canvasGroupAlvo.alpha = Mathf.Lerp(canvasGroupAlvo.alpha, alfaAlvo, velocidadeTransicao * Time.deltaTime);

            // Pequena otimização para "cravar" o valor quando estiver muito próximo
            if (Mathf.Abs(canvasGroupAlvo.alpha - alfaAlvo) < 0.01f)
            {
                canvasGroupAlvo.alpha = alfaAlvo;
            }
        }

        // Opcional: Controla se o painel pode ser interagido
        // Se o painel estiver totalmente transparente, desativa a interação com botões, etc.
        canvasGroupAlvo.interactable = canvasGroupAlvo.alpha > 0;
        canvasGroupAlvo.blocksRaycasts = canvasGroupAlvo.alpha > 0;
    }
}