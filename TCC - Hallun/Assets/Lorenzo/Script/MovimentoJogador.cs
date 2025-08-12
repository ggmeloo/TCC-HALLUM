using UnityEngine;

// Certifique-se de que o nome da classe é EXATAMENTE "MovimentoJogador"
public class MovimentoJogador : MonoBehaviour
{
    // --- VARIÁVEIS DE MOVIMENTO ---
    // Você pode ajustar estes valores no Inspector da Unity
    public float velocidade = 5f;
    public float gravidade = -9.81f;
    public float alturaPulo = 1.5f;

    // --- VARIÁVEIS DA CÂMERA ---
    public Transform cameraTransform; // Arraste o objeto da Câmera para este campo no Inspector
    public float sensibilidadeMouse = 2f;
    private float rotacaoCameraX = 0f;

    // --- COMPONENTES E CHECAGEM DE CHÃO ---
    private CharacterController controller;
    private Vector3 velocidadeVertical; // Usado para calcular gravidade e pulo
    public Transform verificadorChao; // Um objeto vazio posicionado nos pés do jogador
    public float raioVerificacaoChao = 0.4f;
    public LayerMask layerChao; // Defina qual layer é considerada "Chão"
    private bool estaNoChao;

    // A função Start é chamada uma vez quando o script é iniciado
    void Start()
    {
        // Pega o componente CharacterController que deve estar no mesmo objeto deste script
        controller = GetComponent<CharacterController>();

        // Trava o cursor no centro da tela e o deixa invisível para um controle de câmera adequado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // A função Update é chamada a cada frame do jogo
    void Update()
    {
        // --- SEÇÃO 1: ROTAÇÃO DA CÂMERA (MOUSE) ---

        // Pega os valores de movimento do mouse nos eixos X e Y
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        // Rotaciona o corpo do jogador para a esquerda e direita
        transform.Rotate(Vector3.up * mouseX);

        // Calcula a rotação vertical da câmera (para cima e para baixo)
        rotacaoCameraX -= mouseY;
        // Limita a rotação da câmera para que o jogador не consiga "quebrar o pescoço"
        rotacaoCameraX = Mathf.Clamp(rotacaoCameraX, -90f, 90f);
        // Aplica a rotação vertical apenas à câmera, e não ao corpo do jogador
        cameraTransform.localRotation = Quaternion.Euler(rotacaoCameraX, 0f, 0f);


        // --- SEÇÃO 2: MOVIMENTAÇÃO (TECLADO) ---

        // Verifica se o jogador está tocando o chão
        estaNoChao = Physics.CheckSphere(verificadorChao.position, raioVerificacaoChao, layerChao);

        // Se o jogador estiver no chão e caindo, reseta sua velocidade vertical
        if (estaNoChao && velocidadeVertical.y < 0)
        {
            velocidadeVertical.y = -2f; // Um pequeno valor para garantir que ele permaneça no chão
        }

        // Pega os valores das teclas W, A, S, D (e setas direcionais)
        float x = Input.GetAxis("Horizontal"); // -1 para A, +1 para D
        float z = Input.GetAxis("Vertical");   // -1 para S, +1 para W

        // Cria o vetor de direção do movimento, relativo à direção para onde o jogador está olhando
        Vector3 direcaoMovimento = transform.right * x + transform.forward * z;

        // Move o CharacterController na direção calculada
        controller.Move(direcaoMovimento * velocidade * Time.deltaTime);


        // --- SEÇÃO 3: PULO E GRAVIDADE ---

        // Verifica se a tecla de pulo (espaço, por padrão) foi pressionada E se o jogador está no chão
        if (Input.GetButtonDown("Jump") && estaNoChao)
        {
            // Aplica uma força vertical para cima, baseada na fórmula física de altura
            velocidadeVertical.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
        }

        // Aplica a força da gravidade ao jogador constantemente
        velocidadeVertical.y += gravidade * Time.deltaTime;
        // Move o CharacterController aplicando a gravidade
        controller.Move(velocidadeVertical * Time.deltaTime);
    }
}