using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoRigidbody : MonoBehaviour
{
    [Header("Referências")]
    public Transform cameraTransform;

    [Header("Movimento")]
    public float velocidadeMovimento = 7f;
    public float forcaPulo = 5f;

    [Header("Suavização")]
    public float tempoSuavizacaoMovimento = 0.1f;

    [Header("Controle da Câmera")]
    public float sensibilidadeMouse = 3f;
    private float rotacaoCameraX = 0f;

    [Header("Verificação de Chão")]
    public Transform verificadorChao;
    public float raioVerificacaoChao = 0.4f;
    public LayerMask layerChao;

    [Header("Sons de Passos")]
    [Tooltip("O AudioSource dedicado APENAS para os passos.")]
    public AudioSource audioSourcePassos;
    [Tooltip("O arquivo de som de passos que será repetido.")]
    public AudioClip somDePasso;
    [Tooltip("Pausa em segundos entre cada repetição do som de passo.")]
    public float intervaloEntrePassos = 1.0f;

    // Variáveis privadas
    private Rigidbody rb;
    private Vector2 inputMovimento;
    private bool querPular = false;
    private bool estaNoChao;
    private Vector3 velocityRef = Vector3.zero;
    private Coroutine rotinaDePassos;
    private bool estaTocandoPassos = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (audioSourcePassos != null)
        {
            audioSourcePassos.playOnAwake = false;
            audioSourcePassos.loop = false; // Garante que o loop do AudioSource está desligado
        }
    }

    void Update()
    {
        inputMovimento.x = Input.GetAxisRaw("Horizontal");
        inputMovimento.y = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump")) querPular = true;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;
        transform.Rotate(Vector3.up * mouseX);
        rotacaoCameraX -= mouseY;
        rotacaoCameraX = Mathf.Clamp(rotacaoCameraX, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(rotacaoCameraX, 0f, 0f);

        VerificarSomDePassos();
    }

    void FixedUpdate()
    {
        estaNoChao = Physics.CheckSphere(verificadorChao.position, raioVerificacaoChao, layerChao);
        MoverJogador();
        Pular();
    }

    private void MoverJogador()
    {
        Vector3 direcaoMovimento = (transform.forward * inputMovimento.y + transform.right * inputMovimento.x).normalized;
        Vector3 velocidadeAlvo = direcaoMovimento * velocidadeMovimento;
        Vector3 velocidadeAlvoComEixoY = new Vector3(velocidadeAlvo.x, rb.linearVelocity.y, velocidadeAlvo.z);
        rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, velocidadeAlvoComEixoY, ref velocityRef, tempoSuavizacaoMovimento);
    }

    private void Pular()
    {
        if (querPular && estaNoChao)
        {
            rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
        }
        querPular = false;
    }

    private void VerificarSomDePassos()
    {
        if (somDePasso == null || audioSourcePassos == null) return;

        bool estaSeMovendo = estaNoChao && inputMovimento.magnitude > 0.1f;

        if (estaSeMovendo && !estaTocandoPassos)
        {
            rotinaDePassos = StartCoroutine(RotinaDePassosRitmicamente());
            estaTocandoPassos = true;
        }
        else if (!estaSeMovendo && estaTocandoPassos)
        {
            StopCoroutine(rotinaDePassos);
            audioSourcePassos.Stop();
            estaTocandoPassos = false;
        }
    }

    private IEnumerator RotinaDePassosRitmicamente()
    {
        while (true)
        {
            audioSourcePassos.PlayOneShot(somDePasso);
            yield return new WaitForSeconds(somDePasso.length);
            yield return new WaitForSeconds(intervaloEntrePassos);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (verificadorChao != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(verificadorChao.position, raioVerificacaoChao);
        }
    }
}