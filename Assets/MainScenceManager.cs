using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    public GameObject sceneLoader;

    private AudioSource mainBGM;

    public GameObject mainPenal;
    public GameObject introPenal;

    public Button play;
    public Button reStart;

    void Start()
    {
        play.onClick.AddListener(PlayGame);
        reStart.onClick.AddListener(ReStartGame);

        mainBGM = SoundManager.Instance.GetComponent<AudioSource>();
    }

    private void PlayGame()
    {
        StartCoroutine(PlayOpeningVideo(() =>
        {
            sceneLoader.GetComponent<SceneLoader>().VillageScene();
        }));
    }

    private void ReStartGame()
    {
        StartCoroutine(PlayOpeningVideo(() =>
        {
            sceneLoader.GetComponent<SceneLoader>().NewGame();
        }));
    }

    private IEnumerator PlayOpeningVideo(System.Action onVideoEnd)
    {
        mainPenal.SetActive(false);
        introPenal.SetActive(true);
        mainBGM.mute = true;

        float timer = 0f;
        while (timer < 10f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        //introPenal.SetActive(false);
        onVideoEnd?.Invoke();
    }
}
