using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public float timer = 0f;
    public float effectTime;
    public float _speedAmount;
    public int _attckDanageAmount;

    public int value;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= effectTime)
        {
            timer = 0;
            if (_speedAmount > 0)
            {
                FoodEffect.instance.EndEffect(_speedAmount);
            }
            else
            {
                FoodEffect.instance.EndEffect(_attckDanageAmount);
            }
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
