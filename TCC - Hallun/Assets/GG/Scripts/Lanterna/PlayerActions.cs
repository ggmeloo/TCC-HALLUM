using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o seu 'Flashlight_Holder' da Hierarquia para este campo.")]
    public ControleLanternaRealista lanternaController;

    [Header("Sons da Lanterna")]
    public AudioClip somLigar;
    public AudioClip somDesligar;

    public void ColetouLanterna()
    {
        if (lanternaController != null)
        {
            // Passa os clipes de áudio para o controlador da lanterna
            lanternaController.ColetarLanterna(somLigar, somDesligar);
        }
        else
        {
            Debug.LogError("ERRO: A referência 'Lanterna Controller' não foi definida no script PlayerActions!", this);
        }
    }
}