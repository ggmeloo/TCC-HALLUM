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
    [Tooltip("Arraste o objeto pai que contém todos os textos do tutorial para cá.")]
    public GameObject textosDoTutorial; // A "caixa" que vamos desligar

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
        // Garante que os textos do tutorial comecem ativos
        if (textosDoTutorial != null) textosDoTutorial.SetActive(true);

        SelecionarNovasAnomaliasParaVolta();
        ExibirAnomaliasRestantes();
    }

    public void IniciarProximaVolta()
    {
        Debug.Log("Fim da volta " + voltaAtual + ". Avançando para a próxima.");

        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            anomalia.ResetarIdentificacao();
        }

        voltaAtual++;
        ExecutarEventosDaVolta();
        SelecionarNovasAnomaliasParaVolta();
        ExibirAnomaliasRestantes();
    }

    private void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2:
                if (paredeDestino != null)
                {
                    paredeDestino.SetActive(false);
                }

                // Desliga a "caixa" com todos os textos do tutorial de uma vez.
                if (textosDoTutorial != null)
                {
                    textosDoTutorial.SetActive(false);
                    Debug.Log("Textos do tutorial foram desativados.");
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

    void ExibirAnomaliasRestantes()
    {
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            anomalia.DesativarAnomalia();
        }
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            anomalia.AtivarAnomalia();
        }
        Debug.Log("VOLTA " + voltaAtual + " CONFIGURADA. Anomalias para encontrar: " + anomaliasSelecionadasParaEstaVolta.Count);
    }

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

    public void RegistrarAnomaliaEncontrada()
    {
        anomaliasEncontradasNestaVolta++;
        Debug.Log("Anomalia encontrada! Progresso nesta volta: " + anomaliasEncontradasNestaVolta + "/" + anomaliasSelecionadasParaEstaVolta.Count);
        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            TodasAnomaliasEncontradas();
        }
    }

    private void TodasAnomaliasEncontradas()
    {
        Debug.LogWarning("!!! TODAS AS ANOMALIAS DA VOLTA " + voltaAtual + " FORAM ENCONTRADAS! !!!");
    }
}