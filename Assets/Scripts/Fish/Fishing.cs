using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Fishing : MonoBehaviour
{
    private bool nibble = false; // 물고기 물음
    private Fish fishtype; // 물고기

    [SerializeField] private GameObject thoughtBubbles;

    private KeyCode fishingKey = KeyCode.E; // 낚시 키


    public Transform bobber;
    public Transform originPos;

    private void Update()
	{
        if (Input.GetMouseButtonDown(0) && !PlayerInfo.Instance.fishingMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();

                if (meshRenderer != null)
                {
                    if (meshRenderer.material.name.Contains("Water"))
                    {
                        Debug.Log("This is water!");

                        if (bobber != null)
                        {
                            bobber.position = hit.point;
                            CastLine();

                            Debug.Log("Bobber moved to: " + hit.point);
                        }
                    }
                }
            }
        }

		if (Input.GetKeyDown(fishingKey) && PlayerInfo.Instance.fishingMode && !nibble) // 물기 전 회수
		{
            if (bobber != null)
            {
                bobber.position = originPos.position;
            }

            StopAllCoroutines();
            PlayerInfo.Instance.fishingMode = false;
                
            // 생각 풍선 초기화
            thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
            thoughtBubbles.SetActive(false);
                
        }
		else if (Input.GetKeyDown(fishingKey) && PlayerInfo.Instance.fishingMode && nibble) // 물고 잡음
		{
            if (bobber != null)
            {
                bobber.position = originPos.position;
                Debug.Log("PlayerInfo.Instance.fishingMode bobber returned to original position: " + originPos);
            }

            StopAllCoroutines();

            // 물고기 잡기 코드
            FishCaught();
        }
    }
    
    // 줄 던지기 호출
    private void CastLine()
	{
        PlayerInfo.Instance.fishingMode = true;
        thoughtBubbles.SetActive(true); // 생각 풍선 활성화
        StartCoroutine(WaitForNibble(10)); // 물기를 기다리는 코루틴 시작
    }
    
    // 무작위 시간 대기
    private IEnumerator WaitForNibble(float maxWaitTime)
	{
        yield return new WaitForSeconds(Random.Range(maxWaitTime * 0.25f, maxWaitTime)); // 최대 대기 시간 사이의 무작위 시간 대기
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Alert"); // 알림 생각 풍선 표시
        nibble = true; 
        StartCoroutine(LineBreak(2)); // 반응 대기 후 줄이 끊어짐
    }
    
    // 반응 시간 초과 시 줄이 끊어짐
    private IEnumerator LineBreak(float lineBreakTime)
	{
        yield return new WaitForSeconds(lineBreakTime); // 대기 시간
        Debug.Log("물고기 놓침"); // 줄 끊어짐 로그 출력
        
        // 생각 풍선 비활성화
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
        thoughtBubbles.SetActive(false);

        PlayerInfo.Instance.fishingMode = false;
        nibble = false;
    }

    public void FishCaught()
	{
        PlayerInfo.Instance.fishingMode = false;
        nibble = false;

        fishtype = FishManager.GetRandomFish();

        Debug.Log($"잡은 물고기 : {fishtype.name}"); // 잡힌 물고기 로그 출력
        // 생각 풍선 초기화
        thoughtBubbles.SetActive(false);
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
    }
}
