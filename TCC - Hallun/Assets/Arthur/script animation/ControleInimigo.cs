using UnityEngine;

public class ControleInimigo : MonoBehaviour
{
    // Variáveis para as animações e o som
    public Animator animador;
    public AudioClip novoSom;
    private AudioSource audioSource;

    // Nome do parâmetro que você criou no Animator para a nova animação
    public string nomeDoParametroDeAnimacao = "EstaTocando";

    void Start()
    {
        // Pega o componente Animator do objeto
        animador = GetComponent<Animator>();

        // Pega o componente AudioSource do objeto
        audioSource = GetComponent<AudioSource>();
    }

    // Esta função é chamada quando o objeto entra em colisão com um trigger
    void OnTriggerEnter2D(Collider2D outroObjeto)
    {
        // Verifica se o objeto que colidiu tem a tag "BlocoDeSom"
        if (outroObjeto.CompareTag("BlocoDeSom"))
        {
            // Mudar a animação
            // Define o parâmetro booleano para "true"
            animador.SetBool(nomeDoParametroDeAnimacao, true);

            // Tocar o novo som
            // Define o novo som no AudioSource e toca
            if (audioSource != null && novoSom != null)
            {
                audioSource.clip = novoSom;
                audioSource.Play();
            }
        }
    }
}