using UnityEngine;
using UnityEngine.SceneManagement;

[System.Obsolete("This class is obsolete, and will be replaced by UILevelSelect")]
public class SceneController : MonoBehaviour
{
    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
}
