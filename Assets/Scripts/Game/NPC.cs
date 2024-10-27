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
    public List<string> generalDialogues; // 일반 대화
    public GameObject questMark; // 퀘스트 마크 오브젝트
    public Text[] DialogueText; // 대화 텍스트 배열

    private QuestManager questManager;
    private PlayerMovement playerMovement;
    private int dialogueIndex = 0;
    private string npcName;
    private string questCategory;
    private int[] quest_ids; // NPC 할당 퀘스트

    public Transform canvas; // 퀘스트 박스를 넣을 캔버스의 Transform
    public GameObject QuestBoxPrefab; // QuestBox 프리팹
    private GameObject questBoxInstance; // 인스턴스화된 QuestBox
    private TMP_Text[] QuestTxt; // 퀘스트 텍스트 배열

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        // NPC 유형에 따라 quest_ids 및 npcName과 questCategory 설정
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
                    QuestTxt[4].text = "퀘스트 완료!";
                }
                else if (quest.GetQuestStatus() == QuestStatus.Available &&
                         (quest.requiresQuestId == 0 || questManager.GetQuestData(quest.requiresQuestId)?.GetQuestStatus() == QuestStatus.Completed))
                {
                    hasAvailableQuest = true;

                    if (questBoxInstance == null)
                        CreateQuestBox();
                    if (questId == quest_ids[0])
                        DisplayQuestInfo(quest);
                }
                else if(quest.GetQuestStatus() == QuestStatus.InProgress)
                {
                    DisplayQuestInfo(quest);
                }
            }
        }

        if (QuestTxt != null)
        {
            QuestTxt[0].text = questCategory;
            QuestTxt[1].text = $"[{completedQuestCount}/{quest_ids.Length}]";
        }

        // 퀘스트 마크 표시 여부 결정
        questMark.SetActive(hasAvailableQuest);

        // 퀘스트 UI 표시 여부 결정
        if (hasAvailableQuest && questBoxInstance != null)
        {
            questBoxInstance.SetActive(true);
        }
        else if (questBoxInstance != null && completedQuestCount == quest_ids.Length)
        {
            Destroy(questBoxInstance); // 모든 퀘스트가 완료된 경우 UI 제거
            questBoxInstance = null; // 인스턴스 null로 초기화
        }
    }

    public void Interact()
    {
        QuestData activeQuest = GetActiveQuest();
        questMark.SetActive(false); // 대화 시작 시 퀘스트 마크 숨김

        if (activeQuest != null)
        {
            // 퀘스트 대화
            DisplayDialogue(activeQuest.questDialogues, activeQuest);
        }
        else
        {
            // 일반 대화
            DisplayDialogue(generalDialogues);
        }
    }

    private void CreateQuestBox()
    {
        questBoxInstance = Instantiate(QuestBoxPrefab, canvas);
        QuestTxt = questBoxInstance.GetComponentsInChildren<TMP_Text>();
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
        DialogueText[0].text = npcName;

        if (dialogueIndex < dialogues.Count)
        {
            DialogueText[1].text = dialogues[dialogueIndex];
            dialogueIndex++;
        }
        else
        {
            if (quest != null)
            {
                // 대화가 끝난 후 퀘스트 상태 변경
                quest.SetQuestStatus(QuestStatus.InProgress);
                quest.OnItemCountUpdated += () => DisplayQuestInfo(quest);

                quest.UpdateItemCount(0); // 초기 업데이트

                // 퀘스트 완료 체크
                if (quest.itemCount >= quest.itemCountRequired)
                {
                    quest.SetQuestStatus(QuestStatus.Completed);
                    questManager.SaveQuestProgress(); // 진행 상황 저장
                }
            }
            UpdateQuest();
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

        QuestTxt[2].text = quest.title; // 퀘스트 제목
        QuestTxt[3].text = $"[{quest.explanation}]"; // 퀘스트 설명
        QuestTxt[4].text = quest.itemId == 100 ? $"{npcName}에게 가볼까?" : $"[{quest.itemCount}/{quest.itemCountRequired}]";
    }
}
