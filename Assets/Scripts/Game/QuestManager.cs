using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    Available,
    InProgress,
    Completed
}

[System.Serializable]
public class QuestData
{
    public int id;
    public string title;
    public string description;
    public bool npcDialogue;
    public int requiresQuestId;
    public List<string> questDialogues;
    public QuestStatus status;

    public QuestData(int id, string title, string description, bool npcDialogue, int requiresQuestId, List<string> questDialogues = null)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.npcDialogue = npcDialogue;
        this.requiresQuestId = requiresQuestId;
        this.questDialogues = questDialogues ?? new List<string>();
        this.status = QuestStatus.Available;
    }

    public QuestStatus GetQuestStatus() => status;

    public void SetQuestStatus(QuestStatus newStatus) => status = newStatus;
}

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private List<QuestData> questList = new List<QuestData>();
    private Dictionary<int, QuestData> questData = new Dictionary<int, QuestData>();

    void Start()
    {
        LoadQuestData();
        UpdateAvailableQuests();
    }

    public bool QuestExists(int questId)
    {
        return questData.ContainsKey(questId);
    }

    public bool IsQuestAvailable(int questId)
    {
        return questData[questId].GetQuestStatus() == QuestStatus.Available;
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

            if (data.Length < 5)
            {
                Debug.LogError("Invalid quest data format: " + line);
                continue;
            }

            int id = int.Parse(data[0]);
            string title = data[1];
            string description = data[2];
            bool npcDialogue = bool.Parse(data[3]);
            int requiresQuestId = string.IsNullOrWhiteSpace(data[4]) ? 0 : int.Parse(data[4]);

            List<string> questDialogues = new List<string>();
            for (int i = 5; i < data.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(data[i]))
                {
                    questDialogues.Add(data[i].Trim());
                }
            }

            var questDataEntry = new QuestData(id, title, description, npcDialogue, requiresQuestId, questDialogues);
            questList.Add(questDataEntry);
            questData[id] = questDataEntry;
        }
    }

    public QuestData GetQuestById(int questId)
    {
        if (questData.ContainsKey(questId))
        {
            return questData[questId];
        }
        Debug.LogError($"Quest ID not found: {questId}");
        return null;
    }

    public void UpdateQuestStatus(int questId, QuestStatus newStatus)
    {
        if (questData.ContainsKey(questId))
        {
            questData[questId].SetQuestStatus(newStatus);
            Debug.Log($"Quest {questId} status updated to {newStatus}");
        }
        else
        {
            Debug.LogError($"Quest ID not found: {questId}");
        }
    }

    public void UpdateAvailableQuests()
    {
        foreach (var quest in questData.Values)
        {
            if (quest.GetQuestStatus() == QuestStatus.Available &&
                (quest.requiresQuestId == 0 ||
                 (questData.ContainsKey(quest.requiresQuestId) &&
                  questData[quest.requiresQuestId].GetQuestStatus() == QuestStatus.Completed)))
            {
                if (!quest.npcDialogue)
                {
                    UpdateQuestStatus(quest.id, QuestStatus.InProgress);
                    Debug.Log($"{quest.title} 퀘스트 자동 진행 중");
                }
            }
        }
    }
}
