using UnityEngine;

public class CameraLookController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o Transform do objeto principal do Player aqui.")]
    public Transform playerBody;
    [Tooltip("Arraste o Transform do objeto 'Hand' (que segura a lanterna) aqui.")]
    public Transform hand;

    [Header("Configurações da Câmera")]
    [Tooltip("Sensibilidade do mouse.")]
    public float mouseSensitivity = 200f;

    [Header("Configurações da Oscilação (Sway)")]
    [Tooltip("Velocidade com que a mão alcança a câmera. Valores menores = mais atraso.")]
    public float handSwaySpeed = 5f;

    private float rotationX = 0f;

    void Start()
    {
        // Trava o cursor no centro da tela e o esconde
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Captura o input do mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Rotação Vertical da Câmera (Olhar para cima/baixo)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Limita a rotação para não virar de cabeça para baixo
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // 3. Rotação Horizontal do Player (Olhar para os lados)
        playerBody.Rotate(Vector3.up * mouseX);

        // 4. LÓGICA DA OSCILAÇÃO DA LANTERNA (O PONTO PRINCIPAL!)
        // O alvo da rotação da mão é sempre a rotação atual da câmera
        Quaternion cameraTargetRotation = transform.localRotation;

        // Interpola suavemente a rotação atual da mão em direção à rotação da câmera
        // É isso que cria o efeito de "atraso" e suavidade
        hand.localRotation = Quaternion.Slerp(hand.localRotation, cameraTargetRotation, handSwaySpeed * Time.deltaTime);
    }
}