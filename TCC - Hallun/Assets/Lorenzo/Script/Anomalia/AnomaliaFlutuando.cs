// AnomaliaFlutuante.cs

using UnityEngine;

public class AnomaliaFlutuante : AnomaliaBase
{
    [Tooltip("O quão alto o objeto vai flutuar a partir de sua posição inicial.")]
    public float alturaFlutuacao = 0.5f;
    [Tooltip("A velocidade com que o objeto sobe e desce.")]
    public float velocidadeFlutuacao = 1f;

    private Vector3 posicaoOriginal;

    void Awake()
    {
        // Salva a posição inicial do objeto.
        posicaoOriginal = transform.position;
    }

    public override void AtivarAnomalia()
    {
        anomaliaAtiva = true;
        Debug.Log(gameObject.name + " começou a flutuar.");
    }

    public override void DesativarAnomalia()
    {
        anomaliaAtiva = false;
        // Garante que, ao ser desativada, volte exatamente para o ponto de partida.
        transform.position = posicaoOriginal;
    }

    // O Update só executa a lógica de flutuação se a anomalia estiver ativa.
    void Update()
    {
        if (anomaliaAtiva)
        {
            // Usa uma onda senoidal para criar um movimento suave de sobe e desce.
            float novoY = posicaoOriginal.y + (Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao);
            transform.position = new Vector3(posicaoOriginal.x, novoY, posicaoOriginal.z);
        }
    }
}