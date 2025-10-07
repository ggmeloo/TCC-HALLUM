using UnityEngine;
using System.Collections.Generic; // Adicione esta linha para usar Listas

public class AtivadorDeLuz : MonoBehaviour
{
    // Agora temos uma LISTA de scripts de luz para ativar.
    // No Inspector, você poderá definir o tamanho dessa lista e arrastar
    // quantas luzes quiser para ela.
    public List<LuzPadraoEspecifico> luzesParaAtivar;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no gatilho tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Verifica se a lista não é nula e tem pelo menos um item
            if (luzesParaAtivar != null && luzesParaAtivar.Count > 0)
            {
                // Passa por cada script de luz na lista
                foreach (LuzPadraoEspecifico luzScript in luzesParaAtivar)
                {
                    // Ativa o componente do script da luz
                    if (luzScript != null)
                    {
                        luzScript.enabled = true;
                    }
                }

                // Destrói este objeto de gatilho depois de usar para não ser ativado novamente.
                Destroy(gameObject);
            }
        }
    }
}