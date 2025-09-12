using UnityEngine;

public class GatilhoDeVolta : MonoBehaviour
{
    private bool jaFoiAtivadoNestaVolta = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !jaFoiAtivadoNestaVolta)
        {
            jaFoiAtivadoNestaVolta = true;
            AnomalyManager.instance.ProcessarMudancaDeVolta();
        }
    }

    public void ResetarGatilho()
    {
        jaFoiAtivadoNestaVolta = false;
    }
}