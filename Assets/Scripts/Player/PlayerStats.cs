using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHp = 3;
    public int currentHp;
    public float moveSpeed = 1.5f;
    public int attackDamage = 1;
    [HideInInspector] public float defaultMoveSpeed = 1.5f;
    [HideInInspector] public bool useItem = false;

    public GameObject[] FoodEffectFxs;
    public GameObject CurrentFoodEffectFys;
    public GameObject timeManager;

    public GameObject moveSpeedUI;
    public GameObject attackDamageUI;

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

    private void Start()
    {
        defaultMoveSpeed = moveSpeed;
    }

    public void ChangeHealHealth(int hpAmount)
    {
        CurrentFoodEffectFys = FoodEffectFxs[0];
        CurrentFoodEffectFys.SetActive(true);
        currentHp = Mathf.Min(currentHp + hpAmount, maxHp);
        SavePlayerState();
    }

    public void ChangeMaxHealth(int maxHpAmount)
    {
        CurrentFoodEffectFys = FoodEffectFxs[1];
        CurrentFoodEffectFys.SetActive(true);
        maxHp += maxHpAmount;
        currentHp = Mathf.Min(currentHp + maxHpAmount, maxHp);
        SavePlayerState();
    }

    public void ChangeMoveSpeed(float speedAmount, float time)
    {
        FoodEffectFxs[2].SetActive(true);
        moveSpeedUI.SetActive(true);
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
        attackDamageUI.SetActive(true);
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
        moveSpeedUI.SetActive(false);
        moveSpeed -= amount;

        SavePlayerState();
    }

    public void EndEffect(int amount)
    {
        FoodEffectFxs[3].SetActive(false);
        attackDamageUI.SetActive(false);
        attackDamage -= amount;

        SavePlayerState();
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