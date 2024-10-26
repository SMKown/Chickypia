using System.Collections.Generic;
using TMPro;
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

    public TMP_Text QuestTitleText;
    public TMP_Text QuestDescriptionText;
    public TMP_Text CountText;
    public TMP_Text MainquestCount;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        UpdateQuest();
    }

    private void Update()
    {
        if (!PlayerInfo.Instance.interacting)
        {
            UpdateQuest();
        }
    }

    private void UpdateQuest()
    {
        bool hasAvailableQuest = false;
        int completedQuestCount = 0;

        foreach (int questId in quest_ids)
        {
            QuestData quest = questManager.GetQuestData(questId);
            if (quest != null)
            {
                if (quest.GetQuestStatus() == QuestStatus.Completed)
                {
                    completedQuestCount++; // 완료된 퀘스트 수
                }
                else if (quest.GetQuestStatus() == QuestStatus.Available &&
                         (quest.requiresQuestId == 0 || questManager.GetQuestData(quest.requiresQuestId)?.GetQuestStatus() == QuestStatus.Completed))
                {
                    hasAvailableQuest = true; // 수락 가능한 퀘스트 존재
                }
            }
        }

        if (MainquestCount != null)
            MainquestCount.text = $"[{completedQuestCount}/{quest_ids.Length}]";
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
                quest.OnItemCountUpdated += () => DisplayQuestInfo(quest); // 이벤트 연결
                DisplayQuestInfo(quest);
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

    private void DisplayQuestInfo(QuestData quest)
    {
        QuestTitleText.text = quest.title;
        QuestDescriptionText.text = $"[{quest.explanation}]";
        CountText.text = quest.itemCount != quest.itemCountRequired ? $"[{quest.itemCount}/{quest.itemCountRequired}]" : "퀘스트 완료!";
    }
}
