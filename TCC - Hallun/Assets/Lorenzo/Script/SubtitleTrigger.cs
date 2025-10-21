using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    [Header("Dados da Legenda")]
    [Tooltip("Arraste aqui o 'Arquivo de Legenda' que você criou no seu projeto.")]
    public SubtitleData subtitleData; // O campo para conectar o molde da legenda

    [Header("Controle do Gatilho")]
    [Tooltip("Marque para que esta legenda apareça apenas uma vez.")]
    public bool triggerOnlyOnce = true;

    private bool hasBeenTriggered = false;

    // Função que detecta a entrada do jogador
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou é o Jogador
        if (other.CompareTag("Player"))
        {
            // Se já foi ativado e só pode uma vez, ele para aqui.
            if (triggerOnlyOnce && hasBeenTriggered)
            {
                return;
            }

            // Validação para evitar erros se você esquecer de arrastar o arquivo.
            if (subtitleData == null)
            {
                Debug.LogError("Nenhum dado de legenda foi atribuído a este gatilho!", this.gameObject);
                return;
            }

            // Pega a mensagem e a duração diretamente do arquivo de dados e manda para o SubtitleManager.
            SubtitleManager.instance.ShowSubtitle(subtitleData.subtitleMessage, subtitleData.displayDuration);

            // Marca que o gatilho já foi usado.
            hasBeenTriggered = true;
        }
    }
}