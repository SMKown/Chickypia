using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void MainScene()
    {
        LoadingSceneManager.LoadScene("MainScene");
    }

    public void CustomScene()
    {
        LoadingSceneManager.LoadScene("CustomScene");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
