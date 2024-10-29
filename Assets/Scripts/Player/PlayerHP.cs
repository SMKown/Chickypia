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
            Debug.LogError("PlayerStats.Instance가 null입니다.");
            return;
        }
        Debug.Log("플레이어가 데미지를 받았습니다: " + damage);
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
            Debug.LogWarning("SceneLoader 없으니 넣어야됨");
        }
    }

    private IEnumerator HideHeartBroken()
    {
        yield return new WaitForSeconds(1.5f);
        heartBroken.SetActive(false);
    }
}
