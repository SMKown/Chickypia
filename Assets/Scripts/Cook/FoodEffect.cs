using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodEffect : MonoBehaviour
{
    public static FoodEffect instance;
    private ItemData itemData;

    public GameObject[] FoodEffectFxs;
    public GameObject[] FoodEffectUI;

    public GameObject timeManager;
    public List<TimerManager> activeSpeedTimers = new List<TimerManager>();
    public List<TimerManager> activeAttackTimers = new List<TimerManager>();

    private static readonly Dictionary<string, int> Effect = new Dictionary<string, int>()
    {
        {"HealHp", 0 },
        {"MaxHP", 1 },
        {"MoveSpeed", 2 },
        {"AttackDamage", 3 },
    };

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        var playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance is null in CheckedEffect().");
            return;
        }

        if (SceneManager.GetActiveScene().name != "LoadingScene")
        {
            CheckedEffect();
        }
    }

    public void GetFood(InventoryItem selectedItem)
    {
        itemData = selectedItem.item;

        EatFood(itemData);
    }

    private void EatFood(ItemData itemData)
    {
        var playerStats = PlayerStats.Instance;
        Effect.TryGetValue(itemData.effectType.ToString(), out int value);

        if (value >= 2)
        {
            GameObject effectTimer = Instantiate(timeManager, Vector3.zero, Quaternion.identity);
            TimerManager timerManager = effectTimer.GetComponent<TimerManager>();
            if (effectTimer != null)
            {
                if(value == 2)
                {
                    timerManager._speedAmount = itemData.moveSpeed;
                    activeSpeedTimers.Add(timerManager);
                }
                if(value == 3)
                {
                    timerManager._attckDanageAmount = itemData.attackDamage;
                    activeAttackTimers.Add(timerManager);
                }
                
                timerManager.effectTime = itemData.time;
                timerManager.value = value;
            }
        }

        CheckedEffect();
        UpdatePlayerStatus(itemData);
    }

    private void CheckedEffect()
    {
        var playerStats = PlayerStats.Instance;
        TimerManager[] timerManagers = FindObjectsOfType<TimerManager>();

        foreach (var timerManager in timerManagers)
        {
            int value = timerManager.value;

            if (value >= 0 && value < FoodEffectFxs.Length)
            {
                if (FoodEffectFxs[value] != null)
                {
                    FoodEffectFxs[value].SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"FoodEffectFxs[{value}] = null.");
                }
            }

            if (value >= 2 && (value - 2) < FoodEffectUI.Length)
            {
                if (FoodEffectUI[value - 2] != null)
                {
                    FoodEffectUI[value - 2].SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"FoodEffectUI[{value - 2}] = null.");
                }
            }
        }
    }

    private void UpdatePlayerStatus(ItemData itemData)
    {
        var playerStats = PlayerStats.Instance;
        playerStats.currentHp = Mathf.Min(playerStats.currentHp + (itemData.hp != 0 ? itemData.hp : 0), playerStats.maxHp);
        playerStats.maxHp = Mathf.Min(playerStats.maxHp + (itemData.hpMax != 0 ? itemData.hpMax : 0), playerStats.FullMaxHp);
        playerStats.moveSpeed += (itemData.moveSpeed != 0 ? itemData.moveSpeed : 0);
        playerStats.attackDamage += (itemData.attackDamage != 0 ? itemData.attackDamage : 0);
    }

    public void EndEffect(float amount)
    {
        var playerStats = PlayerStats.Instance;
        playerStats.moveSpeed -= amount;
        if (activeSpeedTimers.Count > 0)
        {
            activeSpeedTimers.RemoveAt(activeSpeedTimers.Count - 1);
        }

        if (activeSpeedTimers.Count == 0)
        {
            ResetEffect(playerStats, 2);
        }
    }

    public void EndEffect(int amount)
    {
        var playerStats = PlayerStats.Instance;
        playerStats.attackDamage -= amount;
        if (activeAttackTimers.Count > 0)
        {
            activeAttackTimers.RemoveAt(activeAttackTimers.Count - 1);
        }

        if (activeAttackTimers.Count == 0)
        {
            ResetEffect(playerStats, 3);
        }
    }

    private void ResetEffect(PlayerStats playerStats, int value)
    {
        if (FoodEffectFxs[value] != null)
        {
            FoodEffectFxs[value].SetActive(false);
        }

        if (FoodEffectUI[value - 2] != null)
        {                        
            FoodEffectUI[value - 2].SetActive(false);
        }
    }
}
