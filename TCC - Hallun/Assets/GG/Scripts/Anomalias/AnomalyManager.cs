using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager instance;

    [Header("Controle do Loop")]
    public int voltaAtual = 1;

    [Header("Configuração das Anomalias")]
    [Tooltip("Defina quantas anomalias aleatórias devem aparecer em cada volta (a partir da Volta 3).")]
    public int[] quantidadeAnomaliasPorVolta = new int[] { 3, 4, 5 };

    [Header("Configuração da Pane de Energia")]
    public int numeroDePiscadas = 5;
    public float tempoEntrePiscadas = 0.1f;

    // Listas para organizar as anomalias
    private List<Anomalia> anomaliasFixas = new List<Anomalia>();
    private List<Anomalia> anomaliasAleatoriasDisponiveis = new List<Anomalia>();
    private List<Anomalia> anomaliasAleatoriasJaUsadas = new List<Anomalia>();
    private List<Anomalia> anomaliasSelecionadasParaEstaVolta = new List<Anomalia>();
    private int anomaliasEncontradasNestaVolta = 0;

    [Header("Referências da Cena")]
    public DemonioController demonio;
    [Tooltip("Arraste o GameObject do segundo demônio (que aparece na volta 4) para cá.")]
    public GameObject segundoDemonio;
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
        OrganizarTodasAnomalias();
    }

    void Start()
    {
        // Configuração inicial para a Volta 1 (Reconhecimento)
        if (demonio != null) demonio.gameObject.SetActive(false);
        if (segundoDemonio != null) segundoDemonio.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true);
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);
        if (textosDoTutorial != null) textosDoTutorial.SetActive(false);
        if (containerDeLuzes != null) containerDeLuzes.SetActive(true);

        ExibirAnomaliasRestantes();
        Debug.Log("VOLTA 1 (Reconhecimento) INICIADA.");
    }

    public void ProcessarMudancaDeVolta()
    {
        // A Volta 1 sempre avança. As outras, só se o jogador encontrar tudo.
        if (voltaAtual == 1 || anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            Debug.Log("Volta " + voltaAtual + " completada com sucesso!");

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
                if (containerDeLuzes != null) StartCoroutine(PiscarLuzesCoroutine());
                if (paredeDestino != null) paredeDestino.SetActive(false);
                if (textosDoTutorial != null) textosDoTutorial.SetActive(true);
                break;
            case 3: // Susto da Porta
                if (demonio != null) demonio.AparecerEAssombrar(pontoSustoPorta);
                break;
            case 4: // Ativa o segundo demônio e remove o primeiro da porta
                if (demonio != null) demonio.Desaparecer();
                if (segundoDemonio != null)
                {
                    segundoDemonio.SetActive(true);
                    Debug.Log("Segundo demônio foi ativado!");
                }
                // A perseguição com o primeiro demônio será ativada pelo TriggerInicioPerseguicao
                break;
        }
    }

    private IEnumerator PiscarLuzesCoroutine()
    {
        for (int i = 0; i < numeroDePiscadas; i++)
        {
            containerDeLuzes.SetActive(false);
            yield return new WaitForSeconds(tempoEntrePiscadas);
            containerDeLuzes.SetActive(true);
            yield return new WaitForSeconds(tempoEntrePiscadas);
        }
        containerDeLuzes.SetActive(false);
    }

    void SelecionarNovasAnomaliasParaVolta()
    {
        anomaliasEncontradasNestaVolta = 0;
        anomaliasSelecionadasParaEstaVolta.Clear();

        if (voltaAtual == 2) // Volta do Tutorial
        {
            anomaliasSelecionadasParaEstaVolta.AddRange(anomaliasFixas);
        }
        else if (voltaAtual > 2) // Voltas Aleatórias
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
        foreach (var anomalia in anomaliasFixas) anomalia.DesativarAnomalia();
        foreach (var anomalia in anomaliasAleatoriasDisponiveis) anomalia.DesativarAnomalia();
        foreach (var anomalia in anomaliasAleatoriasJaUsadas) anomalia.DesativarAnomalia();

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
    }
}