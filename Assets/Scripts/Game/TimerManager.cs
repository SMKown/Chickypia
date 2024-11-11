using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public float timer = 0f;
    public float saveTimer = 0f;

    public float effectTime;
    public int _attckDanageAmount;

    public int value;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name != "LoadingScene")
        {
            timer += Time.deltaTime;
            saveTimer = timer;
        }
        else
        {
            timer = saveTimer;
        }
        
        if (timer >= effectTime)
        {
            FoodEffect.instance.EndEffect(_attckDanageAmount);
            Destroy(gameObject);
        }
        
        if(PlayerStats.Instance.currentHp == 0)
        {
            FoodEffect.instance.EndEffect(0);
            Destroy(gameObject);
        }
    }

    public float TimerValue
    {
        get { return timer; }
    }

    public void ResetTimer()
    {
        timer = 0f;
    }
}
