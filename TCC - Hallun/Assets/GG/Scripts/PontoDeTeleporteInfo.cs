using UnityEngine;

// Este script � apenas um cont�iner de dados.
// Sua �nica fun��o � guardar a refer�ncia do ponto de destino.
public class PontoDeTeleporteInfo : MonoBehaviour
{
    [Tooltip("Arraste aqui o objeto que serve como destino final deste teleporte.")]
    public int destinationNumber;
}