using UnityEngine;

public class ControleLanterna : MonoBehaviour
{
    [Tooltip("O objeto que contém a luz da lanterna.")]
    public GameObject fonteDeLuz;

    [Tooltip("O som de clique ao ligar/desligar.")]
    public AudioSource somDeClique;

    [Tooltip("Tecla usada para acionar a lanterna.")]
    public KeyCode teclaLanterna = KeyCode.F;

    private bool estaLigada = false;

    void Start()
    {
        // Garante que a lanterna comece desligada
        fonteDeLuz.SetActive(false);
    }

    void Update()
    {
        // Verifica se a tecla da lanterna foi pressionada
        if (Input.GetKeyDown(teclaLanterna))
        {
            // Inverte o estado da lanterna (liga se estava desligada, desliga se estava ligada)
            estaLigada = !estaLigada;
            fonteDeLuz.SetActive(estaLigada);

            // Toca o som de clique, se houver um configurado
            if (somDeClique != null)
            {
                somDeClique.Play();
            }
        }
    }
}