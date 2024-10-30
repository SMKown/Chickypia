using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.VisualScripting;

public class SceneLoader : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent navMeshAgent;

    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;
    public PlayerStats playerstats;
    public QuestManager QuestManager;

    public Image Flame;
    public Image Desert;
    public Image Jungle;
    public Image Custom;
    public Image Village_fishing;
    public Image Village_Last;

    public bool isCanLoad = false;
    public bool isUIReander = false;

    private string sceneName;
    private string currentScenName;

    private Dictionary<string, Action> methodDictionary;
    private Dictionary<string, Image> sceneUIinteraction;

    private float elapsedTime;
    private float animationDuration = 0.25F;

    public GameObject startTransition;
    public GameObject endTransition;

    private Animation startAnimation;
    private Animation endAnimation;

    private Dictionary<(string, string), string> movePointMapping = new Dictionary<(string, string), string>
    {
        { ("Village",       "Flame01"),     "MovePoint01" },
        { ("Flame01",       "Flame02"),     "MovePoint01" },
        { ("Flame02",       "Flame03"),     "MovePoint01" },
        { ("Flame01",       "Village"),     "MovePoint01" },
        { ("Flame02",       "Flame01"),     "MovePoint02" },
        { ("Flame03",       "Flame02"),     "MovePoint02" },
        { ("Flame03",       "Village"),     "MovePoint01" },
        { ("Village",       "Jungle01"),    "MovePoint01" },
        { ("Jungle01",      "Jungle02"),    "MovePoint01" },
        { ("Jungle02",      "Jungle03"),    "MovePoint01" },
        { ("Jungle01",      "Village"),     "MovePoint03" },
        { ("Jungle02",      "Jungle01"),    "MovePoint02" },
        { ("Jungle03",      "Jungle02"),    "MovePoint02" },
        { ("Jungle03",      "Village"),     "MovePoint03" },
        { ("Village",       "Desert01"),    "MovePoint01" },
        { ("Desert01",      "Desert02"),    "MovePoint01" },
        { ("Desert02",      "Desert03"),    "MovePoint01" },
        { ("Desert01",      "Village"),     "MovePoint02" },
        { ("Desert02",      "Desert01"),    "MovePoint02" },
        { ("Desert03",      "Desert02"),    "MovePoint02" },
        { ("Desert03",      "Village"),     "MovePoint02" },
        { ("CustomScene",   "Village"),     "MovePoint04" },
        { ("FishingScene",  "Village"),     "MovePoint05" },
        { ("MainScene",     "Village"),     "MovePoint00" }
    };

    private void Start()
    {
        if (startTransition != null)
        {
            startAnimation = startTransition.GetComponent<Animation>();
            endAnimation = endTransition.GetComponent<Animation>();

            startTransition.SetActive(false);
            endAnimation.Play();
        }

        currentScenName = SceneManager.GetActiveScene().name;
        player = GameObject.FindWithTag("Player");

        methodDictionary = new Dictionary<string, Action>
        {
            { "MainScene",      MainScene    },
            { "NewGame",        NewGame      },
            { "Village",        VillageScene },
            { "CustomScene",    CustomScene  },
            { "FishingScene",   FishingScene },
            { "Flame01",        Flame01      },
            { "Flame02",        Flame02      },
            { "Flame03",        Flame03      },
            { "Jungle01",       Jungle01     },
            { "Jungle02",       Jungle02     },
            { "Jungle03",       Jungle03     },
            { "Desert01",       Desert01     },
            { "Desert02",       Desert02     },
            { "Desert03",       Desert03     }
        };

       sceneUIinteraction = new Dictionary<string, Image>
       {
           { "Flame01",     Flame },
           { "Desert01",    Desert },
           { "Jungle01",    Jungle },
           { "CustomScene", Custom },
           { "Village_fishing", Village_fishing },
           { "Village_Last", Village_Last },
       };
        SceneManager.sceneLoaded += OnSceneLoaded;
    }    

    private void Update()
    {
        if (isUIReander && Input.GetKeyDown(KeyCode.E))
        {
            if (isCanLoad == true && methodDictionary.TryGetValue(sceneName, out Action method))
            {
                StartCoroutine(ExecuteAfterTransition(method));
            }
        }
    }  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MoveScene"))
        {
            
            isCanLoad = true;
            sceneName = other.gameObject.name;
            if (sceneUIinteraction.TryGetValue(sceneName, out Image image) && sceneName == "Village_fishing")
            {
                isUIReander = true;
                StartCoroutine(ImageOn(image));
            }            
            else if (sceneUIinteraction.TryGetValue(sceneName, out Image _image) && sceneName == "Village")
            {
                isUIReander = true;
                StartCoroutine(ImageOn(image));
            }
            else if (isCanLoad == true && methodDictionary.TryGetValue(sceneName, out Action method))
            {
                StartCoroutine(ExecuteAfterTransition(method));
            }
            else
            {
                Debug.LogWarning($"No method mapped for scene: {sceneName}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MoveScene"))
        {
            isCanLoad = false;
            sceneName = other.gameObject.name;
            if (isUIReander == true && sceneUIinteraction.TryGetValue(sceneName, out Image image))
            {
                isUIReander = false;
                StartCoroutine(ImageOff(image));
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {        
        string targetScene = scene.name;
        if (targetScene == "LoadingScene")
        {
            Debug.Log("LoadingScene loaded, skipping player positioning.");
            return;
        }

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player object not found in the scene.");
            return;
        }

        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogWarning("NavMeshAgent not found on player object.");
            return;
        }


        if (movePointMapping.TryGetValue((currentScenName, targetScene), out string movePointName))
        {
            GameObject movePoint = GameObject.Find(movePointName);
            if (movePoint != null)
            {
                player.transform.position = movePoint.transform.position;
                navMeshAgent.Warp(movePoint.transform.position);
                Debug.Log($"이동 성공: 현재 씬 '{currentScenName}', 이동 씬 '{targetScene}', 이동 포인트 '{movePointName}'");
            }
            else
            {
                Debug.LogWarning($"이동 실패: 이동 포인트 '{movePointName}'를 씬 '{targetScene}'에서 찾을 수 없음");
            }
        }
        else
        {
            Debug.LogWarning($"매핑 누락: '{currentScenName}'에서 '{targetScene}'으로의 전환에 대한 매핑이 존재하지 않음");
        }
        currentScenName = targetScene;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    IEnumerator ExecuteAfterTransition(Action method)
    {
        startTransition.SetActive(true);
        startAnimation.Play();
        yield return StartCoroutine(Transitioner());

        method.Invoke(); 
    }

    IEnumerator Transitioner()
    {
        yield return new WaitForSeconds(0.8f);
    }

    #region 씬

    public void MainScene()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("MainScene");
    }
    public void NewGame()
    {
        ResetScene();
        playerstats.SetMoveSpeed(3.5f);
        LoadingSceneManager.LoadScene("Village");
    }
    public void DieScene()
    {
        SaveAllBeforeSceneLoad();
        if (playerstats != null)
        {
            playerstats.ResetPlayerState();
        }
        playerstats.SetMoveSpeed(3.5f);
        LoadingSceneManager.LoadScene("Village");
    }

    public void VillageScene()
    {
        SaveAllBeforeSceneLoad();
        if (playerstats != null)
            playerstats.SetMoveSpeed(3.5f);
        LoadingSceneManager.LoadScene("Village");
    }

    public void CustomScene()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("CustomScene");
    }

    public void FishingScene()
    {
        SaveAllBeforeSceneLoad();
        playerstats.ResetMoveSpeed();
        LoadingSceneManager.LoadScene("FishingScene");
    }
    public void Flame01()
    {
        SaveAllBeforeSceneLoad();
        playerstats.ResetMoveSpeed();
        LoadingSceneManager.LoadScene("Flame01");
    }
    public void Flame02()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Flame02");
    }
    public void Flame03()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Flame03");
    }
    public void Jungle01()
    {
        SaveAllBeforeSceneLoad();
        playerstats.ResetMoveSpeed();
        LoadingSceneManager.LoadScene("Jungle01");
    }
    public void Jungle02()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Jungle02");
    }
    public void Jungle03()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Jungle03");
    }
    public void Desert01()
    {
        SaveAllBeforeSceneLoad();
        playerstats.ResetMoveSpeed();
        LoadingSceneManager.LoadScene("Desert01");
    }
    public void Desert02()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Desert02");
    }
    public void Desert03()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Desert03");
    }

    public void ExitGame()
    {
        SaveAllBeforeSceneLoad();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    private void SaveAllBeforeSceneLoad()
    {
        //인벤토리 저장
        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
        }
        else
        {
            Debug.Log("InventoryManager 없음");
        }
        //도감 저장
        if (compendiumManager != null)
        {
            compendiumManager.SaveCompendium();
        }
        else
        {
            Debug.Log("CompendiumManager 없음");
        }
        //스탯 저장
        if(playerstats != null)
        {
            playerstats.SavePlayerState();
        }
        else
        {
            Debug.Log("PlayerState 없음");
        }
        //퀘스트 저장
        if (QuestManager != null)
        {
            QuestManager.SaveQuestProgress();
        }
        else
        {
            Debug.Log("QuestProgress 없음");  
        }

        isCanLoad = false;
        isUIReander = false;
    }

    private void ResetScene() // 리셋
    {
        if (inventoryManager != null)
        {
            inventoryManager.ResetInventory();
        }
        if (compendiumManager != null)
        {
            compendiumManager.ResetCompendium();
        }
        if (playerstats != null)
        {
            playerstats.ResetPlayerState();
        }
        if (QuestManager != null)
        {
            QuestManager.ResetQuestProgress();
        }
    }
    
    private IEnumerator ImageOn(Image image)
    {
        if (image == null)
        {
            yield break;
        }
        image.enabled = true;

        elapsedTime = 0F;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.transform.localScale = Vector3.one;
    }

    private IEnumerator ImageOff(Image image)
    {
        if (image == null)
        {
            yield break;
        }
        elapsedTime = 0F;
        while (elapsedTime < animationDuration)
        {
            if (image == null)
            {
                yield break;
            }
            float t = elapsedTime / animationDuration;
            image.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (image != null)
        {
            image.transform.localScale = Vector3.zero;
            image.enabled = false;
        }
    }
}