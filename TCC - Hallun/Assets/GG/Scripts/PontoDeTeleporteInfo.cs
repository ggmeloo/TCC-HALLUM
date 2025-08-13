using UnityEngine;

// Este script é apenas um contêiner de dados.
// Sua única função é guardar a referência do ponto de destino.
public class PontoDeTeleporteInfo : MonoBehaviour
{
    [Tooltip("Arraste aqui o objeto que serve como destino final deste teleporte.")]
    public int destinationNumber;
}