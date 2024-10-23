using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 추가된 네임스페이스

public class NPC : MonoBehaviour
{
    public int[] quest_ids; // 해당 NPC 할당 퀘스트
    public int currentQuestIndex; // 현재 진행 중인 퀘스트 인덱스
    public List<string> generalDialogues; // 일반 대화 리스트
    public GameObject questIndicator; // 퀘스트 표시 이미지
    public Text[] DialogueText;

    private QuestManager questManager; // QuestManager에 대한 참조

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        if (questManager == null)
        {
            Debug.LogError("QuestManager not found in the scene.");
            return;
        }
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
        if (transform.parent.name == "Cat") DialogueText[0].text = "삼냥이";
        else DialogueText[0].text = "강태곰";

        if (questIndicator.activeSelf && currentQuestIndex < quest_ids.Length)
        {
            // 퀘스트 대화
            int questId = quest_ids[currentQuestIndex];
            QuestData currentQuest = questManager.GetQuestById(questId);
            if (currentQuest != null && currentQuest.questDialogues.Count > 0)
            {
                DialogueText[1].text = currentQuest.questDialogues[0];
                Debug.Log(currentQuest.questDialogues[0]);
            }
        }
        else
        {
            // 일반 대화
            string randomDialogue = generalDialogues[UnityEngine.Random.Range(0, generalDialogues.Count)];
            DialogueText[1].text = randomDialogue;
            Debug.Log(randomDialogue);
        }
    }
}
