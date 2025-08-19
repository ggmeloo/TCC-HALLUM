using UnityEngine;
using System.Collections; // Necessário para Corrotinas, embora não estejam mais neste script.

public class AnomalyManager : MonoBehaviour
{
    // Variáveis que pertencem à classe inteira
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
    public Transform pontoInicioPerseguicao; // Novo ponto!

    // As variáveis de anomalia também pertencem à classe
    private int totalDeAnomaliasNaCena;
    private int anomaliasEncontradas = 0;

    // O método Awake
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // O método Start
    void Start()
    {
        if (demonio != null) demonio.gameObject.SetActive(false);
        if (paredeDestino != null) paredeDestino.SetActive(true);

        totalDeAnomaliasNaCena = FindObjectsByType<Anomalia>(FindObjectsSortMode.None).Length;
        Debug.Log("VOLTA " + voltaAtual + " INICIADA.");

        // Garante que o gatilho de fim comece o jogo DESATIVADO.
        if (triggerFimPerseguicao != null)
        {
            triggerFimPerseguicao.SetActive(false);
        }

    }

    // O método IniciarProximaVolta (agora no lugar correto)
    public void IniciarProximaVolta()
    {
        voltaAtual++;
        Debug.Log("VOLTA " + voltaAtual + " INICIADA.");

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
                break;
        }
    }

    // O método RegistrarAnomaliaEncontrada
    public void RegistrarAnomaliaEncontrada() { /* seu código aqui */ }

    // O método TodasAnomaliasEncontradas
    private void TodasAnomaliasEncontradas() { /* seu código aqui */ }
}