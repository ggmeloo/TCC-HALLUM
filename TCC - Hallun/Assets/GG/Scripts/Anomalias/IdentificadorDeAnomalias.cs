using UnityEngine;

public class IdentificadorDeAnomalias : MonoBehaviour
{
    [Tooltip("A distância máxima que a 'foto' da câmera pode alcançar para identificar algo.")]
    public float distanciaMaxima = 10f;

    void Update()
    {
        // --- CONDIÇÃO ALTERADA AQUI ---
        // Agora, o código só executa se o botão direito ESTIVER SENDO SEGURADO (GetMouseButton)
        // E o botão esquerdo for PRESSIONADO (GetMouseButtonDown).
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Tentativa de identificar anomalia (Segurando Direito + Clicando Esquerdo)");

            // O resto da lógica é exatamente o mesmo.
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