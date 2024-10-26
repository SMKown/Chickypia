using System;
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
    public GameObject timeManager;

    private string saveFilePath;

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

    public void ChangeHealHealth(int hpAmount)
    {
        currentHp = Mathf.Min(currentHp + hpAmount, maxHp);
        SavePlayerState();
        Debug.Log("ChangeHealHealth Start");
    }

    public void ChangeMaxHealth(int maxHpAmount)
    {
        maxHp += maxHpAmount;
        currentHp = Mathf.Min(currentHp + maxHpAmount, maxHp);
        SavePlayerState();
        Debug.Log("ChangeMaxHealth Start");
    }

    public void ChangeMoveSpeed(float speedAmount, float time)
    {
        moveSpeed += speedAmount;
        SavePlayerState();
        GameObject effecTimer = Instantiate(timeManager, Vector3.zero, Quaternion.identity);
        TimerManager timerManager = effecTimer.GetComponent<TimerManager>();
        if (timerManager != null)
        {
            timerManager.effectTime = time;
            timerManager._speedAmount = speedAmount;
        }
        Debug.Log("ChangeMoveSpeed Start");
    }

    public void ChangeAttackDamage(int attckDanageAmount, float time)
    {
        attackDamage += attckDanageAmount;
        SavePlayerState();
        GameObject effecTimer = Instantiate(timeManager, Vector3.zero, Quaternion.identity);
        TimerManager timerManager = effecTimer.GetComponent<TimerManager>();
        if (timerManager != null)
        {
            timerManager.effectTime = time;
            timerManager._attckDanageAmount = attckDanageAmount;
        }
        Debug.Log("ChangeAttackDamage Start");
    }    

    public void EndEffect(float amount)
    {
        moveSpeed -= amount;

        SavePlayerState();
    }

    public void EndEffect(int amount)
    {
        attackDamage -= amount;

        SavePlayerState();
    }

    public void ResetPlayerState()
    {
        maxHp = 3;
        currentHp = maxHp;
        attackDamage = 1;
        moveSpeed = 1.4f;
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