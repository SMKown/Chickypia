using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodEffect : MonoBehaviour
{
    public static FoodEffect instance;
    private ItemData itemData;
    private PlayerStats playerStats;
    
    public GameObject[] FoodEffectFxs;
    public GameObject[] FoodEffectUI;

    public GameObject timeManager;

    Dictionary<string, int> Effect = new Dictionary<string, int>()
    {
        {"HealHp", 0 },
        {"MaxHP", 1 },
        {"MoveSpeed", 2 },
        {"AttackDamage", 3 },
    };

    private void Awake()
    {
        if(instance != null )
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

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
        Effect.TryGetValue(itemData.effectType.ToString(), out int vlaue);

        playerStats.CurrentFoodEffectFxs = FoodEffectFxs[vlaue];
        if (vlaue >= 2)
        {
            playerStats.CurrentFoodEffectUI = FoodEffectUI[vlaue];
            GameObject effectTimer = Instantiate(timeManager, Vector3.zero, Quaternion.identity);
            TimerManager timerManager = effectTimer.GetComponent<TimerManager>();
            if (effectTimer != null)
            {
                timerManager.effectTime = itemData.time;
            }
        }

        CheckedEffect();
        UpdatePlayerStatus(itemData);
    }

    private void CheckedEffect()
    {
        if( playerStats.CurrentFoodEffectFxs != null )
        {
            playerStats.CurrentFoodEffectFxs.SetActive(true);
        }        
        if (playerStats.CurrentFoodEffectUI != null)
        {
            playerStats.CurrentFoodEffectUI.SetActive(true);
        }
    }

    private void UpdatePlayerStatus(ItemData itemData)
    {
        playerStats.currentHp = Mathf.Min(playerStats.currentHp + (itemData.hp != 0 ? itemData.hp : 0), playerStats.maxHp);
        playerStats.maxHp = Mathf.Min(playerStats.maxHp + (itemData.hpMax != 0 ? itemData.hpMax : 0), playerStats.FullMaxHp);
        playerStats.moveSpeed += (itemData.moveSpeed != 0 ? itemData.moveSpeed : 0);
        playerStats.attackDamage += (itemData.attackDamage != 0 ? itemData.attackDamage : 0);
    }

    public void EndEffect(float amount)
    {
        playerStats.moveSpeed -= amount;
        playerStats.CurrentFoodEffectFxs = null;
        playerStats.CurrentFoodEffectUI = null;
    }

    public void EndEffect(int amount)
    {
        playerStats.attackDamage -= amount;
        playerStats.CurrentFoodEffectFxs = null;
        playerStats.CurrentFoodEffectUI = null;
    }
}
