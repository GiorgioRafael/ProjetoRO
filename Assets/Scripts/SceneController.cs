using UnityEngine;
using UnityEngine.SceneManagement;

[System.Obsolete("This class is obsolete, and will be replaced by UILevelSelect")]
public class SceneController : MonoBehaviour
{
    public void SceneChange(string name)
    {
        //DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
    public void FecharJogo()
    {
        Debug.Log("O jogo está fechando..."); // Mensagem para o console da Unity (útil para testes no Editor)
        Application.Quit();

        // Se estiver testando no Editor da Unity, Application.Quit() pode não funcionar como esperado.
        // Para parar o jogo no Editor, você pode usar:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
