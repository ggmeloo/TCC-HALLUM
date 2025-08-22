using System.Collections;
using UnityEngine;

public class LuzPadraoEspecifico : MonoBehaviour
{
    // O componente de Luz que será controlado
    public Light minhaLuz;

    [Header("Configurações do Padrão")]
    // 1. NÚMERO DE PISCADAS
    public int numeroDePiscadas = 4;

    // 2. TEMPO ACESA
    public float tempoAcesaEstavel = 2.5f;

    // 3. VELOCIDADE E INTERVALO DA PISCADA
    // O tempo total de uma piscada (apagada + acesa)
    public float intervaloTotalPorPiscada = 0.5f;

    // A porcentagem desse intervalo que a luz fica APAGADA
    [Range(1, 99)]
    public float porcentagemApagada = 40f;

    void Start()
    {
        // Pega o componente de Luz automaticamente se não for atribuído
        if (minhaLuz == null)
        {
            minhaLuz = GetComponent<Light>();
        }

        // Inicia a rotina que vai se repetir para sempre
        StartCoroutine(LoopDeLuzControlado());
    }

    IEnumerator LoopDeLuzControlado()
    {
        // Loop infinito para que o padrão se repita continuamente
        while (true)
        {
            // --- PARTE 1: A SEQUÊNCIA DE PISCADAS ---

            // Calcula a duração exata de "apagar" e "acender" com base na porcentagem
            float tempoApagada = intervaloTotalPorPiscada * (porcentagemApagada / 100f);
            float tempoAcesaNaPiscada = intervaloTotalPorPiscada - tempoApagada;

            // Executa o número de piscadas definido
            for (int i = 0; i < numeroDePiscadas; i++)
            {
                // Apaga a luz
                minhaLuz.enabled = false;
                yield return new WaitForSeconds(tempoApagada);

                // Acende a luz
                minhaLuz.enabled = true;
                yield return new WaitForSeconds(tempoAcesaNaPiscada);
            }

            // --- PARTE 2: A LUZ FICA ACESA ESTÁVEL ---

            // Garante que a luz permaneça acesa
            minhaLuz.enabled = true;
            yield return new WaitForSeconds(tempoAcesaEstavel);

            // O loop recomeçará, iniciando a sequência de piscadas novamente
        }
    }
}