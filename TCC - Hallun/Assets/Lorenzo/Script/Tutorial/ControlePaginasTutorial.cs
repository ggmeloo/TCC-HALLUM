using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControlePaginasTutorial : MonoBehaviour
{
    // --- NOVA VARIÁVEL AQUI ---
    [Header("Referência de Retorno")]
    [Tooltip("Arraste o objeto 'Papeis_Tutorial' que tem o script de interação para cá.")]
    public PapeisInterativosTutorial scriptDosPapeis; // Referência ao outro script

    [Header("Configuração das Páginas")]
    public List<GameObject> paginas;
    [Header("Configuração dos Botões")]
    public Button botaoProximo;
    public Button botaoAnterior;

    // ... (o resto do script continua o mesmo: OnEnable, AcaoBotaoPrincipal, etc.) ...
    // ...

    public void FecharTutorial()
    {
        // Esconde o cursor e trava a tela
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Desativa o Canvas do tutorial
        gameObject.transform.parent.gameObject.SetActive(false);

        // --- AVISO DE RETORNO ---
        // Se a referência existir, chama a função para resetar a interação
        if (scriptDosPapeis != null)
        {
            scriptDosPapeis.ResetarInteracao();
        }
    }
}