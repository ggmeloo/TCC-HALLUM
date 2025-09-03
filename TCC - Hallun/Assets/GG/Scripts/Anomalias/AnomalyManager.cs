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

    // --- LÓGICA DE LISTAS CORRIGIDA ---
    private List<Anomalia> anomaliasFixas = new List<Anomalia>();
    // "Piscina" de anomalias que ainda não foram usadas.
    private List<Anomalia> anomaliasAleatoriasDisponiveis = new List<Anomalia>();
    // "Cemitério" de anomalias que já foram usadas.
    private List<Anomalia> anomaliasAleatoriasJaUsadas = new List<Anomalia>();

    private List<Anomalia> anomaliasSelecionadasParaEstaVolta = new List<Anomalia>();
    private int anomaliasEncontradasNestaVolta = 0;

    [Header("Referências da Cena")]
    public DemonioController demonio;
    public GameObject paredeDestino;
    public GameObject triggerFimPerseguicao;
    [Tooltip("Arraste o objeto pai que contém todos os textos do tutorial para cá.")]
    public GameObject textosDoTutorial;

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
        if (textosDoTutorial != null) textosDoTutorial.SetActive(true);

        SelecionarNovasAnomaliasParaVolta();
        ExibirAnomaliasRestantes();
    }

    public void IniciarProximaVolta()
    {
        if (voltaAtual == 1)
        {
            if (paredeDestino != null && paredeDestino.activeSelf) paredeDestino.SetActive(false);
            if (textosDoTutorial != null && textosDoTutorial.activeSelf) textosDoTutorial.SetActive(false);
        }

        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            Debug.Log("Volta " + voltaAtual + " completada com sucesso!");

            // Move as anomalias da volta bem-sucedida para a lista de "já usadas".
            foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
            {
                if (anomalia.tipo == Anomalia.TipoAnomalia.Aleatoria)
                {
                    anomaliasAleatoriasDisponiveis.Remove(anomalia);
                    anomaliasAleatoriasJaUsadas.Add(anomalia);
                }
                anomalia.ResetarIdentificacao();
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
    }

    // --- LÓGICA DE EXIBIÇÃO CORRIGIDA ---
    void ExibirAnomaliasRestantes()
    {
        // PASSO 1: LIMPEZA TOTAL - Desativa TODAS as anomalias possíveis para garantir uma cena limpa.
        foreach (var anomalia in anomaliasFixas) anomalia.DesativarAnomalia();
        foreach (var anomalia in anomaliasAleatoriasDisponiveis) anomalia.DesativarAnomalia();
        foreach (var anomalia in anomaliasAleatoriasJaUsadas) anomalia.DesativarAnomalia();

        // PASSO 2: ATIVAÇÃO SELETIVA - Ativa apenas as anomalias que ainda não foram encontradas na seleção desta volta.
        int anomaliasAtivasCount = 0;
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            if (!anomalia.FoiIdentificada())
            {
                anomalia.AtivarAnomalia();
                anomaliasAtivasCount++;
            }
        }
        Debug.Log("VOLTA " + voltaAtual + " CONFIGURADA. Anomalias restantes: " + anomaliasAtivasCount);
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

            // Sorteia apenas da piscina de anomalias que ainda não foram usadas.
            var anomaliasEmbaralhadas = anomaliasAleatoriasDisponiveis.OrderBy(a => Random.value).ToList();

            for (int i = 0; i < quantidadeParaAtivar && i < anomaliasEmbaralhadas.Count; i++)
            {
                anomaliasSelecionadasParaEstaVolta.Add(anomaliasEmbaralhadas[i]);
            }
        }
    }

    // O resto do script permanece o mesmo...

    private void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2:
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

    void OrganizarTodasAnomalias()
    {
        Anomalia[] todasAnomalias = FindObjectsByType<Anomalia>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var anomalia in todasAnomalias)
        {
            if (anomalia.tipo == Anomalia.TipoAnomalia.Fixa) anomaliasFixas.Add(anomalia);
            else anomaliasAleatoriasDisponiveis.Add(anomalia);
            anomalia.DesativarAnomalia();
        }
    }

    public void RegistrarAnomaliaEncontrada()
    {
        anomaliasEncontradasNestaVolta++;
        Debug.Log("Anomalia encontrada! Progresso: " + anomaliasEncontradasNestaVolta + "/" + anomaliasSelecionadasParaEstaVolta.Count);
        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            TodasAnomaliasEncontradas();
        }
    }

    private void TodasAnomaliasEncontradas()
    {
        Debug.LogWarning("!!! TODAS AS ANOMALIAS DA VOLTA " + voltaAtual + " FORAM ENCONTRADAS! Você pode prosseguir. !!!");
    }
}