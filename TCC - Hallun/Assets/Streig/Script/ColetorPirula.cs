using UnityEngine;

public class ColetorPilulas : MonoBehaviour
{
    [Header("Configurações")]
    public AudioClip somColetar;
    public ParticleSystem efeitoColeta;
    public float distanciaColeta = 2f; // Distância máxima para pegar a pílula

    private AudioSource audioSource;
    private GameObject pilulaProxima; // Armazena a pílula mais próxima
    private SanidadeController sanidadeController; // Referência para o controlador de sanidade

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Busca o controlador de sanidade no mesmo objeto
        sanidadeController = GetComponent<SanidadeController>();
    }

    void Update()
    {
        // Verifica se há pílulas próximas e se o jogador pressionou "E"
        if (Input.GetKeyDown(KeyCode.E) && pilulaProxima != null)
        {
            Coletar(pilulaProxima);
        }
    }

    void FixedUpdate()
    {
        // Detecta pílulas próximas usando um Raycast ou OverlapSphere
        VerificarPilulasProximas();
    }

    void VerificarPilulasProximas()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distanciaColeta);
        float menorDistancia = Mathf.Infinity;
        pilulaProxima = null;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Pilula"))
            {
                float distancia = Vector3.Distance(transform.position, collider.transform.position);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    pilulaProxima = collider.gameObject;
                }
            }
        }
    }

    void Coletar(GameObject pilula)
    {
        // Efeitos sonoros e visuais
        if (audioSource != null && somColetar != null)
            audioSource.PlayOneShot(somColetar);

        if (efeitoColeta != null)
            Instantiate(efeitoColeta, pilula.transform.position, Quaternion.identity);

        // Destrói a pílula
        Destroy(pilula);
        pilulaProxima = null; // Reseta a referência

        // Recupera sanidade
        if (sanidadeController != null)
        {
            sanidadeController.RecuperarSanidade();
        }
        else
        {
            Debug.LogWarning("SanidadeController não encontrado no personagem!");
        }
    }

    // Debug: mostra o raio de coleta no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaColeta);
    }
}