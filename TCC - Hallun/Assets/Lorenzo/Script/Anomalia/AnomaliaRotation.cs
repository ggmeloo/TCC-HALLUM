using UnityEngine;

public class AnomaliaDeRotacao : AnomaliaBase
{
    [Tooltip("A rotação que o objeto terá quando a anomalia estiver ativa.")]
    public Vector3 rotacaoAnomala;

    private Quaternion rotacaoOriginal;

    void Awake()
    {
        // Salva a rotação inicial do objeto para saber como voltar ao normal.
        rotacaoOriginal = transform.localRotation;
    }

    public override void AtivarAnomalia()
    {
        anomaliaAtiva = true;
        transform.localRotation = Quaternion.Euler(rotacaoAnomala);
        Debug.Log(gameObject.name + " ativou sua anomalia de rotação.");
    }

    public override void DesativarAnomalia()
    {
        anomaliaAtiva = false;
        transform.localRotation = rotacaoOriginal;
    }
}