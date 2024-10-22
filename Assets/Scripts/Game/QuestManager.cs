using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    Available,
    InProgress,
    Completed
}

public class QuestData
{
    public int id; // 퀘스트 ID
    public string title; // 퀘스트 제목
    public string description; // 퀘스트 설명
    public bool requiresDialogue; // NPC와의 대화가 필요한지 여부
    public int? requiresQuestId; // 이전 퀘스트가 완료되어야 하는 경우의 ID
    public List<string> questDialogues; // 퀘스트 대화 (필요한 경우만 사용)
    public QuestStatus status; // 퀘스트 상태

    public QuestData(int id, string title, string description, bool requiresDialogue, int? requiresQuestId, List<string> questDialogues = null)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.requiresDialogue = requiresDialogue;
        this.requiresQuestId = requiresQuestId;
        this.questDialogues = questDialogues ?? new List<string>();
        this.status = QuestStatus.Available; // 초기 상태는 Available
    }

    public QuestStatus GetQuestStatus()
    {
        return status;
    }

    public void SetQuestStatus(QuestStatus newStatus)
    {
        status = newStatus;
    }
}

public class QuestManager : MonoBehaviour
{
    public Dictionary<int, QuestData> questData;

    void Start()
    {
        questData = new Dictionary<int, QuestData>();
        LoadQuestData();
        UpdateAvailableQuests();
    }

    void Update()
    {
        UpdateAvailableQuests();
    }

    void LoadQuestData()
    {
        var textAsset = Resources.Load<TextAsset>("CSV/Quest");
        var lines = textAsset.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] data = line.Split(',');

            int id = int.Parse(data[0]);
            string title = data[1];
            string description = data[2];
            bool requiresDialogue = bool.Parse(data[3]);
            int? requiresQuestId = string.IsNullOrWhiteSpace(data[4]) ? (int?)null : int.Parse(data[4]);

            List<string> questDialogues = new List<string>();

            for (int i = 5; i < data.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(data[i]))
                {
                    questDialogues.Add(data[i].Trim());
                }
            }

            questData[id] = new QuestData(id, title, description, requiresDialogue, requiresQuestId, questDialogues);
        }
    }

    public void UpdateQuestStatus(int questId, QuestStatus newStatus)
    {
        if (questData.ContainsKey(questId))
        {
            questData[questId].SetQuestStatus(newStatus);
        }
    }

    public void UpdateAvailableQuests()
    {
        foreach (var quest in questData.Values)
        {
            // 이전 퀘스트가 완료되었고 현재 퀘스트가 "Available"인 경우
            if (quest.GetQuestStatus() == QuestStatus.Available && 
                (quest.requiresQuestId == null || questData[quest.requiresQuestId.Value].GetQuestStatus() == QuestStatus.Completed))
            {
                // NPC와 대화가 필요 없는 경우 자동으로 진행 중
                if (!quest.requiresDialogue)
                {
                    UpdateQuestStatus(quest.id, QuestStatus.InProgress);
                    // 퀘스트 창 업데이트 로직 추가 (예: UI에 퀘스트 추가)
                    Debug.Log($"{quest.title} 퀘스트가 자동으로 진행 중입니다.");
                }
            }
        }
    }

    public List<QuestData> GetAvailableQuests()
    {
        List<QuestData> availableQuests = new List<QuestData>();

        foreach (var quest in questData.Values)
        {
            if (quest.GetQuestStatus() == QuestStatus.Available &&
                (quest.requiresQuestId == null || questData[quest.requiresQuestId.Value].GetQuestStatus() == QuestStatus.Completed))
            {
                availableQuests.Add(quest);
            }
        }

        return availableQuests;
    }

    public QuestData GetQuestById(int quest_id)
    {
        if (questData.ContainsKey(quest_id))
        {
            return questData[quest_id];
        }
        else
        {
            Debug.LogError("Quest ID not found: " + quest_id);
            return null;
        }
    }
}
