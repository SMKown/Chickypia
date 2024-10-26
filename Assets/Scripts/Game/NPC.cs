using System.Collections;
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

    public Transform canvas; // 퀘스트 박스를 넣을 캔버스의 Transform
    public GameObject QuestBoxPrefab; // QuestBox 프리팹
    private GameObject questBoxInstance; // 인스턴스화된 QuestBox
    private TMP_Text[] QuestTxt; // 퀘스트 텍스트 배열

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (questManager == null || playerMovement == null)
        {
            Debug.LogError("QuestManager or PlayerMovement not found!");
            return;
        }

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
                    completedQuestCount++;
                }
                else if (quest.GetQuestStatus() == QuestStatus.Available &&
                         (quest.requiresQuestId == 0 || questManager.GetQuestData(quest.requiresQuestId)?.GetQuestStatus() == QuestStatus.Completed))
                {
                    hasAvailableQuest = true;

                    if (questBoxInstance == null)
                    {
                        CreateQuestBox();
                        DisplayQuestInfo(quest); // 퀘스트 정보 업데이트
                    }
                }
            }
        }

        if (QuestTxt != null)
        {
            QuestTxt[0].text = transform.parent.name == "Cat" ? "치키별 적응기" : "치키별의 강태공";
            QuestTxt[1].text = $"[{completedQuestCount}/{quest_ids.Length}]";
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

    private void CreateQuestBox()
    {
        questBoxInstance = Instantiate(QuestBoxPrefab, canvas);
        QuestTxt = questBoxInstance.GetComponentsInChildren<TMP_Text>();

        if (QuestTxt == null || QuestTxt.Length == 0)
        {
            return;
        }

        UpdateQuest();
    }

    private void CloseDialogue()
    {
        UIInteraction.Instance.ImageOff(UIInteraction.Instance.dialog);
        PlayerInfo.Instance.canInteract = false;
        dialogueIndex = 0;

        PlayerInfo.Instance.interacting = false;
        playerMovement.ResetCamera();
        StartCoroutine(EnableInteractionDelay(1.5F));
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

                quest.UpdateItemCount(0);

                if (quest.itemCount >= quest.itemCountRequired)
                {
                    quest.SetQuestStatus(QuestStatus.Completed);
                }

                DisplayQuestInfo(quest); // 상태 업데이트 후에 호출
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
        if (QuestTxt == null) return;

        QuestTxt[2].text = quest.title;
        QuestTxt[3].text = $"[{quest.explanation}]";

        Debug.Log($"Quest Status: {quest.status}"); // 디버그 로그 추가

        if (quest.itemId == 100)
        {
            string name = transform.parent.name == "Cat" ? "삼냥이" : "강태곰";
            QuestTxt[4].text = quest.status != QuestStatus.Completed ? $"{name}에게 가볼까?" : "퀘스트 완료!";
        }
        else
        {
            QuestTxt[4].text = quest.status != QuestStatus.Completed ? $"[{quest.itemCount}/{quest.itemCountRequired}]" : "퀘스트 완료!";
        }
    }
}
