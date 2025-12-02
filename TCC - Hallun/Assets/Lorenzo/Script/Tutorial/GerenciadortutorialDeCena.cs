using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; // Adicionei suporte a TextMeshPro caso queira mudar o texto do botão

public class GerenciadorTutorialDeCena : MonoBehaviour
{
    [Header("Configuração das Páginas")]
    [Tooltip("Arraste todas as suas imagens de página para esta lista, NA ORDEM CORRETA.")]
    public List<GameObject> paginas;

    [Header("Configuração dos Botões")]
    public Button botaoProximo;
    public Button botaoAnterior;

    [Tooltip("Opcional: Arraste o texto do botão 'Próximo' para mudar para 'JOGAR' no final")]
    public TextMeshProUGUI textoBotaoProximo;

    [Header("Cena do Jogo")]
    [Tooltip("Digite o nome EXATO da sua cena de gameplay principal.")]
    public string nomeCenaPrincipal = "GameScene";

    private int paginaAtual = 0;
    private AsyncOperation operacaoDeCarregamento; // Variável para controlar o carregamento escondido

    void Start()
    {
        // 1. Configura cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 2. Inicia na primeira página
        paginaAtual = 0;
        AtualizarVisibilidade();

        // 3. COMEÇA A CARREGAR O JOGO NO FUNDO (Essa é a mágica)
        StartCoroutine(CarregarJogoEmSegundoPlano());
    }

    // Carrega o jogo mas impede que ele abra sozinho
    IEnumerator CarregarJogoEmSegundoPlano()
    {
        if (string.IsNullOrEmpty(nomeCenaPrincipal))
        {
            Debug.LogError("Nome da cena principal não definido!");
            yield break;
        }

        // Começa a carregar
        operacaoDeCarregamento = SceneManager.LoadSceneAsync(nomeCenaPrincipal);

        // IMPEDE que a cena mude assim que terminar de carregar
        operacaoDeCarregamento.allowSceneActivation = false;

        Debug.Log("Carregando jogo em segundo plano...");

        yield return null;
    }

    public void AcaoBotaoPrincipal()
    {
        // Se NÃO estamos na última página, avança
        if (paginaAtual < paginas.Count - 1)
        {
            paginaAtual++;
            AtualizarVisibilidade();
        }
        else
        {
            // SE ESTAMOS na última página, libera a cena que já está carregada
            FinalizarTutorialEEntrar();
        }
    }

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

        // Configura Botão Anterior
        if (botaoAnterior != null)
            botaoAnterior.interactable = (paginaAtual > 0);

        // Configura Botão Próximo / Jogar
        if (botaoProximo != null)
        {
            // Se chegou na última página
            if (paginaAtual == paginas.Count - 1)
            {
                // Muda o texto para "JOGAR" ou "CONCLUIR"
                if (textoBotaoProximo != null) textoBotaoProximo.text = "JOGAR";
            }
            else
            {
                // Se não, mantém "PRÓXIMO"
                if (textoBotaoProximo != null) textoBotaoProximo.text = "PRÓXIMO";
            }
        }
    }

    private void FinalizarTutorialEEntrar()
    {
        // Se o carregamento já começou
        if (operacaoDeCarregamento != null)
        {
            // Libera a trava e entra no jogo
            operacaoDeCarregamento.allowSceneActivation = true;
        }
        else
        {
            // Fallback caso o carregamento assíncrono tenha falhado
            SceneManager.LoadScene(nomeCenaPrincipal);
        }
    }
}