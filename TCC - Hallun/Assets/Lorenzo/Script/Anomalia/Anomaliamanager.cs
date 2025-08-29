// AnomalyManager.cs

using UnityEngine;
using System.Collections.Generic; // Necessário para usar Listas

public class AnomalyManager : MonoBehaviour
{
    // --- Variáveis de Configuração ---
    public static AnomalyManager instance;

    [Header("Controle do Loop")]
    public int voltaAtual = 1;

    [Header("Referências da Cena")]
    public DemonioController demonio;
    public GameObject paredeDestino;
    public GameObject triggerFimPerseguicao;

    [Header("Pontos de Susto")]
    public Transform pontoSustoPorta;
    public Transform pontoSustoCorrida;
    public Transform pontoFuga;
    [Tooltip("Ponto de onde o demônio surgirá para iniciar a perseguição na volta 4.")]
    public Transform pontoInicioPerseguicao;

    // --- Variáveis de Controle Interno ---
    private List<Anomalia> todasAsAnomaliasDaCena; // Armazena TODAS as anomalias
    private int totalDeAnomaliasNaVolta; // Quantas anomalias estão ativas NESTA volta
    private int anomaliasEncontradasNestaVolta = 0; // Quantas foram encontradas NESTA volta

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Encontra e armazena TODAS as anomalias da cena, mesmo as inativas.
        todasAsAnomaliasDaCena = new List<Anomalia>(FindObjectsByType<Anomalia>(FindObjectsInactive.Include, FindObjectsSortMode.None));
    }

    void Start()
    {
        if (demonio != null) demonio.gameObject.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true);
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);

        // Inicia a primeira volta
        AtualizarAnomaliasParaVoltaAtual();
    }

    public void IniciarProximaVolta()
    {
        voltaAtual++;
        Debug.Log("----- NOVA VOLTA INICIADA: " + voltaAtual + " -----");

        // Atualiza quais anomalias devem estar ativas
        AtualizarAnomaliasParaVoltaAtual();

        // Executa os eventos especiais da volta
        ExecutarEventosDaVolta();
    }

    /// <summary>
    /// Este método central controla quais anomalias estão visíveis.
    /// </summary>
    void AtualizarAnomaliasParaVoltaAtual()
    {
        // Reseta as contagens para a nova volta
        anomaliasEncontradasNestaVolta = 0;
        totalDeAnomaliasNaVolta = 0;

        // Itera sobre TODAS as anomalias que existem na cena
        foreach (Anomalia anomalia in todasAsAnomaliasDaCena)
        {
            // Verifica se a anomalia pertence a esta volta
            if (anomalia.voltaDeAtivacao == voltaAtual)
            {
                anomalia.gameObject.SetActive(true); // Ativa o objeto
                totalDeAnomaliasNaVolta++; // Adiciona à contagem da volta atual
            }
            else
            {
                anomalia.gameObject.SetActive(false); // Garante que anomalias de outras voltas estejam desativadas
            }
        }

        Debug.Log("Anomalias ativas para esta volta (" + voltaAtual + "): " + totalDeAnomaliasNaVolta);
    }

    /// <summary>
    /// Lida com os sustos e eventos programados para cada volta.
    /// </summary>
    void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2:
                if (paredeDestino != null)
                {
                    paredeDestino.SetActive(false);
                    Debug.Log("A passagem para o destino foi aberta.");
                }
                if (demonio != null)
                {
                    demonio.AparecerEAssombrar(pontoSustoPorta);
                }
                break;

            case 3:
                if (demonio != null)
                {
                    demonio.Desaparecer();
                    demonio.ExecutarSustoDaCorrida(pontoSustoCorrida, pontoFuga);
                }
                break;

            case 4:
                Debug.Log("A perseguição pode ser ativada nesta volta.");
                // Aqui você pode ativar a lógica da perseguição
                break;
        }
    }

    /// <summary>
    /// Chamado pelo script Anomalia quando uma é identificada com sucesso.
    /// </summary>
    public void RegistrarAnomaliaEncontrada()
    {
        anomaliasEncontradasNestaVolta++;
        Debug.Log("Anomalia encontrada! Progresso: " + anomaliasEncontradasNestaVolta + "/" + totalDeAnomaliasNaVolta);

        // Verifica se todas as anomalias da volta atual foram encontradas
        if (anomaliasEncontradasNestaVolta >= totalDeAnomaliasNaVolta)
        {
            TodasAnomaliasDaVoltaEncontradas();
        }
    }

    /// <summary>
    /// Chamado quando todas as anomalias da volta atual são encontradas.
    /// </summary>
    private void TodasAnomaliasDaVoltaEncontradas()
    {
        Debug.LogWarning("OBJETIVO CONCLUÍDO: Todas as " + totalDeAnomaliasNaVolta + " anomalias da volta " + voltaAtual + " foram encontradas!");
        // Você pode adicionar lógicas aqui, como:
        // - Tocar um som de sucesso
        // - Abrir uma porta para o jogador poder avançar para o trigger de loop
        // - Mostrar uma mensagem na tela
    }
}