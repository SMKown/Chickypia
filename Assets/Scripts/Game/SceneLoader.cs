using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;
    public PlayerStats playerstats;

    public Image Flame;
    public Image Desert;
    public Image Jungle;

    public bool isCanLoad = false;
    public bool isUIReander = false;

    private string SceneName;

    private Dictionary<string, Action> methodDictionary;
    private Dictionary<string, Image> SceneUIinteraction;

    private float elapsedTime;
    private float animationDuration = 0.25F;

    private void Start()
    {
        methodDictionary = new Dictionary<string, Action>
        {
            { "MainScene",      MainScene    },
            { "NewGame",        NewGame      },
            { "VillageScene",   VillageScene },
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

       SceneUIinteraction = new Dictionary<string, Image>
       {
           { "Flame01", Flame },
           { "Desert01", Desert },
           { "Jungle01", Jungle }
       };
    }    

    private void Update()
    {
        if (isCanLoad && Input.GetKeyDown(KeyCode.E))
        {
            if (methodDictionary.TryGetValue(SceneName, out Action method))
            {
                method.Invoke();
            }
            else
            {
                Debug.LogWarning($"No method mapped for scene: {SceneName}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isCanLoad = true;
        SceneName = other.gameObject.name;
        if (SceneUIinteraction.TryGetValue(SceneName, out Image image))
        {
            isUIReander = true;
            StartCoroutine(ImageOn(image));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isCanLoad = false;
        SceneName = other.gameObject.name;
        if (SceneUIinteraction.TryGetValue(SceneName, out Image image))
        {
            isUIReander = false;
            StartCoroutine(ImageOff(image));
        }
    }

    public void MainScene()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("MainScene");
    }
    public void NewGame()
    {
        ResetScene();
        LoadingSceneManager.LoadScene("Village");
    }
    public void VillageScene()
    {
        SaveAllBeforeSceneLoad();
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
        LoadingSceneManager.LoadScene("FishingScene");
    }
    public void Flame01()
    {
        SaveAllBeforeSceneLoad();
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

    private void SaveAllBeforeSceneLoad()
    {
        //인벤토리 저장
        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
            Debug.Log("인벤토리 저장");
        }
        else
        {
            Debug.LogWarning("InventoryManager 없음");
        }

        //도감 저장
        if (compendiumManager != null)
        {
            compendiumManager.SaveCompendium(); // 도감 저장
            Debug.Log("도감 저장");
        }
        else
        {
            Debug.LogWarning("CompendiumManager 없음");
        }

        if(playerstats != null)
        {
            playerstats.SavePlayerState();
            Debug.Log("플레이어 스탯 저장");
        }
        else
        {
            Debug.LogWarning("PlayerState 없음");
        }
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
