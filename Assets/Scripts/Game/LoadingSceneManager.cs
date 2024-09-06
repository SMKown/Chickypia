using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public Slider slider;
    public static string loadScene;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        loadScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(loadScene);
        op.allowSceneActivation = false;
        float timer = 0.0F;
        float progress = 0.0F;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            float targetLerpTime = 1F;
            if (op.progress < 0.9F)
            {
                progress = Mathf.Lerp(progress, op.progress, timer);
                slider.value = progress;
                if (progress >= op.progress)
                {
                    timer = 0F;
                }
            }
            else
            {
                progress = Mathf.Lerp(progress, 1F, timer);
                slider.value = progress;
                if (progress == 1.0F)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
            timer = timer < targetLerpTime ? timer : targetLerpTime;
        }
    }
}
