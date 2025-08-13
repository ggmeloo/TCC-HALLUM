using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    // A forma mais simples de Singleton: uma variável estática pública.
    // Qualquer script pode acessá-la com "AnomalyManager.instance".
    public static AnomalyManager instance;

    private int totalDeAnomaliasNaCena;
    private int anomaliasEncontradas = 0;

    // Awake é chamado antes de qualquer método Start.
    void Awake()
    {
        // Se nenhuma instância do gerenciador existe ainda...
        if (instance == null)
        {
            // ...esta se torna a instância.
            instance = this;
        }
        else
        {
            // Se uma instância já existe (é um duplicado), se autodestrói.
            // Isso garante que SEMPRE haverá apenas um AnomalyManager.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // CORRIGIDO: Usando o método moderno e removendo o erro de obsoleto.
        totalDeAnomaliasNaCena = FindObjectsByType<Anomalia>(FindObjectsSortMode.None).Length;

        if (totalDeAnomaliasNaCena == 0)
        {
            Debug.LogWarning("AVISO: Nenhuma anomalia foi encontrada na cena. Verifique se os objetos corretos têm o script 'Anomalia'.");
        }
        else
        {
            Debug.Log("CENA INICIADA. Total de anomalias para encontrar: " + totalDeAnomaliasNaCena);
        }
    }

    // Método público para que outros scripts possam registrar uma anomalia encontrada.
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
        // Coloque aqui sua lógica para avançar (abrir uma porta, carregar o próximo nível, etc.)
    }
}