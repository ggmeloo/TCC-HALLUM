using UnityEngine;

public class Anomalia : MonoBehaviour
{
    private bool jaFoiIdentificada = false;

    public void Identificar()
    {
        // Se esta anomalia ainda não foi encontrada...
        if (!jaFoiIdentificada)
        {
            jaFoiIdentificada = true;

            // CORRIGIDO: Chamando a instância estática simples do AnomalyManager.
            // Adicionamos um teste para garantir que o manager exista antes de chamá-lo.
            if (AnomalyManager.instance != null)
            {
                AnomalyManager.instance.RegistrarAnomaliaEncontrada();
            }
            else
            {
                // Este erro aparecerá se você esquecer de colocar o AnomalyManager na cena.
                Debug.LogError("ERRO CRÍTICO: O AnomalyManager não foi encontrado na cena!");
            }

            Debug.Log("O objeto '" + gameObject.name + "' foi identificado como uma anomalia.");

            // Você pode adicionar um feedback aqui, como desativar o próprio objeto anômalo:
            // gameObject.SetActive(false);
        }
    }
}