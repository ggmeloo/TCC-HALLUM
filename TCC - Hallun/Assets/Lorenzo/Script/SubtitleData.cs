using UnityEngine;

// Esta linha mágica cria a opção no menu: "Create > Jogo de Terror > Legenda"
[CreateAssetMenu(fileName = "Nova Legenda", menuName = "Jogo de Terror/Legenda")]
public class SubtitleData : ScriptableObject
{
    [Header("Conteúdo da Legenda")]
    [Tooltip("A frase que será exibida na tela.")]
    [TextArea(3, 5)] // Cria uma caixa de texto maior no Inspector
    public string subtitleMessage;

    [Tooltip("Por quantos segundos a legenda deve aparecer.")]
    public float displayDuration = 5f;
}