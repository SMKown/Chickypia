using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    Available,
    InProgress,
    Completed
}

[Serializable]
public class QuestData
{
    public int id;
    public string title;
    public string explanation;
    public int requiresQuestId;
    public int itemId;
    public int itemCountRequired;
    public List<string> questDialogues;

    public QuestStatus status;
    public int itemCount;

    public QuestData(int id, string title, string explanation, int requiresQuestId, int itemId = 0, int itemCountRequired = 0, List<string> questDialogues = null)
    {
        this.id = id;
        this.title = title;
        this.explanation = explanation;
        this.requiresQuestId = requiresQuestId;
        this.itemId = itemId;
        this.itemCountRequired = itemCountRequired;
        this.questDialogues = questDialogues ?? new List<string>();
        this.status = QuestStatus.Available;
        this.itemCount = 0;
    }

    public Action OnItemCountUpdated;

    public void UpdateItemCount(int amount)
    {
        if (status == QuestStatus.InProgress)
        {
            itemCount += amount;
            OnItemCountUpdated?.Invoke();
            QuestManager.Instance.SaveQuestProgress();

            if (itemCount >= itemCountRequired)
            {
                SetQuestStatus(QuestStatus.Completed);
                QuestManager.Instance.SaveQuestProgress();
            }
        }
    }

    public QuestStatus GetQuestStatus() => status;

    public void SetQuestStatus(QuestStatus newStatus) => status = newStatus;

    public bool IsComplete()
    {
        return status == QuestStatus.Completed;
    }

    public bool CanStart()
    {
        return status == QuestStatus.Available;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField]
    public List<QuestData> questList = new List<QuestData>();
    private Dictionary<int, QuestData> questData = new Dictionary<int, QuestData>();

    private string filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "questData.json");
        LoadQuestData();
        LoadQuestProgress();
    }

    public QuestData GetQuestData(int questId)
    {
        return questData.ContainsKey(questId) ? questData[questId] : null;
    }

    public bool QuestExists(int questId)
    {
        return questData.ContainsKey(questId);
    }

    public bool IsQuestAvailable(int questId)
    {
        if (questData.TryGetValue(questId, out var quest))
        {
            return quest.CanStart() &&
                   (quest.requiresQuestId == 0 ||
                    (questData.TryGetValue(quest.requiresQuestId, out var prerequisiteQuest) &&
                     prerequisiteQuest.IsComplete()));
        }
        return false;
    }

    public void SaveQuestProgress()
    {
        string jsonData = JsonUtility.ToJson(new QuestListWrapper { quests = questList }, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("퀘스트 저장 완료");
    }

    public void LoadQuestProgress()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            QuestListWrapper questWrapper = JsonUtility.FromJson<QuestListWrapper>(jsonData);

            foreach (var savedQuest in questWrapper.quests)
            {
                if (questData.TryGetValue(savedQuest.id, out var quest))
                {
                    quest.status = savedQuest.status;
                    quest.itemCount = savedQuest.itemCount;
                }
            }
            Debug.Log("퀘스트 진행 불러오기 완료");
        }
        else
        {
            Debug.Log("저장된 퀘스트 데이터가 없습니다");
        }
    }

    public void ResetQuestProgress()
    {
        foreach (var quest in questList)
        {
            quest.SetQuestStatus(QuestStatus.Available);
            quest.itemCount = 0;
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        SaveQuestProgress();

        Debug.Log("퀘스트 진행 초기화 완료");
    }

    private void LoadQuestData()
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
            string explanation = data[2];
            int requiresQuestId = string.IsNullOrWhiteSpace(data[3]) ? 0 : int.Parse(data[3]);
            int itemToCollectId = int.Parse(data[4]);
            int itemCountRequired = int.Parse(data[5]);

            List<string> questDialogues = new List<string>();
            for (int i = 6; i < data.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(data[i]))
                {
                    questDialogues.Add(data[i].Trim());
                }
            }

            var questDataEntry = new QuestData(id, title, explanation, requiresQuestId, itemToCollectId, itemCountRequired, questDialogues);
            questList.Add(questDataEntry);
            questData[id] = questDataEntry;
        }
    }

    public List<QuestData> GetActiveQuests()
    {
        return questList.FindAll(quest => quest.status == QuestStatus.InProgress);
    }

    public void StartQuest(int questId)
    {
        if (IsQuestAvailable(questId) && questData.TryGetValue(questId, out var quest))
        {
            quest.SetQuestStatus(QuestStatus.InProgress);
            SaveQuestProgress();
            Debug.Log($"퀘스트 시작: {quest.title}");
        }
    }

    public void CompleteQuest(int questId)
    {
        if (questData.TryGetValue(questId, out var quest) && quest.status == QuestStatus.InProgress)
        {
            quest.SetQuestStatus(QuestStatus.Completed);
            SaveQuestProgress();
            Debug.Log($"퀘스트 완료: {quest.title}");
        }
    }
}

[Serializable]
public class QuestListWrapper
{
    public List<QuestData> quests;
}
