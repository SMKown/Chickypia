using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum NPCType
{
    Cat,
    Bear
}

public class NPC : MonoBehaviour
{
    public NPCType npcType;
    public List<string> generalDialogues;
    public GameObject questMark;
    public GameObject Dialog;

    private Text[] DialogueText;

    private PlayerMovement playerMovement;
    private int dialogueIndex = 0;
    private string npcName;
    private string questCategory;
    private int[] quest_ids;

    public Transform canvas;
    public GameObject QuestBoxPrefab;
    private GameObject questBoxInstance;
    private GameObject completedImage;
    private TMP_Text[] QuestTxt;

    private Animation animation;
    private bool completed = false;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (Dialog != null)
            DialogueText = Dialog.GetComponentsInChildren<Text>();
        InitializeNPC();
        UpdateQuestUI();

    }

    private void Update()
    {
        if (!PlayerInfo.Instance.interacting && !completed)
        {
            UpdateQuestUI();
        }
    }

    private void InitializeNPC()
    {
        if (npcType == NPCType.Cat)
        {
            quest_ids = new int[] { 1, 2, 3, 4, 5 };
            npcName = "삼냥이";
            questCategory = "치키별 적응기";
        }
        else if (npcType == NPCType.Bear)
        {
            quest_ids = new int[] { 6, 7 };
            npcName = "강태곰";
            questCategory = "치키별의 강태공";
        }
    }

    private void UpdateQuestUI()
    {
        bool hasAvailableQuest = false;
        int completedQuestCount = 0;

        foreach (int questId in quest_ids)
        {
            QuestData quest = QuestManager.Instance.GetQuestData(questId);
            QuestData requireQuest = QuestManager.Instance.GetQuestData(quest.requiresQuestId);

            if (quest != null)
            {
                switch (quest.GetQuestStatus())
                {
                    case QuestStatus.Available when questId == quest_ids[0] && (requireQuest == null || requireQuest.GetQuestStatus() == QuestStatus.Completed):
                        hasAvailableQuest = true;
                        DisplayQuestInfo(quest);
                        break;

                    case QuestStatus.Available when requireQuest == null || requireQuest.GetQuestStatus() == QuestStatus.Completed:
                        hasAvailableQuest = true;
                        break;

                    case QuestStatus.Completed:
                        DisplayQuestInfo(quest);
                        completedQuestCount++;
                        break;

                    case QuestStatus.InProgress:
                        DisplayQuestInfo(quest);
                        break;
                }
            }
        }

        UpdateQuestCategoryText(completedQuestCount);
        questMark.SetActive(hasAvailableQuest);

        if (questBoxInstance != null)
        {
            completed = completedQuestCount >= quest_ids.Length;

            if (completed)
            {
                animation.Play("QuestClear");
                StartCoroutine(DestroyQuestBoxAfterAnimation(1.2F));
            }
        }
    }

    private IEnumerator DestroyQuestBoxAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(questBoxInstance);
    }

    public void Interact()
    {
        QuestData activeQuest = GetActiveQuest();
        questMark.SetActive(false);

        if (activeQuest != null)
        {
            DisplayDialogue(activeQuest.questDialogues, activeQuest);
        }
        else
        {
            DisplayDialogue(generalDialogues);
        }
    }

    private void CreateQuestBox()
    {
        if (questBoxInstance == null)
        {
            questBoxInstance = Instantiate(QuestBoxPrefab, canvas);
            QuestTxt = questBoxInstance.GetComponentsInChildren<TMP_Text>();
            completedImage = questBoxInstance.transform.GetChild(1).gameObject;
            animation = questBoxInstance.GetComponent<Animation>();
        }
    }

    private IEnumerator EnableInteractionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (UIInteraction.Instance.interactableObj != null)
            UIInteraction.Instance.ImageOn(UIInteraction.Instance.dialog, transform);
        PlayerInfo.Instance.canInteract = true;
    }

    private void DisplayDialogue(List<string> dialogues, QuestData quest = null)
    {
        DialogueText[0].text = npcName;

        if (npcType == NPCType.Bear && quest == null)
        {
            DialogueText[1].text = dialogues[dialogueIndex];
            Dialog.transform.GetChild(2).gameObject.SetActive(true);
            return;
        }

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
                quest.OnItemCountUpdated += () => DisplayQuestInfo(quest);
                quest.UpdateItemCount(0);
                animation.Play("QuestUpdate");

                if (quest.itemCount >= quest.itemCountRequired)
                {
                    quest.SetQuestStatus(QuestStatus.Completed);
                    QuestManager.Instance.SaveQuestProgress();
                }
            }
            UpdateQuestUI();
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        UIInteraction.Instance.ImageOff(UIInteraction.Instance.dialog);
        Dialog.transform.GetChild(2).gameObject.SetActive(false);
        PlayerInfo.Instance.canInteract = false;
        dialogueIndex = 0;
        PlayerInfo.Instance.interacting = false;
        playerMovement.ResetCamera();
        StartCoroutine(EnableInteractionDelay(1.5F));
    }

    private QuestData GetActiveQuest()
    {
        foreach (var id in quest_ids)
        {
            if (QuestManager.Instance.QuestExists(id) && QuestManager.Instance.IsQuestAvailable(id))
            {
                return QuestManager.Instance.GetQuestData(id);
            }
        }
        return null;
    }

    private void DisplayQuestInfo(QuestData quest)
    {
        CreateQuestBox();
        if (QuestTxt != null)
        {
            QuestTxt[2].text = quest.title;
            QuestTxt[3].text = $"{quest.explanation}";

            if (quest.status == QuestStatus.Completed)
            {
                completedImage.SetActive(true);
                QuestTxt[4].text = "완료";
            }
            else
            {
                completedImage.SetActive(false);
                QuestTxt[4].text = quest.itemId == 100 ? $"0/1" : $"{quest.itemCount}/{quest.itemCountRequired}";
            }
        }
    }

    private void UpdateQuestCategoryText(int completedQuestCount)
    {
        if (QuestTxt != null)
        {
            QuestTxt[0].text = questCategory;
            QuestTxt[1].text = $"{completedQuestCount}/{quest_ids.Length}";
        }
    }
}