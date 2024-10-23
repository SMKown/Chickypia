using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public int[] quest_ids; // NPC 할당 퀘스트
    public List<string> generalDialogues; // 일반 대화 배열
    public GameObject questMark;
    public Text[] DialogueText;
    public int dialogueIndex = 0;
    
    private QuestManager questManager;
    private bool isInteracting = false;

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
        if (!isInteracting)
        {
            UpdateQuestMark();
        }
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
        isInteracting = true; // 대화 시작
        QuestData activeQuest = GetActiveQuest();
        questMark.SetActive(false); // 대화 중 퀘스트 마크 숨기기

        if (activeQuest != null)
            DisplayDialogue(activeQuest.questDialogues, activeQuest);
        else
            DisplayDialogue(generalDialogues);
    }

    private void CloseDialogue()
    {
        isInteracting = false;
        UIInteraction.Instance.ImageOff(UIInteraction.Instance.dialog);
        dialogueIndex = 0;
    }

    private void DisplayDialogue(List<string> dialogues, QuestData quest = null)
    {
        if (dialogueIndex < dialogues.Count)
        {
            DialogueText[1].text = dialogues[dialogueIndex];
            dialogueIndex++;
        }
        else
        {
            if (quest != null)
            {
                quest.SetQuestStatus(QuestStatus.InProgress);
                quest.UpdateItemCount(0);
            }
            CloseDialogue(); // 대화 종료
        }
    }

    private QuestData GetActiveQuest()
    {
        foreach (var id in quest_ids)
        {
            if (questManager.QuestExists(id) && questManager.IsQuestAvailable(id))
            {
                return questManager.GetQuestData(id);
            }
        }
        return null;
    }
}
