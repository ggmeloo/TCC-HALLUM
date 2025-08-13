// NENHUMA MUDANÇA NECESSÁRIA AQUI
using UnityEngine;

public class IdentificadorDeAnomalias : MonoBehaviour
{
    [Tooltip("A distância máxima que a 'foto' da câmera pode alcançar para identificar algo.")]
    public float distanciaMaxima = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
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