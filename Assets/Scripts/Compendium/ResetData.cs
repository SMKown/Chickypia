using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CompendiumManager compendiumManager;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetInvenCompend();
        }
    }

    public void ResetInvenCompend()
    {
        
    }
}
