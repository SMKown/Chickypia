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

    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;
    public PlayerStats playerstats;
    public QuestManager QuestManager;

    public Image Flame;
    public Image Desert;
    public Image Jungle;
    public Image Village;

    public bool isCanLoad = false;
    public bool isUIReander = false;

    private string sceneName;
    private string currentScenName;

    private Dictionary<string, Action> methodDictionary;
    private Dictionary<string, Image> sceneUIinteraction;

    private float elapsedTime;
    private float animationDuration = 0.25F;

    private void Start()
    {
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
           { "Flame01", Flame },
           { "Desert01", Desert },
           { "Jungle01", Jungle },
           { "Village", Village },
       };

        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 savedPosition = PlayerPositionManager.instance.GetSavedPosition(currentScene);

        if (savedPosition != Vector3.zero && player != null)
        {
            player.transform.position = savedPosition;

            // NavMeshAgent �ʱ�ȭ
            NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.ResetPath();
                agent.destination = player.transform.position;
            }
        }
    }    

    private void Update()
    {
        if (isUIReander && Input.GetKeyDown(KeyCode.E))
        {
            if (isCanLoad == true && methodDictionary.TryGetValue(sceneName, out Action method))
            {
                method.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("MoveScene"))
        {
            isCanLoad = true;
            sceneName = other.gameObject.name;
            currentScenName = SceneManager.GetActiveScene().name;
            if (sceneUIinteraction.TryGetValue(sceneName, out Image image) && currentScenName == "Village")
            {
                isUIReander = true;
                StartCoroutine(ImageOn(image));
            }
            else if (isCanLoad == true && methodDictionary.TryGetValue(sceneName, out Action method))
            {
                method.Invoke();
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

    #region ��

    public void MainScene()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("MainScene");
    }
    public void NewGame()
    {
        ResetScene();
        playerstats.SetMoveSpeed(3.0f);
        LoadingSceneManager.LoadScene("Village");
    }
    public void DieScene()
    {
        SaveAllBeforeSceneLoad();
        if (playerstats != null)
        {
            playerstats.ResetPlayerState();
        }
        playerstats.SetMoveSpeed(3.0f);
        LoadingSceneManager.LoadScene("Village");
    }

    public void VillageScene()
    {
        SaveAllBeforeSceneLoad();
        playerstats.SetMoveSpeed(3.0f);
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

    // Village �� Flame01 �� Flame02 �� Flame03 �� Village
    // Village �� Jungle01 �� Jungle02 �� Jungle03 �� Village
    // Village �� Desert01 �� Desert02 �� Desert03 �� Village
    // FishingScene �� Village
    // CustomScene �� Village
    #endregion

    private void SaveAllBeforeSceneLoad()
    {
        if (player != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            Vector3 playerPosition = player.transform.position;
            PlayerPositionManager.instance.SavePosition(currentScene, playerPosition);
        }

        //�κ��丮 ����
        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
        }
        else
        {
            Debug.Log("InventoryManager ����");
        }
        //���� ����
        if (compendiumManager != null)
        {
            compendiumManager.SaveCompendium();
        }
        else
        {
            Debug.Log("CompendiumManager ����");
        }
        //���� ����
        if(playerstats != null)
        {
            playerstats.SavePlayerState();
        }
        else
        {
            Debug.Log("PlayerState ����");
        }
        //����Ʈ ����
        if (QuestManager != null)
        {
            QuestManager.SaveQuestProgress();
        }
        else
        {
            Debug.Log("QuestProgress ����");  
        }

        isCanLoad = false;
        isUIReander = false;
    }

    private void ResetScene() // ����
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