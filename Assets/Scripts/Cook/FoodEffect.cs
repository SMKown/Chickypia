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
    public PlayerHP playerHp;

    public GameObject timeManager;
    public List<TimerManager> Timers = new List<TimerManager>();

    private static readonly Dictionary<string, int> Effect = new Dictionary<string, int>()
    {
        {"HealHp", 0 },
        {"MaxHP", 1 },
        {"AttackDamage", 2 },
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
            Debug.LogError("PlayerStats instance");
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
                    timerManager._attckDanageAmount = itemData.attackDamage;
                    Timers.Add(timerManager);
                }
                timerManager.effectTime = itemData.time;
                timerManager.value = value;
            }
        }
        else
        {
            FoodEffectFxs[value].SetActive(true);
        }

        CheckedEffect();
        UpdatePlayerStatus(itemData);
        
        if (playerHp != null)
            playerHp.HPUI();
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
        playerStats.attackDamage = Mathf.Min(playerStats.attackDamage + (itemData.attackDamage != 0 ? itemData.attackDamage : 0), playerStats.MaxAttackDamage);
    }

    public void EndEffect(int amount)
    {
        var playerStats = PlayerStats.Instance;
        playerStats.attackDamage -= amount;
        if (Timers.Count > 0)
        {
            Timers.RemoveAt(Timers.Count - 1);
            ResetEffect(playerStats, 2);
        }
    }

    private void ResetEffect(PlayerStats playerStats, int value)
    {
        Debug.Log(value);
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
