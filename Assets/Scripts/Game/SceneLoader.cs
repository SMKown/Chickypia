using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public void MainScene()
    {
        SaveInventoryBeforeSceneLoad();
        LoadingSceneManager.LoadScene("MainScene");
    }

    public void CustomScene()
    {
        LoadingSceneManager.LoadScene("CustomScene");
    }

    public void FishingScene()
    {
        SaveInventoryBeforeSceneLoad();
        LoadingSceneManager.LoadScene("FishingScene");
    }

    public void ExitGame()
    {
        SaveInventoryBeforeSceneLoad();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SaveInventoryBeforeSceneLoad()
    {
        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
            Debug.Log("Inventory saved before scene load.");
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }
    }
}
