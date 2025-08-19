using UnityEngine;

public class TriggerFimPerseguicao : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // 1. A primeira coisa que fazemos é anunciar que ALGO tocou no gatilho.
        Debug.Log("GATILHO DE FIM ATIVADO por: " + other.name);

        // 2. Verificamos se é o jogador.
        if (other.CompareTag("Player"))
        {
            Debug.Log("O objeto é o jogador. Verificando a volta atual...");

            // 3. Verificamos se estamos na volta correta (Volta 4).
            if (AnomalyManager.instance.voltaAtual == 4)
            {
                Debug.Log("A volta está correta (Volta 4). Comandando o demônio para desaparecer.");

                // 4. Se todas as condições passaram, executamos a lógica.
                AnomalyManager.instance.demonio.PararPerseguicaoEDesaparecer();

                // Desativa o gatilho para não disparar de novo.
                gameObject.SetActive(false);
            }
            else
            {
                // Esta mensagem aparecerá se você tocar no gatilho na volta errada.
                Debug.LogWarning("O jogador tocou no gatilho, MAS NÃO ESTÁ NA VOLTA 4. A volta atual é: " + AnomalyManager.instance.voltaAtual);
            }
        }
        else
        {
            // Esta mensagem aparecerá se um objeto que NÃO é o jogador tocar no gatilho.
            Debug.LogWarning("O objeto que tocou no gatilho NÃO tem a tag 'Player'. A tag dele é: '" + other.tag + "'");
        }
    }
}