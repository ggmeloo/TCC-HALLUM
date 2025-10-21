using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro; // <-- 1. ADICIONAR ESTA LINHA PARA USAR TEXTMESHPRO

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
    public SanidadeController sanidadeController;

    // ▼▼▼ 2. ADICIONAR ESTA LINHA ▼▼▼
    [Header("Referências da UI")]
    public TextMeshProUGUI textoAnomalias; // Referência para o texto do contador

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
        if (segundoDemonio != null) segundoDemonio.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true);
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);
        if (textosDoTutorial != null) textosDoTutorial.SetActive(false);
        if (containerDeLuzes != null) containerDeLuzes.SetActive(true);

        ExibirAnomaliasRestantes();
        Debug.Log("VOLTA 1 (Reconhecimento) INICIADA.");

        AtualizarContadorAnomalias(); // <-- 4. CHAMAR A FUNÇÃO AQUI
    }

    public void ProcessarMudancaDeVolta()
    {
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

            AtualizarContadorAnomalias(); // <-- 4. CHAMAR A FUNÇÃO AQUI
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
            case 2:
                Debug.Log("INICIANDO EVENTOS DA VOLTA 2.");
                if (containerDeLuzes != null) StartCoroutine(PiscarLuzesCoroutine());
                if (paredeDestino != null) paredeDestino.SetActive(false);
                if (textosDoTutorial != null) textosDoTutorial.SetActive(true);
                if (sanidadeController != null) sanidadeController.podePerderSanidade = true;
                break;
            case 3:
                if (textosDoTutorial != null) textosDoTutorial.SetActive(false);
                if (demonio != null) demonio.AparecerEAssombrar(pontoSustoPorta);
                break;
            case 4:
                if (demonio != null) demonio.Desaparecer();
                if (segundoDemonio != null) segundoDemonio.SetActive(true);
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

        AtualizarContadorAnomalias(); // <-- 4. CHAMAR A FUNÇÃO AQUI
    }

    // ▼▼▼ 3. ADICIONAR ESTA FUNÇÃO INTEIRA ▼▼▼
    void AtualizarContadorAnomalias()
    {
        if (textoAnomalias == null) return; // Proteção para não dar erro

        // Se for a volta 1 ou se não houver anomalias, esconde o texto
        if (voltaAtual <= 1 || anomaliasSelecionadasParaEstaVolta.Count == 0)
        {
            textoAnomalias.gameObject.SetActive(false);
        }
        else // Caso contrário, exibe e atualiza
        {
            textoAnomalias.gameObject.SetActive(true);
            textoAnomalias.text = $"Anomalias: {anomaliasEncontradasNestaVolta} / {anomaliasSelecionadasParaEstaVolta.Count}";
        }
    }
}