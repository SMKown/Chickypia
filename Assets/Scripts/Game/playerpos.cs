using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerpos : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        // �÷��̾��� ��ġ�� (5, 5, 5)�� ����
        player.transform.position = new Vector3(5f, 5f, 5f);
    }
}
