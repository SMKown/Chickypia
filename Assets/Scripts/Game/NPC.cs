using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public int[] quest_ids; // NPC 할당 퀘스트
    public List<string> generalDialogues; // 일반 대화
    public GameObject questMark;
    public Text[] DialogueText;
    
    private QuestManager questManager;
    private PlayerMovement playerMovement;
    private int dialogueIndex = 0;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        UpdateQuestMark();
    }

    private void Update()
    {
        if (!PlayerInfo.Instance.interacting)
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
        QuestData activeQuest = GetActiveQuest();
        questMark.SetActive(false);

        if (activeQuest != null)
            DisplayDialogue(activeQuest.questDialogues, activeQuest);
        else
            DisplayDialogue(generalDialogues);
    }

    public void CloseDialogue()
    {
        UIInteraction.Instance.ImageOff(UIInteraction.Instance.dialog);
        dialogueIndex = 0;

        if (PlayerInfo.Instance.interacting)
        {
            PlayerInfo.Instance.interacting = false;
            playerMovement.ResetCamera();
            UIInteraction.Instance.interactableObj = null;
        }
    }

    private void DisplayDialogue(List<string> dialogues, QuestData quest = null)
    {
        DialogueText[0].text = transform.parent.name == "Cat" ? "삼냥이" : "강태곰";

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
            CloseDialogue();
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
