using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public bool isSpeed;
    public bool isPower;

    void Start()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
