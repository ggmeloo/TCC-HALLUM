using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Essencial para carregar a cena do jogo

public class GerenciadorTutorialDeCena : MonoBehaviour
{
    [Header("Configuração das Páginas")]
    [Tooltip("Arraste todas as suas imagens de página para esta lista, NA ORDEM CORRETA.")]
    public List<GameObject> paginas;

    [Header("Configuração dos Botões")]
    [Tooltip("Arraste o botão de avançar para cá.")]
    public Button botaoProximo;
    [Tooltip("Arraste o botão de voltar para cá.")]
    public Button botaoAnterior;

    [Header("Cena do Jogo")]
    [Tooltip("Digite o nome EXATO da sua cena de gameplay principal.")]
    public string nomeCenaPrincipal;

    private int paginaAtual = 0;

    // Start é chamado quando a cena começa
    void Start()
    {
        // Garante que o cursor do mouse esteja visível e livre
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Inicia na primeira página
        paginaAtual = 0;
        AtualizarVisibilidade();
    }

    // Função que será chamada pelo botão "Próximo"
    public void AcaoBotaoPrincipal()
    {
        // Se NÃO estamos na última página...
        if (paginaAtual < paginas.Count - 1)
        {
            // ...avança para a próxima página.
            paginaAtual++;
            AtualizarVisibilidade();
        }
        else
        {
            // ...SE ESTAMOS na última página, carrega a cena principal do jogo.
            IniciarJogo();
        }
    }

    // Função que será chamada pelo botão "Anterior"
    public void PaginaAnterior()
    {
        if (paginaAtual > 0)
        {
            paginaAtual--;
            AtualizarVisibilidade();
        }
    }

    private void AtualizarVisibilidade()
    {
        // Mostra a página atual e esconde as outras
        for (int i = 0; i < paginas.Count; i++)
        {
            paginas[i].SetActive(i == paginaAtual);
        }

        // Ativa/Desativa o botão "Anterior"
        if (botaoAnterior != null) botaoAnterior.interactable = (paginaAtual > 0);

        // O botão "Próximo" está sempre ativo, mas sua função muda na última página
        if (botaoProximo != null) botaoProximo.interactable = true;
    }

    private void IniciarJogo()
    {
        // Verifica se um nome de cena foi fornecido
        if (!string.IsNullOrEmpty(nomeCenaPrincipal))
        {
            SceneManager.LoadScene(nomeCenaPrincipal);
        }
        else
        {
            Debug.LogError("O 'Nome Cena Principal' não foi definido no Inspector!");
        }
    }
}