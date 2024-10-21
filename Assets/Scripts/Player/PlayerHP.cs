using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    private void Start()
    {
        HPUI();
    }

    public void TakeDamage(int damage)
    {
        PlayerStats.Instance.ChangeHealHealth(-damage);

        if (PlayerStats.Instance.currentHp <= 0)
        {
            Die();
        }
        HPUI();
    }

    private void HPUI()
    {

    }

    private void Die()
    {
        // »ç¸Á Ã³¸®
    }
}
