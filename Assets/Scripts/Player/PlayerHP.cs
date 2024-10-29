using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public GameObject[] Heart;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public GameObject heartBroken;
    public SceneLoader sceneLoader;
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
        HPUI();

        if (PlayerStats.Instance.currentHp <= 0)
        {
            Die();
        }
    }

    private void HPUI()
    {
        for (int i = 0; i < Heart.Length; i++)
        {
            if (i < PlayerStats.Instance.maxHp)
            {
                Heart[i].gameObject.SetActive(true);
                Image heartImage = Heart[i].GetComponent<Image>();

                if (i < PlayerStats.Instance.currentHp)
                {
                    heartImage.sprite = fullHeart;
                }
                else /*if (i == PlayerStats.Instance.currentHp && PlayerStats.Instance.currentHp % 2 == 1)*/
                {
                    heartImage.sprite = halfHeart;
                }
            }
            else
            {
                Heart[i].SetActive(false);
            }
        }
    }

    private void Die()
    {
        if (sceneLoader != null)
        {
            sceneLoader.DieScene();
        }
        else
        {
            Debug.LogWarning("SceneLoader ������ �־�ߵ�");
        }
    }

    private IEnumerator HideHeartBroken()
    {
        yield return new WaitForSeconds(1.5f);
        heartBroken.SetActive(false);
    }
}
