using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using TMPro;

public class SanidadeController : MonoBehaviour
{
    [Header("Configurações de Sanidade")]
    public float sanidadeMaxima = 100f;
    public float sanidadeAtual = 100f;
    public float taxaDecaimento = 0.5f;
    public float sanidadePorPilula = 10f;
    public bool podePerderSanidade = false;

    [Header("UI")]
    public Slider barraSanidade;
    public TextMeshProUGUI avisoSanidadeText;
    public float duracaoPiscada = 0.2f;
    public int quantidadePiscadas = 3;
    private bool aviso50Mostrado = false;

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
    [Range(0, 10000)]
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

    private float initialFocusDistance;
    private float initialAperture;
    private float initialMotionBlurIntensity;

    void Start()
    {
        // --- ALTERADO: Agora o script não assume que o AudioSource existe ---
        // Ele tenta pegar o componente. Se não encontrar, audioSource continuará nulo.
        audioSource = GetComponent<AudioSource>();

        sanidadeAtual = sanidadeMaxima;
        sanidadeAnterior = sanidadeAtual;
        ConfigurarEfeitosURP();
        SalvarValoresIniciais();
        ResetarEfeitosParaSanidadeMaxima();
        AtualizarUI();

        if (avisoSanidadeText != null) avisoSanidadeText.gameObject.SetActive(false);

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
        if (podePerderSanidade)
        {
            sanidadeAtual -= taxaDecaimento * Time.deltaTime;
            sanidadeAtual = Mathf.Clamp(sanidadeAtual, 0, sanidadeMaxima);
        }

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

        if (Mathf.Abs(sanidadeAtual - sanidadeAnterior) > 0.01f)
        {
            AtualizarUI();
            AtualizarEfeitosVisuais();

            if (sanidadeAtual <= sanidadeMaxima * 0.5f && !aviso50Mostrado)
            {
                StartCoroutine(PiscarTextoDeAviso());
                aviso50Mostrado = true;
            }

            sanidadeAnterior = sanidadeAtual;
        }

        if (sanidadeAtual > sanidadeMaxima * 0.5f && aviso50Mostrado)
        {
            aviso50Mostrado = false;
        }
    }

    IEnumerator PiscarTextoDeAviso()
    {
        if (avisoSanidadeText != null)
        {
            avisoSanidadeText.text = "Atenção sua sanidade está ficando baixa!";

            for (int i = 0; i < quantidadePiscadas; i++)
            {
                avisoSanidadeText.gameObject.SetActive(true);
                yield return new WaitForSeconds(duracaoPiscada);
                avisoSanidadeText.gameObject.SetActive(false);
                yield return new WaitForSeconds(duracaoPiscada);
            }

            avisoSanidadeText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            avisoSanidadeText.gameObject.SetActive(false);
        }
    }

    void AtualizarEfeitosVisuais()
    {
        float porcentagemSanidade = sanidadeAtual / sanidadeMaxima;

        if (sanidadeAtual > sanidadeMaxima / 2f)
        {
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
            float progressoBlur = 1 - (sanidadeAtual / (sanidadeMaxima / 2f));
            float intensidadeBlur = blurCurve.Evaluate(progressoBlur);

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

        if (efeitoTela != null)
        {
            efeitoTela.color = Color.Lerp(corAltaSanidade, corBaixaSanidade, intensidadeGeral);
        }

        // --- ALTERADO: Adicionamos "audioSource != null" para evitar o erro ---
        // O código agora só tenta tocar o som SE o componente de áudio existir.
        if (sanidadeAtual < 30f)
        {
            if (audioSource != null && !audioSource.isPlaying && somCoracao != null)
            {
                AtivarEfeitoLoucura();
            }
        }
        else if (audioSource != null && audioSource.isPlaying)
        {
            DesativarEfeitoLoucura();
        }
    }

    void ReduzirSanidadeTeste()
    {
        sanidadeAtual = Mathf.Max(sanidadeAtual - reducaoTeste, 0);
    }

    void RestaurarSanidadeCompleta()
    {
        sanidadeAtual = sanidadeMaxima;
        aviso50Mostrado = false;
        if (avisoSanidadeText != null) avisoSanidadeText.gameObject.SetActive(false);
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
        // --- ALTERADO: Verificação dupla para máxima segurança ---
        if (audioSource != null && somCoracao != null)
        {
            audioSource.PlayOneShot(somCoracao);
        }
    }

    void DesativarEfeitoLoucura()
    {
        // --- ALTERADO: Verificação dupla para máxima segurança ---
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}