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
    public bool shouldTurn = false;
    public bool shouldAttack = false;
    public bool isGround = true;

    // private string prefabPath = "Prefabs/Player/Model"; // Resources 폴더 내의 경로 (확장자 제외)

    // private void Start()
    // {
    //     GameObject prefab = Resources.Load<GameObject>(prefabPath);

    //     if (prefab != null)
    //     {
    //         GameObject instance = Instantiate(prefab, transform);
    //         instance.transform.SetParent(transform);
    //     }
    //     else
    //     {
    //         Debug.LogError("Prefab not found at path: " + prefabPath);
    //     }
    // }
}
