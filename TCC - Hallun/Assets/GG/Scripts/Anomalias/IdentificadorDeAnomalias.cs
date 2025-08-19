using UnityEngine;
using UnityEngine.UI;

public class IdentificadorDeAnomalias : MonoBehaviour
{
    [Header("Referências Visuais e Sonoras")]
    // MUDANÇA IMPORTANTE: A referência agora é para o componente Image, não o GameObject.
    [Tooltip("Arraste o objeto de UI 'EfeitoCameraUI' para este campo.")]
    public Image efeitoCameraUI;

    [Header("Efeitos Sonoros")]
    [Tooltip("Som de estática que toca enquanto a câmera está ativa.")]
    public AudioClip somChiado;
    [Tooltip("Som que toca UMA VEZ ao segurar o botão direito.")]
    public AudioClip somZoomIn;
    [Tooltip("Som que toca UMA VEZ ao soltar o botão direito.")]
    public AudioClip somZoomOut;
    [Tooltip("Som de 'clique' ao identificar uma anomalia.")]
    public AudioClip somFoto;

    [Header("Configurações de Efeitos")]
    public float fovNormal = 60f;
    public float fovZoom = 40f;
    public float velocidadeZoom = 10f;
    [Tooltip("Velocidade com que a interface da câmera aparece e desaparece.")]
    public float velocidadeFade = 8f;

    [Header("Identificação de Anomalias")]
    public float distanciaMaxima = 10f;

    private Camera cameraPrincipal;
    private AudioSource audioSource;
    private Color corOriginalUI;

    void Awake()
    {
        cameraPrincipal = GetComponent<Camera>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }
        audioSource.playOnAwake = false;

        if (efeitoCameraUI != null)
        {
            // Garante que a UI comece invisível, mas ativa.
            efeitoCameraUI.gameObject.SetActive(true);
            corOriginalUI = efeitoCameraUI.color; // Guarda a cor original
            efeitoCameraUI.color = new Color(corOriginalUI.r, corOriginalUI.g, corOriginalUI.b, 0f); // Zera o alpha
        }

        if (cameraPrincipal != null) cameraPrincipal.fieldOfView = fovNormal;
    }

    void Update()
    {
        // --- LÓGICA DE SOM DE ZOOM ---
        // Toca o som de "zoom in" no exato frame em que o botão é pressionado.
        if (Input.GetMouseButtonDown(1))
        {
            TocarSom(somZoomIn);
        }
        // Toca o som de "zoom out" no exato frame em que o botão é solto.
        if (Input.GetMouseButtonUp(1))
        {
            TocarSom(somZoomOut);
        }

        // --- LÓGICA DE EFEITOS CONTÍNUOS (ENQUANTO SEGURA O BOTÃO) ---
        if (Input.GetMouseButton(1))
        {
            // Efeitos visuais (Zoom da câmera e Fade In da UI)
            cameraPrincipal.fieldOfView = Mathf.Lerp(cameraPrincipal.fieldOfView, fovZoom, Time.deltaTime * velocidadeZoom);
            AtualizarFadeUI(1f); // Alvo alpha = 1 (visível)

            // Efeito sonoro contínuo (Chiado)
            ControlarSomChiado(true);

            // Ação de identificar a anomalia
            if (Input.GetMouseButtonDown(0))
            {
                IdentificarAnomalia();
            }
        }
        else
        {
            // Reverte os efeitos visuais (Zoom out e Fade Out da UI)
            cameraPrincipal.fieldOfView = Mathf.Lerp(cameraPrincipal.fieldOfView, fovNormal, Time.deltaTime * velocidadeZoom);
            AtualizarFadeUI(0f); // Alvo alpha = 0 (invisível)

            // Para o som contínuo (Chiado)
            ControlarSomChiado(false);
        }
    }

    // Função para fazer o fade in/out da UI da câmera
    void AtualizarFadeUI(float alphaAlvo)
    {
        if (efeitoCameraUI != null)
        {
            // Pega a cor atual
            Color corAtual = efeitoCameraUI.color;
            // Calcula o novo alpha suavemente
            float novoAlpha = Mathf.Lerp(corAtual.a, alphaAlvo, Time.deltaTime * velocidadeFade);
            // Aplica a nova cor com o alpha atualizado
            efeitoCameraUI.color = new Color(corAtual.r, corAtual.g, corAtual.b, novoAlpha);
        }
    }

    void IdentificarAnomalia()
    {
        Ray raio = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(raio, out hitInfo, distanciaMaxima))
        {
            Anomalia anomaliaDetectada = hitInfo.collider.GetComponent<Anomalia>();
            if (anomaliaDetectada != null)
            {
                // Toca o som da foto ANTES de identificar
                TocarSom(somFoto);
                anomaliaDetectada.Identificar();
            }
        }
    }

    // Funções auxiliares de som
    void ControlarSomChiado(bool tocar)
    {
        if (audioSource != null && somChiado != null)
        {
            if (tocar && !audioSource.isPlaying)
            {
                audioSource.clip = somChiado;
                audioSource.loop = true;
                audioSource.Play();
            }
            else if (!tocar && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void TocarSom(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // PlayOneShot é perfeito para sons que não devem se sobrepor ou parar o som principal
            audioSource.PlayOneShot(clip);
        }
    }
}