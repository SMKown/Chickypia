using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerpos : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        // 플레이어의 위치를 (5, 5, 5)로 설정
        player.transform.position = new Vector3(5f, 5f, 5f);
    }
}
