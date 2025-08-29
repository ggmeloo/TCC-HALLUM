using UnityEngine;

public class Anomalia : MonoBehaviour
{
    private bool jaFoiIdentificada = false;

    // Este método agora retorna 'bool' (verdadeiro/falso).
    // Ele informa se a identificação foi bem-sucedida (era a primeira vez).
    public bool Identificar()
    {
        // Se esta anomalia ainda não foi encontrada...
        if (!jaFoiIdentificada)
        {
            // ...marca como encontrada para não contar duas vezes.
            jaFoiIdentificada = true;

            // Avisa ao Gerenciador que uma anomalia foi encontrada.
            if (AnomalyManager.instance != null)
            {
                AnomalyManager.instance.RegistrarAnomaliaEncontrada();
            }
            else
            {
                Debug.LogError("ERRO CRÍTICO: O AnomalyManager não foi encontrado na cena!");
            }

            Debug.Log("O objeto '" + gameObject.name + "' foi identificado como uma anomalia.");

            // Retorna VERDADEIRO para indicar que a identificação foi um sucesso.
            return true;
        }
        else
        {
            // Se já foi identificada, não faz nada e retorna FALSO.
            Debug.Log("O objeto '" + gameObject.name + "' já havia sido identificado.");
            return false;
        }
    }
}