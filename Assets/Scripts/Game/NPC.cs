using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public int[] quest_ids; // 해당 NPC 할당 퀘스트
    public List<string> generalDialogues; // 일반 대화 리스트
    public GameObject questIndicator;
    public Text[] DialogueText;

    private QuestManager questManager;
    private QuestData questData;

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

        foreach (var id in quest_ids)
        {
            if (questManager.QuestExists(id) && questManager.IsQuestAvailable(id))
            {
                hasAvailableQuest = true;
                break;
            }
        }

        questIndicator.SetActive(hasAvailableQuest);
    }

    public void Interact()
    {
        if (transform.parent.name == "Cat") 
            DialogueText[0].text = "삼냥이";
        else
            DialogueText[0].text = "강태곰";

        if (questData != null && questData.npcDialogue)
        {
            DialogueText[1].text = questData.questDialogues[0];
            questData.SetQuestStatus(QuestStatus.InProgress);
        }
        else
        {
            // 일반 대화 처리
            DialogueText[1].text = generalDialogues[Random.Range(0, generalDialogues.Count)];
        }
    }

    private void EndDialogue()
    {
        questData.SetQuestStatus(QuestStatus.InProgress);
        questIndicator.SetActive(false);
        // 대화 UI 끄기 로직
    }
}
