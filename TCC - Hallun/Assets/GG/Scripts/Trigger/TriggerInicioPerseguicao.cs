using UnityEngine;

public class TriggerInicioPerseguicao : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && AnomalyManager.instance.voltaAtual == 4)
        {
            // Pega as referências necessárias do AnomalyManager
            DemonioController demonio = AnomalyManager.instance.demonio;
            Transform pontoDeInicio = AnomalyManager.instance.pontoInicioPerseguicao;
            GameObject gatilhoDeFim = AnomalyManager.instance.triggerFimPerseguicao; // Pega a referência do gatilho de fim

            // Verificação de segurança completa
            if (demonio != null && pontoDeInicio != null && gatilhoDeFim != null)
            {
                Debug.Log("PERSEGUIÇÃO INICIADA!");

                // --- NOVA RESPONSABILIDADE ---
                // 1. Ativa o gatilho de fim, preparando a "rota de fuga".
                gatilhoDeFim.SetActive(true);
                Debug.Log("Gatilho de Fim de Perseguição foi ATIVADO.");

                // 2. Posiciona o demônio no ponto de início escondido.
                demonio.transform.position = pontoDeInicio.position;
                demonio.transform.rotation = pontoDeInicio.rotation;

                // 3. Comanda o demônio para iniciar a perseguição.
                demonio.IniciarPerseguicao(other.transform);

                // 4. Desativa este gatilho de início para não disparar de novo.
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("ERRO: Uma ou mais referências (Demonio, PontoInicioPerseguicao, TriggerFimPerseguicao) não estão configuradas no AnomalyManager!");
            }
        }
    }
}