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
    public float moveSpeed = 1.4f;
    public int attackDamage = 1;    

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
        FoodEffectFxs[0].SetActive(true);
        currentHp = Mathf.Min(currentHp + hpAmount, maxHp);
        SavePlayerState();
    }

    public void ChangeMaxHealth(int maxHpAmount)
    {
        FoodEffectFxs[1].SetActive(true);
        maxHp += maxHpAmount;
        currentHp = Mathf.Min(currentHp + maxHpAmount, maxHp);
        SavePlayerState();
    }

    public void ChangeMoveSpeed(float speedAmount, float time)
    {
        FoodEffectFxs[2].SetActive(true);
        moveSpeed += speedAmount;
        SavePlayerState();
        GameObject effecTimer = Instantiate(timeManager, Vector3.zero, Quaternion.identity);
        TimerManager timerManager = effecTimer.GetComponent<TimerManager>();
        if (timerManager != null)
        {
            timerManager.effectTime = time;
            timerManager._speedAmount = speedAmount;
        }
    }

    public void ChangeAttackDamage(int attckDanageAmount, float time)
    {
        FoodEffectFxs[3].SetActive(true);
        attackDamage += attckDanageAmount;
        SavePlayerState();
        GameObject effecTimer = Instantiate(timeManager, Vector3.zero, Quaternion.identity);
        TimerManager timerManager = effecTimer.GetComponent<TimerManager>();
        if (timerManager != null)
        {
            timerManager.effectTime = time;
            timerManager._attckDanageAmount = attckDanageAmount;
        }
    }    

    public void EndEffect(float amount)
    {
        FoodEffectFxs[2].SetActive(false);
        moveSpeed -= amount;

        SavePlayerState();
    }

    public void EndEffect(int amount)
    {
        FoodEffectFxs[3].SetActive(false);
        attackDamage -= amount;

        SavePlayerState();
    }

    public void ResetPlayerState()
    {
        maxHp = 3;
        currentHp = maxHp;        
        moveSpeed = 1.4f;
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