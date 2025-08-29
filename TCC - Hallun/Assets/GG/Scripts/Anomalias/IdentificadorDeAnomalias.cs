using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class IdentificadorDeAnomalias : MonoBehaviour
{
    [Header("Referências Visuais e Sonoras")]
    public Image efeitoCameraUI;
    public TextMeshProUGUI mensagemFeedbackText;

    [Header("Efeitos Sonoros")]
    public AudioClip somChiado, somZoomIn, somZoomOut, somFoto, somDigitacao;

    [Header("Configurações de Efeitos")]
    public float fovNormal = 60f;
    public float fovZoom = 40f;
    public float velocidadeZoom = 10f;
    public float velocidadeFade = 8f;
    public float velocidadeDigitacao = 0.05f;
    public float duracaoMensagem = 2.5f;

    [Header("Identificação de Anomalias")]
    public float distanciaMaxima = 10f;

    private Camera cameraPrincipal;
    private AudioSource audioSource;
    private Coroutine mensagemCoroutine;

    void Awake()
    {
        cameraPrincipal = GetComponent<Camera>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }

        if (efeitoCameraUI != null)
        {
            efeitoCameraUI.gameObject.SetActive(true);
            efeitoCameraUI.color = new Color(efeitoCameraUI.color.r, efeitoCameraUI.color.g, efeitoCameraUI.color.b, 0f);
        }

        if (mensagemFeedbackText != null)
        {
            mensagemFeedbackText.text = "";
        }

        if (cameraPrincipal != null) cameraPrincipal.fieldOfView = fovNormal;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) TocarSom(somZoomIn);
        if (Input.GetMouseButtonUp(1)) TocarSom(somZoomOut);

        bool estaMirando = Input.GetMouseButton(1);

        if (estaMirando)
        {
            cameraPrincipal.fieldOfView = Mathf.Lerp(cameraPrincipal.fieldOfView, fovZoom, Time.deltaTime * velocidadeZoom);
            AtualizarFadeUI(1f);
            ControlarSomChiado(true);
            if (Input.GetMouseButtonDown(0))
            {
                IdentificarAnomalia();
            }
        }
        else
        {
            cameraPrincipal.fieldOfView = Mathf.Lerp(cameraPrincipal.fieldOfView, fovNormal, Time.deltaTime * velocidadeZoom);
            AtualizarFadeUI(0f);
            ControlarSomChiado(false);
            if (mensagemCoroutine != null)
            {
                StopCoroutine(mensagemCoroutine);
                mensagemCoroutine = null;
                if (mensagemFeedbackText != null)
                {
                    mensagemFeedbackText.text = "";
                }
            }
        }
    }

    void IdentificarAnomalia()
    {
        TocarSom(somFoto);
        Ray raio = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(raio, out hitInfo, distanciaMaxima))
        {
            Anomalia anomaliaDetectada = hitInfo.collider.GetComponent<Anomalia>();
            if (anomaliaDetectada != null)
            {
                bool sucessoNaIdentificacao = anomaliaDetectada.Identificar();
                if (sucessoNaIdentificacao)
                {
                    MostrarMensagem("ANOMALIA IDENTIFICADA", Color.green);
                }
                else
                {
                    MostrarMensagem("ANOMALIA JÁ IDENTIFICADA", Color.yellow);
                }
            }
            else
            {
                MostrarMensagem("NENHUMA ANOMALIA IDENTIFICADA", Color.red);
            }
        }
        else
        {
            MostrarMensagem("NENHUMA ANOMALIA IDENTIFICADA", Color.red);
        }
    }

    void MostrarMensagem(string texto, Color cor)
    {
        if (mensagemCoroutine != null) StopCoroutine(mensagemCoroutine);
        mensagemCoroutine = StartCoroutine(MensagemTypewriterCoroutine(texto, cor));
    }

    private IEnumerator MensagemTypewriterCoroutine(string texto, Color cor)
    {
        if (mensagemFeedbackText != null)
        {
            mensagemFeedbackText.color = cor;
            mensagemFeedbackText.text = "";

            foreach (char letra in texto)
            {
                mensagemFeedbackText.text += letra;
                TocarSom(somDigitacao);
                yield return new WaitForSeconds(velocidadeDigitacao);
            }

            yield return new WaitForSeconds(duracaoMensagem);
            mensagemFeedbackText.text = "";
        }
    }

    void AtualizarFadeUI(float alphaAlvo)
    {
        if (efeitoCameraUI != null)
        {
            Color corAtual = efeitoCameraUI.color;
            float novoAlpha = Mathf.Lerp(corAtual.a, alphaAlvo, Time.deltaTime * velocidadeFade);
            efeitoCameraUI.color = new Color(corAtual.r, corAtual.g, corAtual.b, novoAlpha);
        }
    }

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
            audioSource.PlayOneShot(clip);
        }
    }
}