using UnityEngine;

public class ControleLanterna : MonoBehaviour
{
    [Header("Configurações da Lanterna")]
    [Tooltip("Arraste o objeto de Luz da Lanterna para este campo.")]
    public Light luzDaLanterna;

    [Header("Efeitos Sonoros")]
    [Tooltip("O som de clique ao ligar a lanterna.")]
    public AudioClip somLigar;
    [Tooltip("O som de clique ao desligar a lanterna.")]
    public AudioClip somDesligar;

    private AudioSource audioSource;

    void Start()
    {
        if (luzDaLanterna == null)
        {
            Debug.LogError("ERRO CRÍTICO: A 'Luz da Lanterna' não foi atribuída no Inspetor.", this);
            this.enabled = false;
            return;
        }

        // --- LINHA DE CORREÇÃO ADICIONADA ---
        // Garante que o GameObject que contém a luz esteja sempre ATIVO.
        // Isso previne o problema de o "lustre" estar desligado.
        luzDaLanterna.gameObject.SetActive(true);

        // Agora, com o GameObject garantidamente ativo, apenas controlamos o componente de luz.
        luzDaLanterna.enabled = false;

        // Pega ou adiciona um componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AlternarEstadoLanterna();
        }
    }

    private void AlternarEstadoLanterna()
    {
        // Esta parte já estava correta.
        luzDaLanterna.enabled = !luzDaLanterna.enabled;

        if (luzDaLanterna.enabled)
        {
            if (somLigar != null) audioSource.PlayOneShot(somLigar);
        }
        else
        {
            if (somDesligar != null) audioSource.PlayOneShot(somDesligar);
        }
    }
}