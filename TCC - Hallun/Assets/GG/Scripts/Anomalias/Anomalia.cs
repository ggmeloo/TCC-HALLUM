using UnityEngine;

public class Anomalia : MonoBehaviour
{
    public enum TipoAnomalia { Fixa, Aleatoria }

    [Tooltip("Defina se esta anomalia é Fixa (tutorial) ou Aleatória.")]
    public TipoAnomalia tipo = TipoAnomalia.Aleatoria;

    private bool jaFoiIdentificada = false;

    public bool FoiIdentificada() { return jaFoiIdentificada; }
    public void AtivarAnomalia() { gameObject.SetActive(true); }
    public void DesativarAnomalia() { gameObject.SetActive(false); }
    public void ResetarIdentificacao() { jaFoiIdentificada = false; }

    public bool Identificar()
    {
        if (!jaFoiIdentificada)
        {
            jaFoiIdentificada = true;
            if (AnomalyManager.instance != null)
            {
                AnomalyManager.instance.RegistrarAnomaliaEncontrada();
            }
            return true;
        }
        return false;
    }
}