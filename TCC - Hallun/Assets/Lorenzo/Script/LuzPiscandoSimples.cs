using System.Collections;
using UnityEngine;

public class LuzPadraoEspecifico : MonoBehaviour
{
    // O componente de Luz que será controlado
    public Light minhaLuz;

    [Header("Configurações do Padrão")]
    public int numeroDePiscadas = 4;
    public float tempoAcesaEstavel = 2.5f;
    public float intervaloTotalPorPiscada = 0.5f;
    [Range(1, 99)]
    public float porcentagemApagada = 40f;

    // A Unity chama OnEnable sempre que o componente é ativado no Inspector
    // ou via script (com .enabled = true).
    void OnEnable()
    {
        // Pega o componente de Luz automaticamente se não for atribuído
        if (minhaLuz == null)
        {
            minhaLuz = GetComponent<Light>();
        }

        // Inicia a rotina que vai se repetir
        StartCoroutine(LoopDeLuzControlado());
    }

    IEnumerator LoopDeLuzControlado()
    {
        // Loop infinito para que o padrão se repita continuamente
        while (true)
        {
            // --- PARTE 1: A SEQUÊNCIA DE PISCADAS ---
            float tempoApagada = intervaloTotalPorPiscada * (porcentagemApagada / 100f);
            float tempoAcesaNaPiscada = intervaloTotalPorPiscada - tempoApagada;

            for (int i = 0; i < numeroDePiscadas; i++)
            {
                minhaLuz.enabled = false;
                yield return new WaitForSeconds(tempoApagada);

                minhaLuz.enabled = true;
                yield return new WaitForSeconds(tempoAcesaNaPiscada);
            }

            // --- PARTE 2: A LUZ FICA ACESA ESTÁVEL ---
            minhaLuz.enabled = true;
            yield return new WaitForSeconds(tempoAcesaEstavel);
        }
    }
}