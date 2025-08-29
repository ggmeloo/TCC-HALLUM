// AnomaliaBase.cs

using UnityEngine;

// Garante que todo objeto com uma anomalia possa ser detectado pelo Raycast.
[RequireComponent(typeof(Collider))]
public abstract class AnomaliaBase : MonoBehaviour
{
    [Tooltip("Em qual volta esta anomalia deve se manifestar?")]
    public int voltaDeAtivacao = 1;

    protected bool jaFoiIdentificada = false;
    protected bool anomaliaAtiva = false; // Controla se o comportamento anômalo está ocorrendo

    /// <summary>
    /// A lógica principal para ativar o comportamento específico da anomalia (flutuar, rotacionar, etc.).
    /// Cada script filho DEVE implementar este método.
    /// </summary>
    public abstract void AtivarAnomalia();

    /// <summary>
    /// A lógica para reverter o objeto ao seu estado completamente normal.
    /// Cada script filho DEVE implementar este método.
    /// </summary>
    public abstract void DesativarAnomalia();

    /// <summary>
    /// Lógica universal para quando o jogador identifica a anomalia.
    /// </summary>
    /// <returns>Verdadeiro se foi a primeira vez, falso caso contrário.</returns>
    public bool Identificar()
    {
        if (!jaFoiIdentificada)
        {
            jaFoiIdentificada = true;
            Debug.Log("Anomalia '" + gameObject.name + "' foi identificada com sucesso.");

            if (AnomalyManager.instance != null)
            {
                AnomalyManager.instance.RegistrarAnomaliaEncontrada();
            }

            // Opcional: Você pode querer que a anomalia volte ao normal assim que for identificada.
            // DesativarAnomalia();

            return true;
        }
        else
        {
            Debug.Log("Anomalia '" + gameObject.name + "' já havia sido identificada.");
            return false;
        }
    }
}