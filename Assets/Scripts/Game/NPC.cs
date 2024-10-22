using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int[] quest_ids; // NPC가 가진 퀘스트 ID 배열
    public int currentQuestIndex; // 현재 진행 중인 퀘스트 인덱스
    public List<string> generalDialogues; // 일반 대화 리스트
    public GameObject questIndicator; // 퀘스트 표시 이미지
    private QuestManager questManager; // QuestManager에 대한 참조

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>(); // QuestManager 찾기
        UpdateQuestIndicator();
    }

    private void UpdateQuestIndicator()
    {
        bool hasAvailableQuest = false;

        foreach (var questId in quest_ids)
        {
            QuestData quest = questManager.GetQuestById(questId);
            if (quest != null && quest.GetQuestStatus() == QuestStatus.Available)
            {
                // 이전 퀘스트가 완료된 경우만 활성화
                if (quest.requiresQuestId == null || 
                    (questManager.GetQuestById(quest.requiresQuestId.Value)?.GetQuestStatus() == QuestStatus.Completed))
                {
                    hasAvailableQuest = true;
                    break;
                }
            }
        }

        questIndicator.SetActive(hasAvailableQuest);
    }

    public void Interact()
    {
        // NPC의 퀘스트 인디케이터가 활성화된 경우에만 대화 진행
        if (questIndicator.activeSelf)
        {
            // 현재 퀘스트 인덱스가 유효한지 확인
            if (currentQuestIndex < quest_ids.Length)
            {
                int questId = quest_ids[currentQuestIndex];
                QuestData currentQuest = questManager.GetQuestById(questId);

                // 퀘스트가 표시되고 대화가 필요한 경우
                if (currentQuest != null && 
                    currentQuest.GetQuestStatus() == QuestStatus.Available && 
                    currentQuest.requiresDialogue && 
                    currentQuest.questDialogues.Count > 0)
                {
                    // 대화 시작
                    Debug.Log(currentQuest.questDialogues[0]);
                    // 추가 로직: 대화 문장 진행하기
                }
                else if (currentQuest != null && currentQuest.GetQuestStatus() == QuestStatus.Available)
                {
                    // 대화 없이 퀘스트 수락 가능
                    currentQuest.SetQuestStatus(QuestStatus.InProgress);
                    Debug.Log($"퀘스트 수락: {currentQuest.title}");
                }
            }
        }
        else
        {
            // 기본 대화 랜덤 선택
            Debug.Log(generalDialogues[UnityEngine.Random.Range(0, generalDialogues.Count)]);
        }
    }
}
