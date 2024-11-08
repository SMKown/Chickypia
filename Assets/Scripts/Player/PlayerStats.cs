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

    public int maxHp = 4;
    public int FullMaxHp = 7;
    public int currentHp;
    public float moveSpeed = 2f;
    public int attackDamage = 1;
    public int MaxAttackDamage = 5;

    [HideInInspector] public float defaultMoveSpeed = 2f;
    [HideInInspector] public bool useItem = false;

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
        maxHp = 4;
        currentHp = maxHp;        
        moveSpeed = 2f;
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
            Debug.Log(" No have SaveFile");
        }
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        SavePlayerState();
    }

    public void ResetMoveSpeed()
    {
        moveSpeed = 2f;
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