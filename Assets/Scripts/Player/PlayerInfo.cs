using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool attackMode = false;

    public bool moving = false;
    public bool interacting = false;
    public bool attacking = false;
    public bool fishing = false;

    public bool isGround = true;

    public bool UnableMove()
    {
        if (interacting || attacking || fishing) return true;
        else return false;
    }
}
