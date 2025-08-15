using UnityEngine;

public class IdentificadorDeAnomalias : MonoBehaviour
{
    [Tooltip("A dist�ncia m�xima que a 'foto' da c�mera pode alcan�ar para identificar algo.")]
    public float distanciaMaxima = 10f;

    void Update()
    {
        // --- CONDI��O ALTERADA AQUI ---
        // Agora, o c�digo s� executa se o bot�o direito ESTIVER SENDO SEGURADO (GetMouseButton)
        // E o bot�o esquerdo for PRESSIONADO (GetMouseButtonDown).
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Tentativa de identificar anomalia (Segurando Direito + Clicando Esquerdo)");

            // O resto da l�gica � exatamente o mesmo.
            Ray raio = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(raio, out hitInfo, distanciaMaxima))
            {
                Anomalia anomaliaDetectada = hitInfo.collider.GetComponent<Anomalia>();

                if (anomaliaDetectada != null)
                {
                    anomaliaDetectada.Identificar();
                }
            }
        }
    }
}