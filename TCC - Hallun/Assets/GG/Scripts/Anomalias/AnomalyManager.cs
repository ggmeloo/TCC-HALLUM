using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    // A forma mais simples de Singleton: uma vari�vel est�tica p�blica.
    // Qualquer script pode acess�-la com "AnomalyManager.instance".
    public static AnomalyManager instance;

    private int totalDeAnomaliasNaCena;
    private int anomaliasEncontradas = 0;

    // Awake � chamado antes de qualquer m�todo Start.
    void Awake()
    {
        // Se nenhuma inst�ncia do gerenciador existe ainda...
        if (instance == null)
        {
            // ...esta se torna a inst�ncia.
            instance = this;
        }
        else
        {
            // Se uma inst�ncia j� existe (� um duplicado), se autodestr�i.
            // Isso garante que SEMPRE haver� apenas um AnomalyManager.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // CORRIGIDO: Usando o m�todo moderno e removendo o erro de obsoleto.
        totalDeAnomaliasNaCena = FindObjectsByType<Anomalia>(FindObjectsSortMode.None).Length;

        if (totalDeAnomaliasNaCena == 0)
        {
            Debug.LogWarning("AVISO: Nenhuma anomalia foi encontrada na cena. Verifique se os objetos corretos t�m o script 'Anomalia'.");
        }
        else
        {
            Debug.Log("CENA INICIADA. Total de anomalias para encontrar: " + totalDeAnomaliasNaCena);
        }
    }

    // M�todo p�blico para que outros scripts possam registrar uma anomalia encontrada.
    public void RegistrarAnomaliaEncontrada()
    {
        if (anomaliasEncontradas < totalDeAnomaliasNaCena)
        {
            anomaliasEncontradas++;
            Debug.Log("Anomalia encontrada! Progresso: " + anomaliasEncontradas + "/" + totalDeAnomaliasNaCena);

            if (anomaliasEncontradas >= totalDeAnomaliasNaCena)
            {
                TodasAnomaliasEncontradas();
            }
        }
    }

    private void TodasAnomaliasEncontradas()
    {
        Debug.LogWarning("!!! TODAS AS ANOMALIAS FORAM ENCONTRADAS. FIM DO LOOP !!!");
        // Coloque aqui sua l�gica para avan�ar (abrir uma porta, carregar o pr�ximo n�vel, etc.)
    }
}