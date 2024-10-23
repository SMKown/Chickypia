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
    public int id;
    public string title;
    public string description;
    public bool requiresDialogue;
    public int? requiresQuestId;
    public List<string> questDialogues;
    public QuestStatus status;

    public QuestData(int id, string title, string description, bool requiresDialogue, int? requiresQuestId, List<string> questDialogues = null)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.requiresDialogue = requiresDialogue;
        this.requiresQuestId = requiresQuestId;
        this.questDialogues = questDialogues ?? new List<string>();
        this.status = QuestStatus.Available;
    }

    public QuestStatus GetQuestStatus() => status;

    public void SetQuestStatus(QuestStatus newStatus) => status = newStatus;
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
            if (quest.GetQuestStatus() == QuestStatus.Available &&
                (quest.requiresQuestId == null ||
                 (questData.ContainsKey(quest.requiresQuestId.Value) &&
                  questData[quest.requiresQuestId.Value].GetQuestStatus() == QuestStatus.Completed)))
            {
                if (!quest.requiresDialogue)
                {
                    UpdateQuestStatus(quest.id, QuestStatus.InProgress);
                    Debug.Log($"{quest.title} 퀘스트 자동 진행 중");
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
                (quest.requiresQuestId == null ||
                 (questData.ContainsKey(quest.requiresQuestId.Value) &&
                  questData[quest.requiresQuestId.Value].GetQuestStatus() == QuestStatus.Completed)))
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
