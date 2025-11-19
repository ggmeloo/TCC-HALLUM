using System.Collections;
using UnityEngine;

// Garante que o script tenha acesso a um CharacterController e um AudioSource
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class ControlePassosJogador : MonoBehaviour
{
    [Header("Configurações dos Passos")]
    [Tooltip("Lista de sons de passo para variar o som.")]
    public AudioClip[] sonsDePasso;
    [Tooltip("O tempo em segundos entre cada passo.")]
    public float intervaloEntrePassos = 0.5f;

    private AudioSource audioSource;
    private CharacterController controller;
    private Coroutine rotinaDePassos;
    private bool estaTocandoPassos = false;

    void Start()
    {
        // Pega os componentes automaticamente
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        // Configura o AudioSource para não tocar ao iniciar
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // Verifica se o jogador está se movendo no chão
        // controller.velocity.magnitude mede a velocidade atual do jogador
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            // Se ele está se movendo mas a rotina de passos não começou, inicia ela.
            if (!estaTocandoPassos)
            {
                rotinaDePassos = StartCoroutine(RotinaDePassos());
                estaTocandoPassos = true;
            }
        }
        else
        {
            // Se ele parou de se mover e a rotina ainda está ativa, para ela.
            if (estaTocandoPassos)
            {
                StopCoroutine(rotinaDePassos);
                estaTocandoPassos = false;
            }
        }
    }

    private IEnumerator RotinaDePassos()
    {
        // Loop infinito que só é interrompido quando a coroutine é parada
        while (true)
        {
            // Pega um som de passo aleatório da lista
            AudioClip clipe = sonsDePasso[Random.Range(0, sonsDePasso.Length)];

            // Toca o som uma vez
            audioSource.PlayOneShot(clipe);

            // Espera o tempo definido antes do próximo passo
            yield return new WaitForSeconds(intervaloEntrePassos);
        }
    }
}