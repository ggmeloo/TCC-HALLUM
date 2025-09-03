using UnityEngine;
using UnityEngine.AI;

public class ComportamentoInimigo : MonoBehaviour
{
    public Transform jogador;
    public float distanciaParaCorrer = 5f;
    public Animator animador; // Arraste o componente Animator para cá

    private NavMeshAgent agenteDeNavegacao;

    void Start()
    {
        agenteDeNavegacao = GetComponent<NavMeshAgent>();
        // O NavMeshAgent pode começar desativado, já que a personagem não está correndo
        if (agenteDeNavegacao != null)
        {
            agenteDeNavegacao.enabled = false;
        }

        // Garante que a animação inicial seja a de chorar
        if (animador != null)
        {
            animador.SetBool("IsCorrendo", false);
        }
    }

    void Update()
    {
        // Calcula a distância entre a personagem e o jogador
        float distancia = Vector3.Distance(transform.position, jogador.position);

        if (distancia < distanciaParaCorrer)
        {
            // Se o jogador estiver perto, ative a perseguição
            AtivarPerseguicao();
        }
        else
        {
            // Se o jogador estiver longe, volte a chorar
            PararPerseguicao();
        }
    }

    void AtivarPerseguicao()
    {
        // Liga a animação de correr
        if (animador != null)
        {
            animador.SetBool("IsCorrendo", true);
        }

        // Ativa o NavMeshAgent para a personagem seguir o jogador
        if (agenteDeNavegacao != null)
        {
            agenteDeNavegacao.enabled = true;
            agenteDeNavegacao.SetDestination(jogador.position);
        }
    }

    void PararPerseguicao()
    {
        // Desliga a animação de correr
        if (animador != null)
        {
            animador.SetBool("IsCorrendo", false);
        }

        // Desativa o NavMeshAgent para a personagem parar de se mover
        if (agenteDeNavegacao != null)
        {
            agenteDeNavegacao.enabled = false;
        }
    }
}