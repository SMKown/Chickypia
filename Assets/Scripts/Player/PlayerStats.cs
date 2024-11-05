using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    private InventoryManager inventoryManager;

    public int maxHp = 3;
    public int FullMaxHp = 7;
    public int currentHp;
    public float moveSpeed = 1.5f;
    public int attackDamage = 1;

    [HideInInspector] public float defaultMoveSpeed = 1.5f;
    [HideInInspector] public bool useItem = false;

    public GameObject CurrentFoodEffectFxs;
    public GameObject CurrentFoodEffectUI;

    private string saveFilePath;

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

    private void Start()
    {
        defaultMoveSpeed = moveSpeed;
    }

    public void ResetPlayerState()
    {
        maxHp = 3;
        currentHp = maxHp;        
        moveSpeed = 1.5f;
        attackDamage = 1;
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
            Debug.Log("저장파일 X");
        }
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        SavePlayerState();
    }

    public void ResetMoveSpeed()
    {
        moveSpeed = 1.5f;
        SavePlayerState();
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