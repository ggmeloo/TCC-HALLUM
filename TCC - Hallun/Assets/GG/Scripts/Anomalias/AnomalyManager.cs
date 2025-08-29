using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager instance;

    [Header("Controle do Loop")]
    public int voltaAtual = 1;

    [Header("Configuração das Anomalias")]
    public int[] quantidadeAnomaliasPorVolta = new int[] { 3, 4, 5 };

    private List<Anomalia> anomaliasFixas = new List<Anomalia>();
    private List<Anomalia> anomaliasAleatorias = new List<Anomalia>();
    private List<Anomalia> anomaliasSelecionadasParaEstaVolta = new List<Anomalia>();
    private int anomaliasEncontradasNestaVolta = 0;

    [Header("Referências da Cena")]
    public DemonioController demonio;
    public GameObject paredeDestino;
    public GameObject triggerFimPerseguicao;
    [Header("Pontos de Susto")]
    public Transform pontoSustoPorta, pontoSustoCorrida, pontoFuga, pontoInicioPerseguicao;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        OrganizarTodasAnomalias();
    }

    void Start()
    {
        if (demonio != null) demonio.gameObject.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true);
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);

        SelecionarNovasAnomaliasParaVolta();
        ExibirAnomaliasRestantes();
    }

    // Este método agora sempre avança para a próxima volta
    public void IniciarProximaVolta()
    {
        Debug.Log("Fim da volta " + voltaAtual + ". Avançando para a próxima.");

        // Reseta o estado das anomalias da volta que acabou
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            anomalia.ResetarIdentificacao();
        }

        // Avança o contador de volta
        voltaAtual++;

        // Executa os eventos da nova volta
        ExecutarEventosDaVolta();

        // Prepara um novo conjunto de anomalias
        SelecionarNovasAnomaliasParaVolta();

        // Exibe as novas anomalias na cena
        ExibirAnomaliasRestantes();
    }

    // Controla os eventos com script
    private void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2:
                // A parede é desativada aqui, no início da volta 2
                if (paredeDestino != null)
                {
                    paredeDestino.SetActive(false);
                    Debug.Log("A passagem para o destino foi aberta.");
                }

                if (demonio != null) demonio.AparecerEAssombrar(pontoSustoPorta);
                break;
            case 3:
                if (demonio != null)
                {
                    demonio.Desaparecer();
                    demonio.ExecutarSustoDaCorrida(pontoSustoCorrida, pontoFuga);
                }
                break;
        }
    }

    // Seleciona as anomalias para a volta atual
    void SelecionarNovasAnomaliasParaVolta()
    {
        anomaliasEncontradasNestaVolta = 0;
        anomaliasSelecionadasParaEstaVolta.Clear();
        if (voltaAtual == 1)
        {
            anomaliasSelecionadasParaEstaVolta.AddRange(anomaliasFixas);
        }
        else if (voltaAtual > 1 && (voltaAtual - 2) < quantidadeAnomaliasPorVolta.Length)
        {
            int quantidadeParaAtivar = quantidadeAnomaliasPorVolta[voltaAtual - 2];
            var anomaliasEmbaralhadas = anomaliasAleatorias.OrderBy(a => Random.value).ToList();
            for (int i = 0; i < quantidadeParaAtivar && i < anomaliasEmbaralhadas.Count; i++)
            {
                anomaliasSelecionadasParaEstaVolta.Add(anomaliasEmbaralhadas[i]);
            }
        }
    }

    // Ativa as anomalias selecionadas na cena
    void ExibirAnomaliasRestantes()
    {
        // Primeiro desativa todas as anomalias da seleção anterior para limpar a cena
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            anomalia.DesativarAnomalia();
        }

        // Agora ativa as novas anomalias selecionadas para a volta atual
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            anomalia.AtivarAnomalia();
        }
        Debug.Log("VOLTA " + voltaAtual + " CONFIGURADA. Anomalias para encontrar: " + anomaliasSelecionadasParaEstaVolta.Count);
    }

    // Encontra e organiza todas as anomalias da cena no início
    void OrganizarTodasAnomalias()
    {
        Anomalia[] todasAnomalias = FindObjectsByType<Anomalia>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var anomalia in todasAnomalias)
        {
            if (anomalia.tipo == Anomalia.TipoAnomalia.Fixa) anomaliasFixas.Add(anomalia);
            else anomaliasAleatorias.Add(anomalia);
            anomalia.DesativarAnomalia();
        }
    }

    // Registra quando uma anomalia é encontrada
    public void RegistrarAnomaliaEncontrada()
    {
        anomaliasEncontradasNestaVolta++;
        Debug.Log("Anomalia encontrada! Progresso nesta volta: " + anomaliasEncontradasNestaVolta + "/" + anomaliasSelecionadasParaEstaVolta.Count);

        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            TodasAnomaliasEncontradas();
        }
    }

    // Apenas um feedback para o jogador
    private void TodasAnomaliasEncontradas()
    {
        Debug.LogWarning("!!! TODAS AS ANOMALIAS DA VOLTA " + voltaAtual + " FORAM ENCONTRADAS! !!!");
    }
}