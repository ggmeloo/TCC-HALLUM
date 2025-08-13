using UnityEngine;

// Garante que este script só pode ser adicionado a um objeto que tenha um Rigidbody.
[RequireComponent(typeof(Rigidbody))]
public class MovimentoRigidbody : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o objeto da Câmera do jogador para este campo.")]
    public Transform cameraTransform;

    [Header("Movimento")]
    [Tooltip("A velocidade de caminhada do jogador.")]
    public float velocidadeMovimento = 7f;
    [Tooltip("A força aplicada para o pulo.")]
    public float forcaPulo = 5f;

    [Header("Controle da Câmera")]
    [Tooltip("Sensibilidade do mouse para rotação.")]
    public float sensibilidadeMouse = 3f;
    private float rotacaoCameraX = 0f;

    [Header("Verificação de Chão")]
    [Tooltip("Um objeto vazio posicionado nos pés do jogador para detectar o chão.")]
    public Transform verificadorChao;
    [Tooltip("O raio da esfera de verificação de chão.")]
    public float raioVerificacaoChao = 0.4f;
    [Tooltip("Define qual(is) layer(s) são consideradas 'Chão' para o pulo.")]
    public LayerMask layerChao;

    // Variáveis privadas
    private Rigidbody rb;
    private Vector2 inputMovimento; // Usaremos para guardar o input do teclado
    private bool querPular = false;
    private bool estaNoChao;

    // Awake é chamado antes de Start. Ideal para pegar referências.
    void Awake()
    {
        // Pega a referência do Rigidbody neste objeto.
        rb = GetComponent<Rigidbody>();

        // Trava o cursor no centro da tela e o esconde.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update é chamado a cada frame. Ideal para capturar input.
    void Update()
    {
        // --- SEÇÃO DE INPUT ---

        // Captura o input do teclado (W,A,S,D ou setas)
        inputMovimento.x = Input.GetAxisRaw("Horizontal");
        inputMovimento.y = Input.GetAxisRaw("Vertical");

        // Captura o input do mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        // Captura o input do pulo
        if (Input.GetButtonDown("Jump"))
        {
            querPular = true;
        }

        // --- SEÇÃO DE ROTAÇÃO DA CÂMERA ---

        // Rotaciona o corpo do jogador horizontalmente
        transform.Rotate(Vector3.up * mouseX);

        // Calcula e limita a rotação vertical da câmera
        rotacaoCameraX -= mouseY;
        rotacaoCameraX = Mathf.Clamp(rotacaoCameraX, -90f, 90f);

        // Aplica a rotação vertical apenas à câmera
        cameraTransform.localRotation = Quaternion.Euler(rotacaoCameraX, 0f, 0f);
    }

    // FixedUpdate é chamado em um intervalo fixo. É OBRIGATÓRIO para todas as operações de física.
    void FixedUpdate()
    {
        // Verifica se o jogador está no chão usando uma esfera invisível
        estaNoChao = Physics.CheckSphere(verificadorChao.position, raioVerificacaoChao, layerChao);

        // --- MOVIMENTO DO JOGADOR ---
        MoverJogador();

        // --- PULO DO JOGADOR ---
        Pular();
    }

    private void MoverJogador()
    {
        // Calcula a direção do movimento baseada no input e na rotação do jogador
        Vector3 direcaoMovimento = (transform.forward * inputMovimento.y + transform.right * inputMovimento.x).normalized;

        // Calcula a velocidade alvo
        Vector3 velocidadeAlvo = direcaoMovimento * velocidadeMovimento;

        // Define a velocidade do Rigidbody
        // Mantemos a velocidade vertical (Y) original para não interferir com a gravidade ou pulo
        rb.linearVelocity = new Vector3(velocidadeAlvo.x, rb.linearVelocity.y, velocidadeAlvo.z);
    }

    private void Pular()
    {
        // Se o jogador quer pular E está no chão...
        if (querPular && estaNoChao)
        {
            // Adiciona uma força vertical instantânea (Impulse)
            rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
        }
        // Reseta a flag do pulo para que ele não pule infinitamente
        querPular = false;
    }

    // Função para desenhar o gizmo de verificação de chão no editor (ajuda na visualização)
    private void OnDrawGizmosSelected()
    {
        if (verificadorChao != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(verificadorChao.position, raioVerificacaoChao);
        }
    }
}