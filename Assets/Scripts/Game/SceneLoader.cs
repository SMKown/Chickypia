using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;
    public PlayerStats playerstats;

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
}
