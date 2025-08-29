using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager instance;

    [Header("Controle do Loop")]
    public int voltaAtual = 1;

    [Header("Configuração das Anomalias")]
    [Tooltip("Defina quantas anomalias aleatórias devem aparecer em cada volta (a partir da Volta 2).")]
    public int[] quantidadeAnomaliasPorVolta = new int[] { 3, 4, 5 };

    // Listas para organizar todas as anomalias
    private List<Anomalia> anomaliasFixas = new List<Anomalia>();
    private List<Anomalia> anomaliasAleatorias = new List<Anomalia>();
    // Lista "memória" que guarda as anomalias escolhidas para a volta atual
    private List<Anomalia> anomaliasSelecionadasParaEstaVolta = new List<Anomalia>();
    private int anomaliasEncontradasNestaVolta = 0;

    [Header("Referências da Cena")]
    [Tooltip("Arraste o GameObject da parede que bloqueia o caminho do destino para cá.")]
    public GameObject paredeDestino;
    public DemonioController demonio;
    public GameObject triggerFimPerseguicao;
    [Header("Pontos de Susto")]
    public Transform pontoSustoPorta;
    public Transform pontoSustoCorrida;
    public Transform pontoFuga;
    public Transform pontoInicioPerseguicao;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        OrganizarTodasAnomalias();
    }

    void Start()
    {
        // Configuração inicial da cena
        if (demonio != null) demonio.gameObject.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true); // Garante que a parede comece ativa
        if (triggerFimPerseguicao != null) triggerFimPerseguicao.SetActive(false);

        // Prepara e exibe as anomalias da primeira volta
        SelecionarNovasAnomaliasParaVolta();
        ExibirAnomaliasRestantes();
    }

    public void IniciarProximaVolta()
    {
        // Verifica se o jogador encontrou todas as anomalias da volta atual
        if (anomaliasEncontradasNestaVolta >= anomaliasSelecionadasParaEstaVolta.Count)
        {
            // SUCESSO: O jogador encontrou tudo
            Debug.Log("Volta " + voltaAtual + " completada com sucesso!");

            // Reseta o estado das anomalias da volta que acabou de passar
            foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
            {
                anomalia.ResetarIdentificacao();
            }

            // Avança para a próxima volta e aciona os eventos
            voltaAtual++;
            ExecutarEventosDaVolta();
            // Seleciona um NOVO conjunto de anomalias para a nova volta
            SelecionarNovasAnomaliasParaVolta();
        }
        else
        {
            // FALHA: O jogador não encontrou tudo, então a volta é reiniciada
            Debug.LogWarning("Nem todas as anomalias foram encontradas! Reiniciando a Volta " + voltaAtual + ".");
        }

        // Reexibe as anomalias restantes (seja da mesma volta ou da nova)
        ExibirAnomaliasRestantes();
    }

    // Método que lida com os eventos com script de cada volta
    private void ExecutarEventosDaVolta()
    {
        switch (voltaAtual)
        {
            case 2:
                // Esta é a linha correta. Ela desativa o objeto INTEIRO (parte visual e colisor).
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

    // Seleciona e "memoriza" quais anomalias farão parte da volta atual
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

    // Ativa na cena apenas as anomalias que ainda não foram encontradas pelo jogador
    void ExibirAnomaliasRestantes()
    {
        int anomaliasAtivasCount = 0;
        foreach (var anomalia in anomaliasSelecionadasParaEstaVolta)
        {
            if (anomalia.FoiIdentificada())
            {
                anomalia.DesativarAnomalia();
            }
            else
            {
                anomalia.AtivarAnomalia();
                anomaliasAtivasCount++;
            }
        }

        Debug.Log("VOLTA " + voltaAtual + " CONFIGURADA. Anomalias restantes para encontrar: " + anomaliasAtivasCount);
    }

    // Encontra e organiza todas as anomalias da cena no início do jogo
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

    // Chamado pelo script Anomalia.cs quando uma anomalia é identificada com sucesso
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
        Debug.LogWarning("!!! TODAS AS ANOMALIAS DA VOLTA " + voltaAtual + " FORAM ENCONTRADAS! Você pode prosseguir. !!!");
    }
}