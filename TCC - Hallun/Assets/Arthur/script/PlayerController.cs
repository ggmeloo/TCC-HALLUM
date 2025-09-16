using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    // A posição para onde o jogador será teletransportado
    public Transform posicaoMorte;

    // O nome da tag do bloco que causa a morte
    public string tagBlocoMorte = "Morte";

    private Rigidbody rb;
    private bool estaMorto = false;

    // A função Start é chamada quando o script é ativado
    void Start()
    {
        // Encontra o componente Rigidbody no Player
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("O Rigidbody não foi encontrado no Player. Adicione um para o script funcionar corretamente.");
        }
    }

    // Esta função é chamada quando o jogador entra em um Collider com 'Is Trigger' ativado
    void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto com o qual colidimos tem a tag "Morte"
        if (other.CompareTag(tagBlocoMorte))
        {
            // Chama a função para lidar com a "morte" e o teletransporte
            TeleportarEImobilizar();
        }
    }

    // Função para teletransportar o jogador e impedir que ele se mova
    void TeleportarEImobilizar()
    {
        // Teleporta o jogador para a posição do objeto "posicaoMorte"
        transform.position = posicaoMorte.position;

        // Trava o Rigidbody para que o jogador não consiga se mover
        // isKinematic desativa a física do objeto, ignorando colisões e forças
        rb.isKinematic = true;

        // Para garantir que o jogador não seja afetado pela gravidade
        rb.useGravity = false;

        Debug.Log("Jogador colidiu com a morte e foi teletransportado e imobilizado!");
    }
}