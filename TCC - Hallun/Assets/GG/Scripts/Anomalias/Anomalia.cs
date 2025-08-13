using UnityEngine;

public class Anomalia : MonoBehaviour
{
    private bool jaFoiIdentificada = false;

    public void Identificar()
    {
        // Se esta anomalia ainda n�o foi encontrada...
        if (!jaFoiIdentificada)
        {
            jaFoiIdentificada = true;

            // CORRIGIDO: Chamando a inst�ncia est�tica simples do AnomalyManager.
            // Adicionamos um teste para garantir que o manager exista antes de cham�-lo.
            if (AnomalyManager.instance != null)
            {
                AnomalyManager.instance.RegistrarAnomaliaEncontrada();
            }
            else
            {
                // Este erro aparecer� se voc� esquecer de colocar o AnomalyManager na cena.
                Debug.LogError("ERRO CR�TICO: O AnomalyManager n�o foi encontrado na cena!");
            }

            Debug.Log("O objeto '" + gameObject.name + "' foi identificado como uma anomalia.");

            // Voc� pode adicionar um feedback aqui, como desativar o pr�prio objeto an�malo:
            // gameObject.SetActive(false);
        }
    }
}