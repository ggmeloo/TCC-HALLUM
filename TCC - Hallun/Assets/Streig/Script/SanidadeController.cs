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

    [Header("Tutorial da Pílula")]
    public TutorialSequencia sequenciaTutorialPilula;
    private bool tutorialDaPilulaJaFoiMostrado = false;

    // NOVO: Flag para garantir que as pílulas só sejam criadas uma vez por rodada.
    private bool pillsHaveBeenSpawned = false;

    [Header("UI")]
    public Image painelVisaoTurva;
    [Range(0, 1)]
    public float maxOpacidadePainel = 0.85f;
    public float velocidadeTransicaoPainel = 2f;

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
    public float minFocusDistance = 0.1f;
    public float maxAperture = 32f;
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
    public KeyCode teclaReduzirSanidade = KeyCode.J;
    public KeyCode teclaRestaurarSanidade = KeyCode.H;
    public float reducaoTeste = 10f;

    // ... (demais variáveis privadas) ...
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
    private float opacidadeAlvoPainel;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sanidadeAtual = sanidadeMaxima;
        sanidadeAnterior = sanidadeAtual;
        ConfigurarEfeitosURP();
        SalvarValoresIniciais();
        ResetarEfeitosParaSanidadeMaxima();

        if (painelVisaoTurva != null)
        {
            Color painelColor = painelVisaoTurva.color;
            painelColor.a = 0;
            painelVisaoTurva.color = painelColor;
            opacidadeAlvoPainel = 0;
        }
        if (avisoSanidadeText != null) avisoSanidadeText.gameObject.SetActive(false);

        Debug.Log("Controles de teste:");
        Debug.Log($"{teclaReduzirSanidade} - Reduzir {reducaoTeste} de sanidade");
        Debug.Log($"{teclaRestaurarSanidade} - Restaurar sanidade completa");
    }

    void Update()
    {
        // NOVO: Bloco de código que verifica se é hora de criar as pílulas.
        // Se o gameplay começou (podePerderSanidade) E as pílulas ainda não foram criadas...
        if (podePerderSanidade && !pillsHaveBeenSpawned)
        {
            // ...chama o spawner para criar a primeira leva de pílulas.
            BatchPillSpawner.instance.SpawnNewBatch();

            // ...e levanta o "flag" para que este código não rode novamente.
            pillsHaveBeenSpawned = true;
        }

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

        if (painelVisaoTurva != null)
        {
            Color painelColor = painelVisaoTurva.color;
            painelColor.a = Mathf.Lerp(painelColor.a, opacidadeAlvoPainel, velocidadeTransicaoPainel * Time.deltaTime);
            painelVisaoTurva.color = painelColor;
        }
    }

    // ... (O resto do seu script continua exatamente igual) ...
    // ... (ConfigurarEfeitosURP, SalvarValoresIniciais, RestaurarSanidadeCompleta, etc.) ...

    void ConfigurarEfeitosURP()
    {
        if (volumeSanidade != null && volumeSanidade.profile != null)
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
        if (motionBlur != null) motionBlur.intensity.value = initialMotionBlurIntensity;
        if (vignette != null) vignette.intensity.value = 0;
        if (lensDistortion != null) lensDistortion.intensity.value = 0;
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0;
        if (filmGrain != null) filmGrain.intensity.value = 0;
        if (efeitoTela != null) efeitoTela.color = corAltaSanidade;
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
        }
    }

    void AtualizarEfeitosVisuais()
    {
        float porcentagemSanidade = sanidadeAtual / sanidadeMaxima;
        float intensidadeGeral = 1 - porcentagemSanidade;

        if (painelVisaoTurva != null)
        {
            opacidadeAlvoPainel = intensidadeGeral * maxOpacidadePainel;
        }

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

        if (vignette != null) vignette.intensity.value = Mathf.Lerp(0, maxVignette, intensidadeGeral);
        if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(0, maxIntensidadeDistorcao, intensidadeGeral);
        if (chromaticAberration != null) chromaticAberration.intensity.value = Mathf.Lerp(0, maxAberracaoCromatica, intensidadeGeral);
        if (filmGrain != null) filmGrain.intensity.value = Mathf.Lerp(0, maxFilmGrain, intensidadeGeral);
        if (efeitoTela != null) efeitoTela.color = Color.Lerp(corAltaSanidade, corBaixaSanidade, intensidadeGeral);

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

    public void RestaurarSanidadeCompleta()
    {
        sanidadeAtual = sanidadeMaxima;
        aviso50Mostrado = false;
        if (avisoSanidadeText != null)
        {
            StopAllCoroutines();
            avisoSanidadeText.gameObject.SetActive(false);
        }
        ResetarEfeitosParaSanidadeMaxima();
        DesativarEfeitoLoucura();
        if (painelVisaoTurva != null)
        {
            opacidadeAlvoPainel = 0;
            Color painelColor = painelVisaoTurva.color;
            painelColor.a = 0;
            painelVisaoTurva.color = painelColor;
        }
    }

    public void RecuperarSanidade()
    {
        if (!tutorialDaPilulaJaFoiMostrado)
        {
            //TutorialManager.instance.ExibirSequenciaTutorial(sequenciaTutorialPilula);
            tutorialDaPilulaJaFoiMostrado = true;
        }

        sanidadeAtual = Mathf.Min(sanidadeAtual + sanidadePorPilula, sanidadeMaxima);
    }

    void AtivarEfeitoLoucura()
    {
        if (audioSource != null && somCoracao != null && !audioSource.isPlaying)
        {
            audioSource.clip = somCoracao;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void DesativarEfeitoLoucura()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    public void ReduzirSanidadePorcentagem(float porcentagem)
    {
        if (porcentagem <= 0) return;
        float valorReducao = sanidadeMaxima * (porcentagem / 100f);
        sanidadeAtual -= valorReducao;
        sanidadeAtual = Mathf.Clamp(sanidadeAtual, 0, sanidadeMaxima);
        Debug.Log($"Sanidade reduzida em {valorReducao} pontos ({porcentagem}%). Sanidade atual: {sanidadeAtual}");
    }
}