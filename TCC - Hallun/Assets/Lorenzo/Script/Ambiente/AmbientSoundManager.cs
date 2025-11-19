using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [Header("Referências Principais")]
    public Transform jogadorTransform;
    private SanidadeController sanidadeController;

    [Header("Sons de Respiração (Crossfade)")]
    public AudioSource audioSourceRespiracaoCalma;
    public AudioSource audioSourceRespiracaoOfegante;
    public AudioClip respiracaoCalma;
    public AudioClip respiracaoOfegante;
    public float tempoDeTransicaoRespiracao = 1.5f;
    private enum EstadoRespiracao { Calma, Ofegante }
    private EstadoRespiracao estadoAtualRespiracao;
    private Coroutine rotinaDeTransicaoRespiracao;

    [Header("Sons de Sanidade Crítica")]
    public AudioClip somDeCoracao;
    private AudioSource audioSourceCoracao;
    private bool estaTocandoCoracao = false;

    [Header("Sons de Ambiente (Aleatórios)")]
    public AudioClip[] risadas;
    public AudioClip[] sonsDeRato;
    public AudioClip[] batidasNaPorta;
    public AudioClip[] sonsDeVento;
    public AudioClip[] sussurrosEGritos;

    [Header("Controle de Medo (A partir da Volta 2)")]
    public AnimationCurve curvaDeIntensidadeDoMedo = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector2 tempoEntreSons_SanidadeAlta = new Vector2(15f, 25f);
    public Vector2 tempoEntreSons_SanidadeBaixa = new Vector2(5f, 10f);
    public Vector2 tempoEntreSussurros_SanidadeAlta = new Vector2(25f, 40f);
    public Vector2 tempoEntreSussurros_SanidadeBaixa = new Vector2(0.5f, 1.5f);
    [Range(0, 1)] public float volumeSussurros_SanidadeAlta = 0.3f;
    [Range(0, 1)] public float volumeSussurros_SanidadeBaixa = 1.0f;
    public Vector2 distanciaDoSom = new Vector2(5f, 20f);

    [Header("Configurações de Pooling de Áudio")]
    [Tooltip("Pool para sons 3D (risadas, batidas, etc.).")]
    public int tamanhoDoPool_3D = 5;
    [Tooltip("Pool para sussurros 2D.")]
    public int tamanhoDoPool_Sussurros = 8;
    private List<AudioSource> poolDeAudioSources3D;
    private List<AudioSource> poolDeAudioSources2DSussurros;

    private bool sistemaDeMedoAtivo = false;
    private Coroutine rotinaSonsBasicos;

    void Start()
    {
        sanidadeController = jogadorTransform.GetComponent<SanidadeController>();
        if (sanidadeController == null) { Debug.LogError("ERRO: 'SanidadeController' não encontrado!", this); this.enabled = false; return; }

        InicializarPoolsDeAudio();

        Debug.Log("AmbientSoundManager: Iniciando em modo Volta 1 (sons básicos).");
        rotinaSonsBasicos = StartCoroutine(RotinaDeSonsBasicos());
    }

    public void AtivarSistemaDeMedoCompleto()
    {
        if (sistemaDeMedoAtivo) return;

        Debug.Log("AmbientSoundManager: Sistema de Medo Completo ATIVADO para a Volta 2.");
        sistemaDeMedoAtivo = true;

        if (rotinaSonsBasicos != null)
        {
            StopCoroutine(rotinaSonsBasicos);
        }

        InicializarSonsFixos();
        StartCoroutine(RotinaDeSonsDeAmbiente());
        StartCoroutine(RotinaDeSussurrosDeSanidade());
    }

    void Update()
    {
        if (sistemaDeMedoAtivo)
        {
            ControlarEfeitosDeReacao();
        }
    }

    private IEnumerator RotinaDeSonsBasicos()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(18f, 30f));
            List<AudioClip> baralhoDeSons = new List<AudioClip>();
            baralhoDeSons.AddRange(sonsDeRato);
            baralhoDeSons.AddRange(sonsDeVento);
            TocarSomDeAmbienteDoPool3D(baralhoDeSons.ToArray());
        }
    }

    private void ControlarEfeitosDeReacao()
    {
        if (sanidadeController == null) return;
        float sanidadeNormalizada = sanidadeController.sanidadeAtual / sanidadeController.sanidadeMaxima;

        if (sanidadeNormalizada < 0.4f && estadoAtualRespiracao == EstadoRespiracao.Calma) IniciarTransicaoRespiracao(EstadoRespiracao.Ofegante);
        else if (sanidadeNormalizada > 0.45f && estadoAtualRespiracao == EstadoRespiracao.Ofegante) IniciarTransicaoRespiracao(EstadoRespiracao.Calma);

        if (sanidadeNormalizada < 0.3f && !estaTocandoCoracao) { estaTocandoCoracao = true; audioSourceCoracao.Play(); }
        else if (sanidadeNormalizada >= 0.3f && estaTocandoCoracao) { estaTocandoCoracao = false; audioSourceCoracao.Stop(); }
    }

    private IEnumerator RotinaDeSussurrosDeSanidade()
    {
        while (true)
        {
            float sanidadeInvertida = 1 - (sanidadeController.sanidadeAtual / sanidadeController.sanidadeMaxima);
            float intensidade = curvaDeIntensidadeDoMedo.Evaluate(sanidadeInvertida);

            float espera = Mathf.Lerp(tempoEntreSussurros_SanidadeAlta.x, tempoEntreSussurros_SanidadeBaixa.x, intensidade);
            yield return new WaitForSeconds(Random.Range(espera, espera * 1.5f));

            float volume = Mathf.Lerp(volumeSussurros_SanidadeAlta, volumeSussurros_SanidadeBaixa, intensidade);

            TocarSussurroDoPool2D(sussurrosEGritos, volume);
        }
    }

    private IEnumerator RotinaDeSonsDeAmbiente()
    {
        while (true)
        {
            float sanidadeInvertida = 1 - (sanidadeController.sanidadeAtual / sanidadeController.sanidadeMaxima);
            float intensidade = curvaDeIntensidadeDoMedo.Evaluate(sanidadeInvertida);

            float espera = Mathf.Lerp(tempoEntreSons_SanidadeAlta.x, tempoEntreSons_SanidadeBaixa.x, intensidade);
            yield return new WaitForSeconds(Random.Range(espera, espera * 1.5f));

            List<AudioClip> baralhoDeSons = new List<AudioClip>();
            baralhoDeSons.AddRange(sonsDeRato);
            baralhoDeSons.AddRange(sonsDeVento);

            if (intensidade > 0.3f) { baralhoDeSons.AddRange(batidasNaPorta); baralhoDeSons.AddRange(risadas); }
            if (intensidade > 0.6f) { baralhoDeSons.AddRange(batidasNaPorta); baralhoDeSons.AddRange(risadas); }

            TocarSomDeAmbienteDoPool3D(baralhoDeSons.ToArray());
        }
    }

    private void TocarSomDeAmbienteDoPool3D(AudioClip[] clipes, float volume = 1.0f)
    {
        if (clipes.Length == 0) return;
        AudioSource sourceDisponivel = poolDeAudioSources3D.Find(s => !s.isPlaying);
        if (sourceDisponivel != null)
        {
            sourceDisponivel.clip = clipes[Random.Range(0, clipes.Length)];
            sourceDisponivel.volume = volume;
            sourceDisponivel.transform.position = jogadorTransform.position + (Random.insideUnitSphere * Random.Range(distanciaDoSom.x, distanciaDoSom.y));
            sourceDisponivel.Play();
        }
    }

    private void TocarSussurroDoPool2D(AudioClip[] clipes, float volume = 1.0f)
    {
        if (clipes.Length == 0) return;
        AudioSource sourceDisponivel = poolDeAudioSources2DSussurros.Find(s => !s.isPlaying);
        if (sourceDisponivel != null)
        {
            sourceDisponivel.clip = clipes[Random.Range(0, clipes.Length)];
            sourceDisponivel.volume = volume;
            sourceDisponivel.Play();
        }
    }

    #region Inicialização e Respiração

    void InicializarPoolsDeAudio()
    {
        poolDeAudioSources3D = new List<AudioSource>();
        for (int i = 0; i < tamanhoDoPool_3D; i++)
        {
            GameObject objSom = new GameObject($"AudioSourcePool_3D_{i}");
            objSom.transform.SetParent(this.transform);
            AudioSource source = objSom.AddComponent<AudioSource>();
            source.spatialBlend = 1.0f;
            source.playOnAwake = false;
            poolDeAudioSources3D.Add(source);
        }

        poolDeAudioSources2DSussurros = new List<AudioSource>();
        for (int i = 0; i < tamanhoDoPool_Sussurros; i++)
        {
            GameObject objSom = new GameObject($"AudioSourcePool_2D_Sussurro_{i}");
            objSom.transform.SetParent(this.transform);
            AudioSource source = objSom.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.playOnAwake = false;
            poolDeAudioSources2DSussurros.Add(source);
        }
    }

    void InicializarSonsFixos()
    {
        if (audioSourceRespiracaoCalma != null && respiracaoCalma != null)
        {
            audioSourceRespiracaoCalma.clip = respiracaoCalma;
            audioSourceRespiracaoCalma.loop = true;
            audioSourceRespiracaoCalma.volume = 1f;
            audioSourceRespiracaoCalma.Play();
        }
        if (audioSourceRespiracaoOfegante != null && respiracaoOfegante != null)
        {
            audioSourceRespiracaoOfegante.clip = respiracaoOfegante;
            audioSourceRespiracaoOfegante.loop = true;
            audioSourceRespiracaoOfegante.volume = 0f;
            audioSourceRespiracaoOfegante.Play();
        }
        estadoAtualRespiracao = EstadoRespiracao.Calma;

        if (somDeCoracao != null)
        {
            audioSourceCoracao = gameObject.AddComponent<AudioSource>();
            audioSourceCoracao.clip = somDeCoracao;
            audioSourceCoracao.loop = true;
            audioSourceCoracao.playOnAwake = false;
            audioSourceCoracao.spatialBlend = 0;
        }
    }

    private void IniciarTransicaoRespiracao(EstadoRespiracao novoEstado)
    {
        if (rotinaDeTransicaoRespiracao != null) StopCoroutine(rotinaDeTransicaoRespiracao);
        estadoAtualRespiracao = novoEstado;
        if (novoEstado == EstadoRespiracao.Ofegante) rotinaDeTransicaoRespiracao = StartCoroutine(RotinaDeCrossfade(audioSourceRespiracaoCalma, audioSourceRespiracaoOfegante, tempoDeTransicaoRespiracao));
        else rotinaDeTransicaoRespiracao = StartCoroutine(RotinaDeCrossfade(audioSourceRespiracaoOfegante, audioSourceRespiracaoCalma, tempoDeTransicaoRespiracao));
    }

    private IEnumerator RotinaDeCrossfade(AudioSource fadeOutSource, AudioSource fadeInSource, float duracao)
    {
        if (fadeOutSource == null || fadeInSource == null) yield break;
        float tempoPassado = 0f;
        float volInicialFadeOut = fadeOutSource.volume;
        float volInicialFadeIn = fadeInSource.volume;
        if (!fadeInSource.isPlaying) fadeInSource.Play();
        while (tempoPassado < duracao)
        {
            tempoPassado += Time.deltaTime;
            float progresso = tempoPassado / duracao;
            fadeOutSource.volume = Mathf.Lerp(volInicialFadeOut, 0f, progresso);
            fadeInSource.volume = Mathf.Lerp(volInicialFadeIn, 1f, progresso);
            yield return null;
        }
        fadeOutSource.volume = 0f;
        fadeInSource.volume = 1f;
    }
    #endregion
}