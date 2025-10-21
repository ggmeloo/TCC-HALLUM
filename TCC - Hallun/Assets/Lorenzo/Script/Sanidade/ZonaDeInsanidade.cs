using UnityEngine;

[RequireComponent(typeof(Collider))] // Garante que o objeto sempre terá um Collider
public class ZonaDeInsanidade : MonoBehaviour
{
    [Header("Configuração da Zona")]
    [Tooltip("Qual porcentagem da sanidade MÁXIMA será perdida ao entrar nesta zona. (Ex: 25 para 25%)")]
    [Range(0, 100)]
    public float porcentagemReducao = 25f;

    [Tooltip("Se marcado, esta zona só funcionará uma vez.")]
    public bool usoUnico = true;

    [Tooltip("Se for de uso único, o objeto será destruído após ativar?")]
    public bool destruirAposUso = false;

    private bool jaAtivou = false;

    private void Start()
    {
        // Garante que o collider está configurado como Trigger para que o OnTriggerEnter funcione
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se for de uso único e já tiver ativado, não faz mais nada.
        if (usoUnico && jaAtivou)
        {
            return;
        }

        // Verifica se o objeto que entrou na zona é o jogador (pela tag)
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o componente SanidadeController do jogador
            SanidadeController sanidadeController = other.GetComponent<SanidadeController>();

            if (sanidadeController != null)
            {
                Debug.Log($"Jogador entrou na zona de insanidade. Reduzindo sanidade em {porcentagemReducao}%.");

                // Chama a nova função no SanidadeController, passando a porcentagem
                sanidadeController.ReduzirSanidadePorcentagem(porcentagemReducao);

                // Marca que já foi ativado
                jaAtivou = true;

                // Se for configurado para destruir, destrói o próprio objeto
                if (usoUnico && destruirAposUso)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}