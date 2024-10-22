using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public GameObject heartBroken;
    private void Start()
    {
        HPUI();
    }

    public void TakeDamage(int damage)
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerStats.Instance�� null�Դϴ�.");
            return;
        }
        Debug.Log("�÷��̾ �������� �޾ҽ��ϴ�: " + damage);
        PlayerStats.Instance.ChangeHealHealth(-damage);

        if (heartBroken != null)
        {
            heartBroken.SetActive(true);
            StartCoroutine(HideHeartBroken());
        }

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
        // ��� ó��
    }

    private IEnumerator HideHeartBroken()
    {
        yield return new WaitForSeconds(1.5f);
        heartBroken.SetActive(false);
    }
}
