using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager instance;

    [Header("Controle do Loop")]
    public int voltaAtual = 1;

    [Header("Referências dos Containers de Anomalias")]
    [Tooltip("Arraste o objeto PAI que contém as anomalias do tutorial para cá.")]
    public GameObject anomaliasTutorialContainer;
    [Tooltip("Arraste o objeto PAI que contém TODAS as anomalias aleatórias para cá.")]
    public GameObject anomaliasAleatoriasContainer;

    [Header("Configuração das Voltas")]
    public int[] quantidadeAnomaliasPorVolta = new int[] { 3, 4, 5 };

    // Listas internas
    private List<Anomalia> anomaliasFixas = new List<Anomalia>();
    private List<Anomalia> anomaliasAleatoriasDisponiveis = new List<Anomalia>();
    private List<Anomalia> anomaliasAleatoriasJaUsadas = new List<Anomalia>();
    private List<Anomalia> anomaliasSelecionadasParaEstaVolta = new List<Anomalia>();
    private int anomaliasEncontradasNestaVolta = 0;

    [Header("Referências da Cena")]
    public DemonioController demonio;
    public GameObject paredeDestino;
    public GameObject triggerFimPerseguicao;
    public GameObject textosDoTutorial;
    public GameObject containerDeLuzes;
    public GatilhoDeVolta gatilhoDeVolta;

    [Header("Pontos de Susto")]
    public Transform pontoSustoPorta, pontoSustoCorrida, pontoFuga, pontoInicioPerseguicao;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        OrganizarAnomaliasDaCena();
    }

    void Start()
    {
        // Configuração inicial para a Volta 1
        if (demonio != null) demonio.gameObject.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true);
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);
        if (textosDoTutorial != null) textosDoTutorial.SetActive(false);
        if (containerDeLuzes != null) containerDeLuzes.SetActive(true);
        if (anomaliasTutorialContainer != null) anomaliasTutorialContainer.SetActive(false);

        Debug.Log("VOLTA 1 (Reconhecimento) INICIADA.");
    }

    void OrganizarAnomaliasDaCena()
    {
        // Busca as anomalias usando o método robusto do script de teste
        if (anomaliasTutorialContainer != null)
        {
            anomaliasFixas.AddRange(anomaliasTutorialContainer.GetComponentsInChildren<Anomalia>(true));
        }
        if (anomaliasAleatoriasContainer != null)
        {
            anomaliasAleatoriasDisponiveis.AddRange(anomaliasAleatoriasContainer.GetComponentsInChildren<Anomalia>(true));
        }
    }

    public void ProcessarMudancaDeVolta()
    {
        if (voltaAtual == 1 || anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            if (voltaAtual > 1)
            {
                foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
                {
                    if (anomalia.tipo == Anomalia.TipoAnomalia.Aleatoria)
                    {
                        anomaliasAleatoriasDisponiveis.Remove(anomalia);
                        anomaliasAleatoriasJaUsadas.Add(anomalia);
                    }
                    anomalia.ResetarIdentificacao();
                }
            }
            voltaAtual++;
            ExecutarEventosDaVolta();
            SelecionarNovasAnomaliasParaVolta();
        }
        else
        {
            Debug.LogWarning("Nem todas as anomalias foram encontradas! Reiniciando a Volta " + voltaAtual + ".");
        }
        ExibirAnomaliasRestantes();
        if (gatilhoDeVolta != null) gatilhoDeVolta.ResetarGatilho();
    }

    private void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2: // PANE DE ENERGIA E TUTORIAL
                if (containerDeLuzes != null) StartCoroutine(PiscarLuzesCoroutine(5, 0.1f));
                if (paredeDestino != null) paredeDestino.SetActive(false);
                if (textosDoTutorial != null) textosDoTutorial.SetActive(true);
                break;
            case 3: // Susto
                if (demonio != null) demonio.AparecerEAssombrar(pontoSustoPorta);
                break;
        }
    }

    void SelecionarNovasAnomaliasParaVolta()
    {
        anomaliasEncontradasNestaVolta = 0;
        anomaliasSelecionadasParaEstaVolta.Clear();

        if (voltaAtual == 2)
        {
            anomaliasSelecionadasParaEstaVolta.AddRange(anomaliasFixas);
        }
        else if (voltaAtual > 2)
        {
            int index = voltaAtual - 3;
            if (index < quantidadeAnomaliasPorVolta.Length)
            {
                int quantidadeParaAtivar = quantidadeAnomaliasPorVolta[index];
                var anomaliasEmbaralhadas = anomaliasAleatoriasDisponiveis.OrderBy(a => Random.value).ToList();
                for (int i = 0; i < quantidadeParaAtivar && i < anomaliasEmbaralhadas.Count; i++)
                {
                    anomaliasSelecionadasParaEstaVolta.Add(anomaliasEmbaralhadas[i]);
                }
            }
        }
    }

    void ExibirAnomaliasRestantes()
    {
        // Limpeza: desativa todos os containers primeiro para garantir
        if (anomaliasTutorialContainer != null) anomaliasTutorialContainer.SetActive(false);
        // Desativa anomalias aleatórias individualmente
        foreach (var anomalia in anomaliasAleatoriasDisponiveis) anomalia.gameObject.SetActive(false);
        foreach (var anomalia in anomaliasAleatoriasJaUsadas) anomalia.gameObject.SetActive(false);

        // Ativação seletiva
        if (voltaAtual == 2)
        {
            if (anomaliasTutorialContainer != null) anomaliasTutorialContainer.SetActive(true);
        }
        else if (voltaAtual > 2)
        {
            foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
            {
                if (!anomalia.FoiIdentificada())
                {
                    anomalia.gameObject.SetActive(true);
                }
            }
        }
        Debug.Log("VOLTA " + voltaAtual + " CONFIGURADA. Anomalias para encontrar: " + anomaliasSelecionadasParaEstaVolta.Count);
    }

    private IEnumerator PiscarLuzesCoroutine(int piscadas, float intervalo)
    {
        for (int i = 0; i < piscadas; i++)
        {
            containerDeLuzes.SetActive(false);
            yield return new WaitForSeconds(intervalo);
            containerDeLuzes.SetActive(true);
            yield return new WaitForSeconds(intervalo);
        }
        containerDeLuzes.SetActive(false);
    }

    public void RegistrarAnomaliaEncontrada()
    {
        anomaliasEncontradasNestaVolta++;
        Debug.Log("Anomalia encontrada! Progresso: " + anomaliasEncontradasNestaVolta + "/" + anomaliasSelecionadasParaEstaVolta.Count);
    }
}