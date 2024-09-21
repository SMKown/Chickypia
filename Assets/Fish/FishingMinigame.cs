using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FishingMinigame : MonoBehaviour
{
    private bool lineCast = false; // 낚시줄을 던졌는지 여부
    private bool nibble = false; // 물고기가 물고 있는지 여부
    public bool reelingFish = false; // 물고기를 끌어당기고 있는지 여부

    private Fish currentFishOnLine; // 현재 줄에 걸린 물고기

    [Header("Setup References")]
    [SerializeField] private GameObject catchingbar; // 낚시바를 표시하는 오브젝트
    private Vector3 catchingBarLoc; // 낚시바의 초기 위치
    private Rigidbody2D catchingBarRB; // 낚시바의 Rigidbody

    [SerializeField] private GameObject fishBar; // 물고기 바
    private FishingMinigame_FishTrigger fishTrigger; // 물고기 트리거
    private bool inTrigger = false; // 물고기가 바 안에 있는지 여부

    private float catchPercentage = 0f; // 잡힐 확률
    [SerializeField] private Slider catchProgressBar; // 잡힐 확률 슬라이더

    [SerializeField] private GameObject thoughtBubbles; // 생각 풍선
    [SerializeField] private GameObject minigameCanvas; // 미니게임 캔버스

    [Header("Settings")]
    [SerializeField] private KeyCode fishingKey = KeyCode.E; // 낚시 키
    [SerializeField] private float catchMultiplier = 10f; // 물고기를 더 빨리 잡을 수 있게 해주는 배수
    [SerializeField] private float catchingForce; // 낚시바를 올리는 힘

    private void Start()
	{
        catchingBarRB = catchingbar.GetComponent<Rigidbody2D>(); // 낚시바의 Rigidbody 참조 가져오기
        catchingBarLoc = catchingbar.GetComponent<RectTransform>().localPosition; // 낚시바의 초기 위치 저장
    }

    private void Update()
	{
        if (!reelingFish)
		{ // 물고기를 끌어당기지 않을 때
            if (Input.GetKeyDown(fishingKey) && !lineCast)
			{ // 줄을 던질 준비가 되었을 때
                CastLine(); // 줄 던지기
            }
			else if (Input.GetKeyDown(fishingKey) && lineCast && !nibble)
			{ // 물고기가 물지 않았을 때 줄 회수
                StopAllCoroutines(); // 대기 중인 코루틴 중지
                lineCast = false; // 줄 던지기 상태 초기화
                
                // 생각 풍선 초기화
                thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
                thoughtBubbles.SetActive(false);
                
            }
			else if (Input.GetKeyDown(fishingKey) && lineCast && nibble)
			{ // 물고기가 물었을 때
                StopAllCoroutines(); // 대기 중인 코루틴 중지
                StartReeling(); // 물고기 끌어당기기 시작
            }
        }
		else
		{ // 물고기와의 싸움 중일 때
            if (Input.GetKey(fishingKey))
			{ // 키를 누를 때
                catchingBarRB.AddForce(Vector2.up * catchingForce * Time.deltaTime, ForceMode2D.Force); // 낚시바에 힘 추가
            }
        }

        // 물고기가 바 안에 있을 때
        if (inTrigger && reelingFish)
		{
            catchPercentage += catchMultiplier * Time.deltaTime; // 잡힐 확률 증가
        }
		else
		{
            catchPercentage -= catchMultiplier * Time.deltaTime; // 잡힐 확률 감소
        }
        
        // 물고기 색 변화
        var fishColor = Color.Lerp(Color.black, Color.white, Map(0, 100, 0, 1, catchPercentage));
        fishBar.GetComponent<Image>().color = fishColor;
        
        // 잡힐 확률 제한
        catchPercentage = Mathf.Clamp(catchPercentage, 0, 100);
        catchProgressBar.value = catchPercentage; // 슬라이더 업데이트
        if (catchPercentage >= 100)
		{ // 물고기 잡힘
            catchPercentage = 0;
            FishCaught(); // 물고기 잡기 호출
        }
    }
    
    // 줄 던지기 호출
    private void CastLine()
	{
        lineCast = true;
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
    
    // 낚시 게임 시작
    private void StartReeling()
	{
        reelingFish = true; // 물고기 끌어당기기 시작
        nibble = false; // 물고기가 물고 있는 상태 초기화
        lineCast = false; // 줄 던지기 상태 초기화
        
        // 잡을 물고기 설정
        currentFishOnLine = FishManager.GetRandomFishWeighted();
        var tempSprite = Resources.Load<Sprite>($"FishSprites/{currentFishOnLine.spriteID}"); // 물고기 스프라이트 로드
		fishBar.GetComponent<Image>().sprite = tempSprite;
		
        // 물고기 바 크기 조정
        var w = Map(0, 32, 0, 100, tempSprite.texture.width);
        var h = Map(0, 32, 0, 100, tempSprite.texture.height);
        fishBar.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
        
        minigameCanvas.SetActive(true); // 미니게임 캔버스 활성화
    }
    
    // 반응 시간 초과 시 줄이 끊어짐
    private IEnumerator LineBreak(float lineBreakTime)
	{
        yield return new WaitForSeconds(lineBreakTime); // 대기 시간
        Debug.Log("물고기 놓침"); // 줄 끊어짐 로그 출력
        
        // 생각 풍선 비활성화
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
        thoughtBubbles.SetActive(false);
        
        lineCast = false; // 줄 던지기 상태 초기화
        nibble = false; // 물고기가 물고 있는 상태 초기화
    }

    // FishingMinigame_FishTrigger 스크립트에서 호출됨
    public void FishInBar()
	{
        inTrigger = true; // 물고기가 바 안에 있음
    }
    
    // FishingMinigame_FishTrigger 스크립트에서 호출됨
    public void FishOutOfBar()
	{
        inTrigger = false; // 물고기가 바 밖에 있음
    }

    // 잡힐 확률이 100이 되었을 때 호출
    public void FishCaught()
	{
        if (currentFishOnLine == null)
		{ // 물고기가 없다면 새로운 물고기 선택
            currentFishOnLine = FishManager.GetRandomFish();
        }
        Debug.Log($"잡은 물고기 : {currentFishOnLine.name}"); // 잡힌 물고기 로그 출력
        reelingFish = false; // 물고기 끌어당기기 중지
        // 생각 풍선 초기화
        thoughtBubbles.SetActive(false);
        thoughtBubbles.GetComponent<Animator>().SetTrigger("Reset");
        minigameCanvas.SetActive(false); // 미니게임 캔버스 비활성화
        catchingbar.transform.localPosition = catchingBarLoc; // 낚시바 위치 초기화
    }
    
    // 값 매핑 함수
    private float Map(float a, float b, float c, float d, float x)
	{
        return (x - a) / (b - a) * (d - c) + c; // 매핑 계산
    }
}
