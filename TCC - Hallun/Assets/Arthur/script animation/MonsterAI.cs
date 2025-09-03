using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    // Variáveis para controlar o estado e as referências
    private bool playerDetected = false;
    private Transform player;

    public float movementSpeed = 3f;
    public Animator monsterAnimator;
    public AudioSource cryingAudio;

    // Esta função é chamada quando algo entra no trigger do CircleDetect
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se o objeto que entrou no círculo de detecção for o Player
        if (other.CompareTag("Player"))
        {
            playerDetected = true;
            player = other.transform;

            // Inicia a animação de correr
            if (monsterAnimator != null)
            {
                // Use o nome do seu parâmetro na Animator (ex: "isRunning")
                monsterAnimator.SetBool("isRunning", true);

            }

            // Para o áudio de choro
            if (cryingAudio != null && cryingAudio.isPlaying)
            {
                cryingAudio.Stop();
            }
        }
    }

    // Esta função é chamada a cada frame
    void Update()
    {
        // Se o player for detectado, o monstro se move em direção a ele
        if (playerDetected && player != null)
        {
            // Calcula a direção para o player
            Vector2 direction = (player.position - transform.position).normalized;

            // Move o monstro em direção ao player
            transform.Translate(direction * movementSpeed * Time.deltaTime);
        }
    }

}