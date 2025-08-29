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
    public GameObject paredeDestino; // A referência para a parede
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
        if (paredeDestino != null) paredeDestino.SetActive(true); // Garante que a parede comece ativa
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);

        SelecionarNovasAnomaliasParaVolta();
        ExibirAnomaliasRestantes();
    }

    public void IniciarProximaVolta()
    {
        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            Debug.Log("Volta " + voltaAtual + " completada com sucesso!");

            foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
            {
                anomalia.ResetarIdentificacao();
            }

            voltaAtual++;
            ExecutarEventosDaVolta(); // Os eventos, incluindo desativar a parede, são chamados aqui
            SelecionarNovasAnomaliasParaVolta();
        }
        else
        {
            Debug.LogWarning("Nem todas as anomalias foram encontradas! Reiniciando a Volta " + voltaAtual + ".");
        }

        ExibirAnomaliasRestantes();
    }

    // --- MÉTODO CORRIGIDO ---
    private void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2:
                // Esta é a linha correta. Ela desativa o objeto INTEIRO (Mesh Renderer e Collider).
                if (paredeDestino != null)
                {
                    paredeDestino.SetActive(false);
                    Debug.Log("Parede do Destino foi DESATIVADA.");
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

    // O resto do script permanece o mesmo...

    void SelecionarNovasAnomaliasParaVolta()
    {
        anomaliasEncontradasNestaVolta = 0;
        anomaliasSelecionadasParaEstaVolta.Clear();
        if (voltaAtual == 1) { anomaliasSelecionadasParaEstaVolta.AddRange(anomaliasFixas); }
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
        int anomaliasAtivasCount = 0;
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            if (anomalia.FoiIdentificada()) { anomalia.DesativarAnomalia(); }
            else { anomalia.AtivarAnomalia(); anomaliasAtivasCount++; }
        }
        Debug.Log("VOLTA " + voltaAtual + " CONFIGURADA. Anomalias restantes: " + anomaliasAtivasCount);
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
        Debug.Log("Anomalia encontrada! Progresso: " + anomaliasEncontradasNestaVolta + "/" + anomaliasSelecionadasParaEstaVolta.Count);
        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            TodasAnomaliasEncontradas();
        }
    }

    private void TodasAnomaliasEncontradas()
    {
        Debug.LogWarning("!!! TODAS AS ANOMALIAS DA VOLTA " + voltaAtual + " FORAM ENCONTRADAS! Prossiga. !!!");
    }
}