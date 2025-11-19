using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [Tooltip("Configure aqui a sequência de painéis que este gatilho irá disparar.")]
    public TutorialSequencia sequenciaDeTutorial;

    [Tooltip("Se marcado, o gatilho só funcionará uma vez.")]
    public bool dispararApenasUmaVez = true;

    private bool jaDisparado = false;

    private void Awake()
    {
        // Garante que o collider está configurado como trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no gatilho é o jogador
        if (other.CompareTag("Player"))
        {
            // Se for para disparar apenas uma vez e já o fez, não faz nada
            if (dispararApenasUmaVez && jaDisparado)
            {
                return;
            }

            // Chama o TutorialManager para iniciar a sequência
            TutorialManager.instance.ExibirSequenciaTutorial(sequenciaDeTutorial);

            jaDisparado = true;

            // Opcional: Desativa o objeto do gatilho após o uso para otimização
            if (dispararApenasUmaVez)
            {
                gameObject.SetActive(false);
            }
        }
    }
}