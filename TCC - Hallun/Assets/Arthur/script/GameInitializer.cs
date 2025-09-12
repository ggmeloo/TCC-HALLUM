using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        // Este bloco de código só será executado na build final, não no Editor da Unity.
        // Isso evita que a janela do seu editor seja redimensionada toda vez que você aperta Play.
#if !UNITY_EDITOR
            // Força a resolução para 1920x1080 em modo tela cheia exclusivo.
            // O modo "Exclusive" geralmente oferece o melhor desempenho.
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
#endif
    }
}