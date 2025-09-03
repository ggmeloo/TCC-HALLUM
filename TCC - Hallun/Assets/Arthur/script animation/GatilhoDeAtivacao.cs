using UnityEngine;

/// <summary>
/// Um gatilho genérico que, ao ser tocado pelo jogador, ativa um inimigo específico.
/// Deve ser colocado em um objeto com um Collider 3D marcado como "Is Trigger".
/// </summary>
public class GatilhoDeAtivacao : MonoBehaviour
{
    [Header("Configurações do Gatilho")]
    [Tooltip("Arraste para cá o Inimigo (que contém o script InimigoPerseguidor3D) que deve ser ativado por este gatilho.")]
    public InimigoPerseguidor3D inimigoParaAtivar;

    [Tooltip("Marque se este gatilho deve funcionar apenas uma vez. Após o uso, ele será desativado.")]
    public bool usarApenasUmaVez = true;

    [Tooltip("A tag do objeto que pode acionar este gatilho (geralmente 'Player').")]
    public string tagDoJogador = "Player";

    /// <summary>
    /// Função da Unity chamada automaticamente quando um Collider entra neste Trigger.
    /// </summary>
    /// <param name="other">O Collider do objeto que entrou no gatilho.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou tem a tag correta e se a referência do inimigo é válida
        if (other.CompareTag(tagDoJogador) && inimigoParaAtivar != null)
        {
            // Mensagem de depuração para confirmar que o gatilho foi acionado
            Debug.Log("Gatilho acionado por: " + other.name + ". Ativando o inimigo: " + inimigoParaAtivar.name);

            // Chama a função pública no script do inimigo para iniciar a perseguição
            inimigoParaAtivar.AtivarPerseguicao();

            // Se a opção estiver marcada, desativa o objeto do gatilho para que ele não possa ser usado novamente
            if (usarApenasUmaVez)
            {
                gameObject.SetActive(false);
            }
        }
    }
}