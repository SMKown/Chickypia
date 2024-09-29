using UnityEngine;

public class EnemyGridAli : MonoBehaviour
{
    public GameObject[] enemies;  // 배치할 적 캐릭터들
    public Vector3 startPosition = Vector3.zero;  // 배치를 시작할 위치
    public float rowSpacing = 2.0f;  // 줄 사이의 간격
    public float columnSpacing = 2.0f;  // 열 사이의 간격
    public int enemiesPerRow = 5;  // 한 줄에 배치할 적 캐릭터 수

    void Start()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            // 줄과 열 계산
            int row = i / enemiesPerRow;
            int column = i % enemiesPerRow;

            // 새 위치 계산
            Vector3 newPosition = startPosition + new Vector3(column * columnSpacing, 0, row * rowSpacing);

            // 적 캐릭터 위치 이동
            enemies[i].transform.position = newPosition;
        }
    }
}
