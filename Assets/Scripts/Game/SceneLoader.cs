using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;
    public void MainScene()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("MainScene");
    }

    public void CustomScene()
    {
        LoadingSceneManager.LoadScene("CustomScene");
    }

    public void FishingScene()
    {
        SaveAllBeforeSceneLoad();
        LoadingSceneManager.LoadScene("FishingScene");
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
