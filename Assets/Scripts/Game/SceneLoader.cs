using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;
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
            Debug.Log("Inventory saved before scene load.");
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }

        //도감 저장
        if (compendiumManager != null)
        {
            compendiumManager.SaveCompendium(); // 도감 저장
            Debug.Log("Compendium saved before scene load.");
        }
        else
        {
            Debug.LogWarning("CompendiumManager not found!");
        }
    }
}
