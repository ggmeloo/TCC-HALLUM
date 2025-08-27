using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class DemonioController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void AparecerEAssombrar(Transform pontoDeAparicao)
    {
        if (pontoDeAparicao != null)
        {
            transform.position = pontoDeAparicao.position;
            transform.rotation = pontoDeAparicao.rotation;
            gameObject.SetActive(true);
            Debug.Log("Demônio apareceu na porta para assombrar a volta 2.");
        }
        else
        {
            Debug.LogError("Tentativa de aparecer, mas o ponto de aparição é nulo!");
        }
    }

    public void Desaparecer()
    {
        gameObject.SetActive(false);
        Debug.Log("Demônio desapareceu.");
    }

    public void ExecutarSustoDaCorrida(Transform pontoDeAparicao, Transform pontoDeFuga)
    {
        gameObject.SetActive(true);
        StartCoroutine(SustoCorridaCoroutine(pontoDeAparicao, pontoDeFuga));
    }

    private IEnumerator SustoCorridaCoroutine(Transform pontoDeAparicao, Transform pontoDeFuga)
    {
        agent.Warp(pontoDeAparicao.position);
        transform.rotation = pontoDeAparicao.rotation;

        yield return new WaitForSeconds(10f);

        animator.SetTrigger("ComecarACorrer");
        agent.SetDestination(pontoDeFuga.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void IniciarPerseguicao(Transform alvo)
    {
        gameObject.SetActive(true);
        animator.SetTrigger("ComecarACorrer");
        StartCoroutine(PerseguirAlvo(alvo));
    }

    // Esta é a corrotina que estava dando erro. Aqui está a versão correta.
    private IEnumerator PerseguirAlvo(Transform alvo)
    {
        while (gameObject.activeSelf)
        {
            if (alvo != null)
            {
                agent.SetDestination(alvo.position);
            }
            // O "yield return" é o "retorno" que o compilador estava procurando.
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void PararPerseguicaoEDesaparecer()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}