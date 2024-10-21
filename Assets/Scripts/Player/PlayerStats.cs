using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHp = 5;
    public int currentHp;
    public int attackDamage = 1;
    public float moveSpeed = 1.4f;

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
        moveSpeed += amount;
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

        Debug.Log("상태 저장: " + saveFilePath);
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

            Debug.Log("상태 불러오기: " + saveFilePath);
        }
        else
        {
            Debug.Log("저장파일 X");
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