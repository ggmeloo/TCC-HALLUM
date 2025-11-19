using UnityEngine;
using System.Collections.Generic;

public class ControlePaginasTutorial : MonoBehaviour
{
    public List<GameObject> paginas;
    private int paginaAtual = 0;

    void OnEnable()
    {
        paginaAtual = 0;
        AtualizarVisibilidadePaginas();
    }

    public void ProximaPagina()
    {
        if (paginaAtual < paginas.Count - 1)
        {
            paginaAtual++;
            AtualizarVisibilidadePaginas();
        }
    }

    public void PaginaAnterior()
    {
        if (paginaAtual > 0)
        {
            paginaAtual--;
            AtualizarVisibilidadePaginas();
        }
    }

    private void AtualizarVisibilidadePaginas()
    {
        for (int i = 0; i < paginas.Count; i++)
        {
            paginas[i].SetActive(i == paginaAtual);
        }
    }

    public void FecharTutorial()
    {
        gameObject.transform.parent.gameObject.SetActive(false); // Desativa o Canvas pai
    }
}