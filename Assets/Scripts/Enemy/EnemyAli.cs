using UnityEngine;

public class EnemyGridAli : MonoBehaviour
{
    public GameObject[] enemies;  // ��ġ�� �� ĳ���͵�
    public Vector3 startPosition = Vector3.zero;  // ��ġ�� ������ ��ġ
    public float rowSpacing = 2.0f;  // �� ������ ����
    public float columnSpacing = 2.0f;  // �� ������ ����
    public int enemiesPerRow = 5;  // �� �ٿ� ��ġ�� �� ĳ���� ��

    void Start()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            // �ٰ� �� ���
            int row = i / enemiesPerRow;
            int column = i % enemiesPerRow;

            // �� ��ġ ���
            Vector3 newPosition = startPosition + new Vector3(column * columnSpacing, 0, row * rowSpacing);

            // �� ĳ���� ��ġ �̵�
            enemies[i].transform.position = newPosition;
        }
    }
}
