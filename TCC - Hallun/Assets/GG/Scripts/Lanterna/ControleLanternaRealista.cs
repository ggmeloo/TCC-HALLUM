using UnityEngine;

public class ControleLanternaRealista : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o Rigidbody do seu Player para cá.")]
    public Rigidbody playerRigidbody;
    [Tooltip("Arraste a Câmera Principal do seu jogador para cá.")]
    public Transform cameraTransform; // Referência para a câmera que vamos seguir

    [Header("Movimento Realista")]
    [Tooltip("Quão suavemente a lanterna segue a rotação da câmera. Menor = mais 'atraso'.")]
    public float suavidadeRotacao = 10f;
    [Tooltip("A velocidade da oscilação quando o jogador está andando.")]
    public float velocidadeBobAndando = 10f;
    [Tooltip("A intensidade da oscilação quando o jogador está andando.")]
    public float intensidadeBobAndando = 0.04f;
    [Tooltip("A velocidade da oscilação quando o jogador está parado (respiração).")]
    public float velocidadeBobParado = 0.5f;
    [Tooltip("A intensidade da oscilação quando o jogador está parado.")]
    public float intensidadeBobParado = 0.02f;

    // Variável pública para que outros scripts possam verificar o estado
    [HideInInspector] public bool temLanterna = false;

    // Componentes e variáveis privadas
    private Light luzDaLanternaComponent;
    private AudioClip somLigar;
    private AudioClip somDesligar;
    private AudioSource audioSource;
    private Vector3 posicaoInicialHolder;

    void Awake()
    {
        // Pega os componentes
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }

        luzDaLanternaComponent = GetComponentInChildren<Light>();

        // Verificações de segurança
        if (playerRigidbody == null || cameraTransform == null || luzDaLanternaComponent == null)
        {
            Debug.LogError("ERRO: Uma ou mais referências não foram atribuídas no Inspetor do ControleLanternaRealista!", this);
            this.enabled = false;
            return;
        }

        posicaoInicialHolder = transform.localPosition;
    }

    void Start()
    {
        // Garante que a luz comece desligada
        luzDaLanternaComponent.enabled = false;
    }

    void Update()
    {
        if (!temLanterna) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AlternarEstadoLanterna();
        }

        // --- MOVIMENTOS COMBINADOS ---
        CalcularMovimentoRealista();
    }

    private void CalcularMovimentoRealista()
    {
        // --- ROTAÇÃO SUAVE (SWAY) ---
        // A rotação alvo é simplesmente a rotação da câmera principal.
        Quaternion rotacaoAlvo = cameraTransform.rotation;
        // Suaviza a transição da rotação deste objeto (Holder) para a rotação da câmera.
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, suavidadeRotacao * Time.deltaTime);

        // --- OSCILAÇÃO POSICIONAL (BOB) ---
        float velocidadeBob = playerRigidbody.linearVelocity.magnitude > 0.1f ? velocidadeBobAndando : velocidadeBobParado;
        float intensidadeBob = playerRigidbody.linearVelocity.magnitude > 0.1f ? intensidadeBobAndando : intensidadeBobParado;
        float bobHorizontal = Mathf.Sin(Time.time * velocidadeBob) * intensidadeBob;
        float bobVertical = Mathf.Cos(Time.time * velocidadeBob * 0.5f) * intensidadeBob * 0.5f;

        // Aplica a oscilação à posição local deste objeto (Holder)
        transform.localPosition = posicaoInicialHolder + new Vector3(bobHorizontal, bobVertical, 0);
    }

    // --- MÉTODOS PÚBLICOS ---
    public void ColetarLanterna(AudioClip somLigarClip, AudioClip somDesligarClip)
    {
        temLanterna = true;
        somLigar = somLigarClip;
        somDesligar = somDesligarClip;

        luzDaLanternaComponent.enabled = true; // Liga a lanterna ao coletar
        TocarSom(somLigar);
    }

    private void AlternarEstadoLanterna()
    {
        luzDaLanternaComponent.enabled = !luzDaLanternaComponent.enabled;
        TocarSom(luzDaLanternaComponent.enabled ? somLigar : somDesligar);
    }

    private void TocarSom(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}