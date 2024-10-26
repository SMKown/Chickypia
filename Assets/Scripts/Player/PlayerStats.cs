using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHp = 3;
    public int currentHp;
    public int attackDamage = 1;
    public float moveSpeed = 1.4f;

    [HideInInspector]public bool useItem = false;

    public GameObject[] FoodEffectFxs;

    private string saveFilePath;

    // 000 public FoodRecipeData food;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerState.json");

        LoadPlayerState();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void ChangeMaxHealth(int amount)
    {
        maxHp += amount;
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        SavePlayerState();
    }

    public void ChangeHealHealth(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        SavePlayerState();
    }

    public void ChangeAttackDamage(int amount)
    {
        attackDamage += amount;
        SavePlayerState();
    }

    public void ChangeMoveSpeed(float amount)
    {
        //FoodEffectFxs[0].SetActive(true);
        moveSpeed += amount;
        SavePlayerState();
    }

    public void ResetPlayerState()
    {
        maxHp = 3;
        currentHp = maxHp;
        attackDamage = 1;
        moveSpeed = 1.4f;
        Debug.Log("PlayerState Reset");
        SavePlayerState();
    }

    public void SavePlayerState()
    {
        PlayerData data = new PlayerData
        {
            maxHp = this.maxHp,
            currentHp = this.currentHp,
            attackDamage = this.attackDamage,
            moveSpeed = this.moveSpeed
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }
    public void LoadPlayerState()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            this.maxHp = data.maxHp;
            this.currentHp = data.currentHp;
            this.attackDamage = data.attackDamage;
            this.moveSpeed = data.moveSpeed;
        }
        else
        {
            Debug.LogError("저장파일 X");
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public int maxHp;
        public int currentHp;
        public int attackDamage;
        public float moveSpeed;
    }
}