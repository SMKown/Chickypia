using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

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
    public Image Village;
    public Image Custom;

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
    public GameObject f_startTransition;
    public GameObject c_startTransition;

    private Animation startAnimation;
    private Animation endAnimation;
    private Animation f_startAnimation;
    private Animation c_startAnimation;

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
        if (f_startTransition != null || c_startTransition != null)
        {
            f_startAnimation = f_startTransition.GetComponent<Animation>();
            c_startAnimation = c_startTransition.GetComponent<Animation>();

            f_startTransition.SetActive(false);
            c_startTransition.SetActive(false);
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
           { "Flame01",      Flame },
           { "Desert01",     Desert },
           { "Jungle01",     Jungle },
           { "CustomScene",  Custom },
           { "Village",      Village },
       };
        SceneManager.sceneLoaded += OnSceneLoaded;
    }    

    private void Update()
    {
        if (isUIReander && Input.GetKeyDown(KeyCode.E))
        {
            if (isCanLoad == true && methodDictionary.TryGetValue(sceneName, out Action method))
            {
                StartCoroutine(AfterTransition(method));
            }
        }
    }  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MoveScene"))
        {
            isCanLoad = true;
            sceneName = other.gameObject.name;

            if (sceneUIinteraction.TryGetValue(sceneName, out Image image) && (currentScenName == "Village" || currentScenName == "FishingScene"))
            {
                isUIReander = true;
                StartCoroutine(ImageOn(image));
            }
            else if (isCanLoad == true && methodDictionary.TryGetValue(sceneName, out Action method))
            {
                StartCoroutine(AfterTransition(method));
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

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            return;
        }

        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            return;
        }

        if (movePointMapping.TryGetValue((currentScenName, targetScene), out string movePointName))
        {
            GameObject movePoint = GameObject.Find(movePointName);
            if (movePoint != null)
            {
                player.transform.position = movePoint.transform.position;
                navMeshAgent.Warp(movePoint.transform.position);
            }
        }
        currentScenName = targetScene;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    IEnumerator AfterTransition(Action method)
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
        LoadingSceneManager.LoadScene("Village");
        playerstats.SetMoveSpeed(playerstats.moveSpeed + 2);
    }
    public void DieScene()
    {
        SaveAllBeforeSceneLoad();
        if (playerstats != null)
        {
            playerstats.ResetPlayerState();
        }
        LoadingSceneManager.LoadScene("Village");
        playerstats.SetMoveSpeed(playerstats.moveSpeed + 2);
    }

    public void VillageScene()
    {
        if (SceneManager.GetActiveScene().name == "CustomScene")
        {
            StartCoroutine(Transitioner());
        }

        SaveAllBeforeSceneLoad();
        StartCoroutine(LoadVillageScene());
    }

    private IEnumerator LoadVillageScene()
    {
        startTransition.SetActive(true);
        startAnimation.Play();
        yield return StartCoroutine(Transitioner());

        LoadingSceneManager.LoadScene("Village");
        playerstats.SetMoveSpeed(playerstats.moveSpeed + 2);
    }

    public void CustomScene()
    {
        StartCoroutine(AfterTransition(CustomScene));
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("CustomScene");
    }

    public void FishingScene()
    {
        StartCoroutine(Transitioner());

        SaveAllBeforeSceneLoad();
        StartCoroutine(LoadFishingScene());
        playerstats.SetMoveSpeed(playerstats.moveSpeed - 2);

    }

    private IEnumerator LoadFishingScene()
    {
        startTransition.SetActive(true);
        startAnimation.Play();
        yield return StartCoroutine(Transitioner());

        LoadingSceneManager.LoadScene("FishingScene");
    }

    public void Flame01()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("Flame01");
        LeavingVillage();
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
        LoadingSceneManager.LoadScene("Jungle01");
        LeavingVillage();
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
        LoadingSceneManager.LoadScene("Desert01");
        LeavingVillage();
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

    private void LeavingVillage()
    {
        if (currentScenName == "Village")
        {
            playerstats.SetMoveSpeed(playerstats.moveSpeed - 2);
        }
    }

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

        if (CharacterBase.Instance != null)
        {
            CharacterBase.Instance.ClearInfo();
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