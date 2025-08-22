using UnityEngine;

public class ControladorAnimacaoBracos : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o componente Animator que está no seu modelo de braços para cá.")]
    public Animator animatorBracos;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (animatorBracos == null) return;

        // **CORREÇÃO APLICADA AQUI**
        // Usa linearVelocity para pegar a velocidade de movimento.
        Vector3 velocidadeHorizontal = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        float velocidade = velocidadeHorizontal.magnitude;

        int estado = 0;
        if (velocidade > 0.1f)
        {
            estado = 1;
        }

        animatorBracos.SetInteger("estadoMovimento", estado);
    }
}