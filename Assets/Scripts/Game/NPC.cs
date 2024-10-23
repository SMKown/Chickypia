using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public int[] quest_ids; // 해당 NPC 할당 퀘스트
    public List<string> generalDialogues; // 일반 대화 리스트
    public GameObject questMark;
    public Text[] DialogueText;

    private QuestManager questManager;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        if (questManager == null)
        {
            Debug.LogError("QuestManager not found in the scene.");
            return;
        }
        UpdateQuestMark();
    }

    private void Update()
    {
        UpdateQuestMark();
    }

    private void UpdateQuestMark()
    {
        bool hasAvailableQuest = false;

        foreach (int questId in quest_ids)
        {
            QuestData quest = questManager.GetQuestData(questId);

            if (quest != null && quest.GetQuestStatus() == QuestStatus.Available)
            {
                if (quest.requiresQuestId == 0 || questManager.GetQuestData(quest.requiresQuestId)?.GetQuestStatus() == QuestStatus.Completed)
                {
                    hasAvailableQuest = true;
                    break;
                }
            }
        }

        questMark.SetActive(hasAvailableQuest);
    }

    public void Interact()
    {
        if (transform.parent.name == "Cat")
            DialogueText[0].text = "삼냥이";
        else
            DialogueText[0].text = "강태곰";

        QuestData activeQuest = GetActiveQuest();

        if (activeQuest != null)
        {
            // 대화 내용 설정 및 퀘스트 상태 변경
            DialogueText[1].text = activeQuest.questDialogues[0];
            activeQuest.SetQuestStatus(QuestStatus.InProgress);
            questMark.SetActive(false); // 대화 후 인디케이터 비활성화
        }
        else
        {
            // 일반 대화 처리
            DialogueText[1].text = generalDialogues[Random.Range(0, generalDialogues.Count)];
        }
    }

    private QuestData GetActiveQuest()
    {
        foreach (var id in quest_ids)
        {
            // 퀘스트 존재 여부 & 상태 확인
            if (questManager.QuestExists(id) && questManager.IsQuestAvailable(id))
            {
                return questManager.GetQuestData(id);
            }
        }
        return null;
    }
}
