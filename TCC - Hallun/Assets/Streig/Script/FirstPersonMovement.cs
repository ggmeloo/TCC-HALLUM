using UnityEngine;

public class FirstPersonMovement: MonoBehaviour
{
    public float velocidadeMovimento = 5f;
    public float sensibilidadeMouse = 2f;
    public float limiteVerticalCamera = 80f;

    private CharacterController controle;
    private Camera cam;
    private float rotacaoVertical = 0f;
    private Vector3 velocidadeVertical = Vector3.zero;
    public float gravidade = -9.81f;

    void Start()
    {
        controle = GetComponent<CharacterController>();
        cam = Camera.main;

        // Trava e esconde o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotação do mouse (olhar)
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        // Rotação horizontal (personagem)
        transform.Rotate(0, mouseX, 0);

        // Rotação vertical (câmera)
        rotacaoVertical -= mouseY;
        rotacaoVertical = Mathf.Clamp(rotacaoVertical, -limiteVerticalCamera, limiteVerticalCamera);
        cam.transform.localRotation = Quaternion.Euler(rotacaoVertical, 0, 0);

        // Movimento WASD
        float movimentoX = Input.GetAxis("Horizontal") * velocidadeMovimento;
        float movimentoZ = Input.GetAxis("Vertical") * velocidadeMovimento;

        Vector3 movimento = transform.right * movimentoX + transform.forward * movimentoZ;

        // Aplica gravidade
        if (controle.isGrounded)
        {
            velocidadeVertical.y = -0.5f; // Pequena força para manter no chão
        }
        else
        {
            velocidadeVertical.y += gravidade * Time.deltaTime;
        }

        // Combina movimento horizontal e vertical
        movimento += velocidadeVertical;
        controle.Move(movimento * Time.deltaTime);
    }
}