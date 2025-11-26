using UnityEngine;
using UnityEngine.AI; // Necessário para a navegação (IA)
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class BossIA : MonoBehaviour
{
    [Header("--- Configurações Principais ---")]
    public Transform player; // Arraste o Player aqui
    public LayerMask camadaDoPlayer; // Layer do Player (para evitar ver através de paredes se quiser melhorar depois)

    [Header("--- Distâncias (Raios) ---")]
    [Tooltip("Distância para o Boss começar a ver e perseguir o player")]
    public float distanciaVisao = 20f;

    [Tooltip("Distância para o Boss parar de correr e atacar")]
    public float distanciaAtaque = 5f;

    [Tooltip("Distância para o Boss executar a animação de Morte (Fatality)")]
    public float distanciaMorte = 2f;

    [Header("--- Patrulha (Aleatório) ---")]
    public float raioDePatrulha = 10f; // O quanto ele anda aleatoriamente
    public float intervaloRugido = 10f; // Tempo entre rugidos

    [Header("--- Tempos e Durações ---")]
    [Tooltip("Tempo que o Boss espera entre um ataque e outro")]
    public float tempoEntreAtaques = 2f;

    [Tooltip("Quanto tempo dura a animação de matar antes de dar Game Over")]
    public float duracaoAnimacaoMorte = 2f;

    [Header("--- Sons ---")]
    public AudioClip somRugido;
    // Para passos, recomendo usar "Animation Events" na própria animação, mas deixei o AudioSource aqui.

    [Header("--- NOMES DAS ANIMAÇÕES (Animator) ---")]
    [Tooltip("Nome do parâmetro BOOL para andar")]
    public string nomeAnimacaoAndar = "EstaAndando";

    [Tooltip("Nome do parâmetro BOOL para correr")]
    public string nomeAnimacaoCorrer = "EstaCorrendo";

    [Tooltip("Nome do parâmetro TRIGGER para atacar")]
    public string nomeAnimacaoAtaque = "Atacar";

    [Tooltip("Nome do parâmetro TRIGGER para matar")]
    public string nomeAnimacaoMorte = "Matar";

    [Tooltip("Nome do parâmetro TRIGGER para rugir")]
    public string nomeAnimacaoRugido = "Rugir";

    // Variáveis internas (não aparecem no editor)
    private NavMeshAgent agente;
    private Animator animador;
    private AudioSource fonteAudio;
    private Vector3 pontoDestino;
    private bool destinoDefinido;
    private bool jaAtacou;
    private float cronometroRugido;
    private bool jogoAcabou = false;

    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animador = GetComponent<Animator>();
        fonteAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (player == null || jogoAcabou) return;

        // Calcula a distância entre o Boss e o Player
        float distanciaAtual = Vector3.Distance(transform.position, player.position);

        // Lógica de Estados (Decisão do Boss)
        if (distanciaAtual <= distanciaMorte)
        {
            EstadoMatarPlayer();
        }
        else if (distanciaAtual <= distanciaAtaque)
        {
            EstadoAtacarPlayer();
        }
        else if (distanciaAtual <= distanciaVisao)
        {
            EstadoPerseguirPlayer();
        }
        else
        {
            EstadoPatrulhar();
        }
    }

    // 1. ESTADO: PATRULHAR (Andar aleatório)
    private void EstadoPatrulhar()
    {
        if (!destinoDefinido) ProcurarPontoAleatorio();

        if (destinoDefinido)
        {
            agente.SetDestination(pontoDestino);

            // Define animações
            DefinirAnimacaoMovimento(true, false); // Andando = true, Correndo = false
        }

        // Verifica se chegou no ponto aleatório
        Vector3 distanciaDoPonto = transform.position - pontoDestino;
        if (distanciaDoPonto.magnitude < 1f)
            destinoDefinido = false;

        // Lógica do Rugido
        cronometroRugido += Time.deltaTime;
        if (cronometroRugido >= intervaloRugido)
        {
            TocarRugido();
            cronometroRugido = 0;
        }
    }

    private void ProcurarPontoAleatorio()
    {
        // Sorteia um ponto X e Z
        float randomZ = Random.Range(-raioDePatrulha, raioDePatrulha);
        float randomX = Random.Range(-raioDePatrulha, raioDePatrulha);

        pontoDestino = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Verifica se o ponto é válido no chão
        if (Physics.Raycast(pontoDestino, -transform.up, 2f, LayerMask.GetMask("Default", "Ground")))
            destinoDefinido = true;
    }

    // 2. ESTADO: PERSEGUIR
    private void EstadoPerseguirPlayer()
    {
        agente.SetDestination(player.position);

        // Muda para animação de correr
        DefinirAnimacaoMovimento(false, true); // Andando = false, Correndo = true
    }

    // 3. ESTADO: ATACAR
    private void EstadoAtacarPlayer()
    {
        // Para o boss para atacar
        agente.SetDestination(transform.position);
        transform.LookAt(player); // Olha para o player

        // Para animações de movimento
        DefinirAnimacaoMovimento(false, false);

        if (!jaAtacou)
        {
            // Ativa a animação com o nome que você escolheu
            animador.SetTrigger(nomeAnimacaoAtaque);

            // AQUI VOCÊ DARIA O DANO NO PLAYER
            Debug.Log("Boss atacou!");

            jaAtacou = true;
            // Chama o reset após o tempo definido
            Invoke(nameof(ResetarAtaque), tempoEntreAtaques);
        }
    }

    private void ResetarAtaque()
    {
        jaAtacou = false;
    }

    // 4. ESTADO: MATAR (FATALITY)
    private void EstadoMatarPlayer()
    {
        agente.SetDestination(transform.position);
        transform.LookAt(player);
        DefinirAnimacaoMovimento(false, false);

        if (!jaAtacou)
        {
            Debug.Log("Executando KILL!");
            animador.SetTrigger(nomeAnimacaoMorte);

            jaAtacou = true;
            jogoAcabou = true; // Impede que ele continue fazendo outras coisas

            StartCoroutine(FinalizarJogo());
        }
    }

    IEnumerator FinalizarJogo()
    {
        // Espera o tempo da animação que você definiu
        yield return new WaitForSeconds(duracaoAnimacaoMorte);

        Debug.Log("FIM DE JOGO - Player Morto");
        // Exemplo: SceneManager.LoadScene("GameOver");
    }

    // Funções Auxiliares
    private void DefinirAnimacaoMovimento(bool andar, bool correr)
    {
        // Só define se os nomes não estiverem vazios
        if (!string.IsNullOrEmpty(nomeAnimacaoAndar)) animador.SetBool(nomeAnimacaoAndar, andar);
        if (!string.IsNullOrEmpty(nomeAnimacaoCorrer)) animador.SetBool(nomeAnimacaoCorrer, correr);
    }

    private void TocarRugido()
    {
        if (somRugido && !fonteAudio.isPlaying)
        {
            fonteAudio.PlayOneShot(somRugido);
            if (!string.IsNullOrEmpty(nomeAnimacaoRugido))
                animador.SetTrigger(nomeAnimacaoRugido);
        }
    }

    // Desenha as linhas coloridas no Editor para ajudar a ver as distâncias
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVisao); // Amarelo = Visão

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque); // Vermelho = Ataque

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, distanciaMorte); // Preto = Morte
    }
}