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
        //�κ��丮 ����
        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
            Debug.Log("Inventory saved before scene load.");
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }

        //���� ����
        if (compendiumManager != null)
        {
            compendiumManager.SaveCompendium(); // ���� ����
            Debug.Log("Compendium saved before scene load.");
        }
        else
        {
            Debug.LogWarning("CompendiumManager not found!");
        }
    }
}
