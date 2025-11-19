using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necessário para usar a Queue

// Enum para escolher como o jogador avança no tutorial
public enum ModoAvanco
{
    Tempo,
    Input
}

// Classe que define as propriedades de CADA painel individual na sequência
[System.Serializable]
public class TutorialInfo
{
    [Tooltip("Arraste o GameObject do painel da sua Hierarquia para este campo.")]
    public GameObject painelDoTutorial; // Referência direta ao seu Panel, Panel (1), etc.

    [Header("Controle de Avanço")]
    public ModoAvanco modoAvanco = ModoAvanco.Input;

    [Tooltip("Duração em segundos que o painel ficará visível, se o modo for 'Tempo'.")]
    public float duracao = 3f;

    [Tooltip("Tecla que o jogador deve pressionar para avançar, se o modo for 'Input'.")]
    public KeyCode teclaParaAvancar = KeyCode.E;
}

// Classe que agrupa uma sequência de painéis de tutorial
[System.Serializable]
public class TutorialSequencia
{
    [Tooltip("A lista de painéis que serão mostrados em ordem.")]
    public TutorialInfo[] paineis; // Um array com todas as etapas do tutorial
}


// -------------------------------------------------------------------------------- //
// SCRIPT PRINCIPAL DO GERENCIADOR
// -------------------------------------------------------------------------------- //
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    private Queue<TutorialSequencia> filaTutoriais = new Queue<TutorialSequencia>();
    private bool tutorialAtivo = false;

    private void Awake()
    {
        // Configuração do Singleton para garantir que só exista um TutorialManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Método público para chamar o início de uma nova sequência de tutorial.
    /// </summary>
    /// <param name="sequencia">A sequência de tutoriais a ser exibida.</param>
    public void ExibirSequenciaTutorial(TutorialSequencia sequencia)
    {
        // Verifica se a sequência é válida antes de adicionar à fila
        if (sequencia == null || sequencia.paineis.Length == 0)
        {
            Debug.LogWarning("Tentativa de iniciar uma sequência de tutorial vazia ou nula.");
            return;
        }

        filaTutoriais.Enqueue(sequencia);

        // Se nenhum tutorial estiver ativo, inicia o processamento da fila
        if (!tutorialAtivo)
        {
            StartCoroutine(ProcessarFila());
        }
    }

    // Processa a fila de tutoriais, garantindo que um termine antes do próximo começar
    private IEnumerator ProcessarFila()
    {
        tutorialAtivo = true;

        while (filaTutoriais.Count > 0)
        {
            TutorialSequencia sequenciaAtual = filaTutoriais.Dequeue();
            yield return StartCoroutine(ProcessarSequencia(sequenciaAtual));
        }

        tutorialAtivo = false;
    }

    // Processa cada painel dentro de uma única sequência de tutorial
    private IEnumerator ProcessarSequencia(TutorialSequencia sequencia)
    {
        GameObject painelAtivoAnterior = null;

        // Loop através de cada etapa (painel) definida na sequência
        foreach (var info in sequencia.paineis)
        {
            // 1. Desativa o painel anterior para limpar a tela
            if (painelAtivoAnterior != null)
            {
                painelAtivoAnterior.SetActive(false);
            }

            // 2. Ativa o painel atual da sequência
            if (info.painelDoTutorial != null)
            {
                info.painelDoTutorial.SetActive(true);
                painelAtivoAnterior = info.painelDoTutorial;
            }
            else
            {
                Debug.LogWarning("Um painel na sequência de tutorial não foi atribuído no Inspector. Pulando etapa.");
                continue; // Pula para a próxima iteração do loop
            }

            // 3. Aguarda a condição de avanço (Tempo ou Input)
            if (info.modoAvanco == ModoAvanco.Tempo)
            {
                yield return new WaitForSeconds(info.duracao);
            }
            else // ModoAvanco.Input
            {
                // Espera em um loop até que a tecla correta seja pressionada
                while (!Input.GetKeyDown(info.teclaParaAvancar))
                {
                    yield return null; // Pausa a execução por um frame
                }
            }
        }

        // 4. Ao final de toda a sequência, desativa o último painel exibido
        if (painelAtivoAnterior != null)
        {
            painelAtivoAnterior.SetActive(false);
        }
    }

    public void ExibirTutorialGameObject(GameObject tutorialObject)
    {
        if (tutorialObject != null)
        {
            tutorialObject.SetActive(true);
        }
    }


    // Adicione esta função inteira ao seu script TutorialManager.cs

    public void ExibirTutorialCanvas(Canvas tutorialCanvas)
    {
        if (tutorialCanvas != null)
        {
            // Ativa o GameObject ao qual o componente Canvas pertence.
            tutorialCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Tentativa de exibir tutorial, mas nenhum Canvas foi fornecido!");
        }
    }
}