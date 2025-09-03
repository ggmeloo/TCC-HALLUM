using UnityEngine;

/// <summary>
/// Controla o comportamento de um inimigo que inicia em um estado passivo (chorando)
/// e, ao ser ativado, persegue um alvo de forma agressiva.
/// </summary>
public class InimigoPerseguidor3D : MonoBehaviour
{
    [Header("Referências de Componentes")]
    [Tooltip("O componente Animator do modelo do inimigo.")]
    public Animator animadorInimigo;

    [Tooltip("O componente AudioSource que contém o som de choro.")]
    public AudioSource audioChoro;

    [Tooltip("O alvo que o inimigo irá perseguir. Geralmente, o jogador.")]
    public Transform alvo;

    [Header("Configurações da Perseguição")]
    [Tooltip("A velocidade de movimento do inimigo quando estiver perseguindo.")]
    public float velocidadePerseguicao = 5f;

    // Variável interna para controlar o estado do inimigo
    private bool estaPerseguindo = false;

    void Start()
    {
        // Garante que o inimigo comece no estado de choro, se o áudio estiver configurado
        if (audioChoro != null && !audioChoro.isPlaying)
        {
            audioChoro.Play();
        }
    }

    void Update()
    {
        // Só executa a lógica de perseguição se o estado 'estaPerseguindo' for verdadeiro
        if (estaPerseguindo && alvo != null)
        {
            PerseguirAlvo();
        }
    }

    /// <summary>
    /// Esta é a função pública que ativa o modo de perseguição.
    /// Ela deve ser chamada por um outro script, como um gatilho no cenário.
    /// </summary>
    public void AtivarPerseguicao()
    {
        // Verificação de segurança para garantir que a ativação só ocorra uma vez
        if (estaPerseguindo) return;

        estaPerseguindo = true;

        // 1. Para o som de choro, se existir
        if (audioChoro != null && audioChoro.isPlaying)
        {
            audioChoro.Stop();
        }

        // 2. Aciona o gatilho no Animator para mudar para a animação de corrida
        if (animadorInimigo != null)
        {
            // Certifique-se de ter um parâmetro do tipo Trigger chamado "IniciarPerseguicao" no seu Animator Controller
            animadorInimigo.SetTrigger("IniciarPerseguicao");
        }

        // Mensagem de depuração para confirmar que o inimigo foi ativado
        Debug.Log(gameObject.name + " foi ativado e está perseguindo o jogador!");
    }

    /// <summary>
    /// Gerencia a lógica de rotação e movimento em direção ao alvo.
    /// </summary>
    private void PerseguirAlvo()
    {
        // --- ROTAÇÃO ---
        // Calcula a direção para olhar para o alvo, ignorando o eixo Y para evitar que o inimigo se incline
        Vector3 direcaoParaOlhar = new Vector3(alvo.position.x, transform.position.y, alvo.position.z);
        transform.LookAt(direcaoParaOlhar);

        // --- MOVIMENTO ---
        // Move o inimigo em direção à posição exata do alvo de forma suave
        transform.position = Vector3.MoveTowards(transform.position, alvo.position, velocidadePerseguicao * Time.deltaTime);
    }
}