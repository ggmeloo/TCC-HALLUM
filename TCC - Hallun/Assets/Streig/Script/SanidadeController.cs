using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanidadeController : MonoBehaviour
{
    [Header("Configurações de Sanidade")]
    public float sanidadeMaxima = 100f;
    public float sanidadeAtual = 100f;
    public float taxaDecaimento = 0.5f;
    public float sanidadePorPilula = 10f;

    [Header("UI")]
    public Slider barraSanidade;

    [Header("Efeitos URP")]
    public Volume volumeSanidade;

    [Header("Configurações de Efeitos Visuais (Gerais)")]
    public float maxIntensidadeDistorcao = -0.5f;
    public float maxAberracaoCromatica = 1f;
    public float maxFilmGrain = 0.5f;
    public float maxVignette = 0.5f;

    [Header("Configurações de Blur (Desfoque)")]
    [Tooltip("Distância mínima do foco para o efeito de blur.")]
    public float minFocusDistance = 0.1f;
    [Tooltip("Valor máximo da Abertura da lente para o desfoque.")]
    public float maxAperture = 32f;
    [Tooltip("Intensidade máxima do Desfoque de Movimento.")]
    [Range(0, 1)]
    public float maxMotionBlurIntensity = 0.5f;

    [Header("Curva de Aceleração do Blur")]
    public AnimationCurve blurCurve;

    [Header("Efeitos Visuais de Tela")]
    public Image efeitoTela;
    public Color corAltaSanidade = new Color(0.8f, 0.8f, 1f, 0.1f);
    public Color corBaixaSanidade = new Color(0.8f, 0.4f, 0.4f, 0.4f);

    [Header("Efeitos Sonoros")]
    public AudioClip somCoracao;
    private AudioSource audioSource;

    [Header("Controles de Teste")]
    [Tooltip("Tecla para reduzir sanidade (teste)")]
    public KeyCode teclaReduzirSanidade = KeyCode.J;
    [Tooltip("Tecla para restaurar sanidade completa (teste)")]
    public KeyCode teclaRestaurarSanidade = KeyCode.H;
    [Tooltip("Quantidade de sanidade reduzida ao pressionar a tecla de teste")]
    public float reducaoTeste = 10f;

    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;
    private Vignette vignette;
    private DepthOfField depthOfField;
    private MotionBlur motionBlur;
    private float sanidadeAnterior;

    // Valores iniciais para reset
    private float initialFocusDistance;
    private float initialAperture;
    private float initialMotionBlurIntensity;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sanidadeAtual = sanidadeMaxima;
        sanidadeAnterior = sanidadeAtual;
        ConfigurarEfeitosURP();
        SalvarValoresIniciais();
        ResetarEfeitosParaSanidadeMaxima();
        AtualizarUI();

        Debug.Log("Controles de teste:");
        Debug.Log($"{teclaReduzirSanidade} - Reduzir {reducaoTeste} de sanidade");
        Debug.Log($"{teclaRestaurarSanidade} - Restaurar sanidade completa");
    }

    void ConfigurarEfeitosURP()
    {
        if (volumeSanidade.profile != null)
        {
            volumeSanidade.profile.TryGet(out lensDistortion);
            volumeSanidade.profile.TryGet(out chromaticAberration);
            volumeSanidade.profile.TryGet(out filmGrain);
            volumeSanidade.profile.TryGet(out vignette);
            volumeSanidade.profile.TryGet(out depthOfField);
            volumeSanidade.profile.TryGet(out motionBlur);
        }
    }

    void SalvarValoresIniciais()
    {
        if (depthOfField != null)
        {
            initialFocusDistance = depthOfField.focusDistance.value;
            initialAperture = depthOfField.aperture.value;
        }

        if (motionBlur != null)
        {
            initialMotionBlurIntensity = motionBlur.intensity.value;
        }
    }

    void ResetarEfeitosParaSanidadeMaxima()
    {
        // Resetar todos os efeitos para valores normais
        if (depthOfField != null)
        {
            depthOfField.focusDistance.value = initialFocusDistance;
            depthOfField.aperture.value = initialAperture;
        }

        if (motionBlur != null)
        {
            motionBlur.intensity.value = initialMotionBlurIntensity;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0;
        }

        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = 0;
        }

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 0;
        }

        if (filmGrain != null)
        {
            filmGrain.intensity.value = 0;
        }

        if (efeitoTela != null)
        {
            efeitoTela.color = corAltaSanidade;
        }
    }

    void Update()
    {
        // Decaimento natural da sanidade
        sanidadeAtual -= taxaDecaimento * Time.deltaTime;
        sanidadeAtual = Mathf.Clamp(sanidadeAtual, 0, sanidadeMaxima);

        // Controles de teste
        if (Input.GetKeyDown(teclaReduzirSanidade))
        {
            ReduzirSanidadeTeste();
            Debug.Log($"Sanidade reduzida: {sanidadeAtual}/{sanidadeMaxima}");
        }

        if (Input.GetKeyDown(teclaRestaurarSanidade))
        {
            RestaurarSanidadeCompleta();
            Debug.Log("Sanidade restaurada completamente!");
        }

        // Atualizar efeitos visuais apenas se houver mudança
        if (Mathf.Abs(sanidadeAtual - sanidadeAnterior) > 0.01f)
        {
            AtualizarUI();
            AtualizarEfeitosVisuais();
            sanidadeAnterior = sanidadeAtual;
        }
    }

    void AtualizarEfeitosVisuais()
    {
        float porcentagemSanidade = sanidadeAtual / sanidadeMaxima;

        // Só aplicar efeitos de blur quando a sanidade estiver abaixo de 50%
        if (sanidadeAtual > sanidadeMaxima / 2f)
        {
            // Sanidade alta - manter efeitos mínimos
            if (depthOfField != null)
            {
                depthOfField.focusDistance.value = initialFocusDistance;
                depthOfField.aperture.value = initialAperture;
            }

            if (motionBlur != null)
            {
                motionBlur.intensity.value = initialMotionBlurIntensity;
            }
        }
        else
        {
            // Calcular intensidade do blur apenas quando sanidade <= 50%
            float progressoBlur = 1 - (sanidadeAtual / (sanidadeMaxima / 2f));
            float intensidadeBlur = blurCurve.Evaluate(progressoBlur);

            // Aplicar efeitos de blur
            if (depthOfField != null)
            {
                depthOfField.focusDistance.value = Mathf.Lerp(initialFocusDistance, minFocusDistance, intensidadeBlur);
                depthOfField.aperture.value = Mathf.Lerp(initialAperture, maxAperture, intensidadeBlur);
            }

            if (motionBlur != null)
            {
                motionBlur.intensity.value = Mathf.Lerp(initialMotionBlurIntensity, maxMotionBlurIntensity, intensidadeBlur);
            }
        }

        // Efeitos gerais (aplicados gradualmente desde o início)
        float intensidadeGeral = 1 - porcentagemSanidade;

        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0, maxVignette, intensidadeGeral);
        }

        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = Mathf.Lerp(0, maxIntensidadeDistorcao, intensidadeGeral);
        }

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0, maxAberracaoCromatica, intensidadeGeral);
        }

        if (filmGrain != null)
        {
            filmGrain.intensity.value = Mathf.Lerp(0, maxFilmGrain, intensidadeGeral);
        }

        // Efeito de tela
        if (efeitoTela != null)
        {
            efeitoTela.color = Color.Lerp(corAltaSanidade, corBaixaSanidade, intensidadeGeral);
        }

        // Efeitos sonoros
        if (sanidadeAtual < 30f)
        {
            if (!audioSource.isPlaying && somCoracao != null)
            {
                AtivarEfeitoLoucura();
            }
        }
        else if (audioSource.isPlaying)
        {
            DesativarEfeitoLoucura();
        }
    }

    // Funções de teste - mantidas como você queria
    void ReduzirSanidadeTeste()
    {
        sanidadeAtual = Mathf.Max(sanidadeAtual - reducaoTeste, 0);
    }

    void RestaurarSanidadeCompleta()
    {
        sanidadeAtual = sanidadeMaxima;
    }

    public void RecuperarSanidade()
    {
        sanidadeAtual = Mathf.Min(sanidadeAtual + sanidadePorPilula, sanidadeMaxima);
    }

    void AtualizarUI()
    {
        if (barraSanidade != null)
            barraSanidade.value = sanidadeAtual / sanidadeMaxima;
    }

    void AtivarEfeitoLoucura()
    {
        if (audioSource != null && somCoracao != null)
        {
            audioSource.PlayOneShot(somCoracao);
        }
    }

    void DesativarEfeitoLoucura()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}