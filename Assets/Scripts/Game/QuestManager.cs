using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    Available, // 퀘스트 수락 전
    InProgress, // 퀘스트 진행 중
    Completed // 퀘스트 완료
}

[System.Serializable]
public class QuestData
{
    public int id; // 번호
    public string title; // 제목
    public bool npc; // NPC 대화 유무
    public int requiresQuestId; // 진행하기 위한 전 단계 퀘스트 번호
    public int itemId; // 수집해야 할 아이템의 ID
    public int itemCountRequired; // 필요한 수집 개수
    public List<string> questDialogues; // 퀘스트 대화

    public QuestStatus status; // 퀘스트 상태
    public int itemCount; // 현재 수집한 개수

    public QuestData(int id, string title, bool npc, int requiresQuestId, int itemId = 0, int itemCountRequired = 0, List<string> questDialogues = null)
    {
        this.id = id;
        this.title = title;
        this.npc = npc;
        this.requiresQuestId = requiresQuestId;
        this.itemId = itemId;
        this.itemCountRequired = itemCountRequired;
        this.questDialogues = questDialogues ?? new List<string>();
        this.status = QuestStatus.Available;
        this.itemCount = 0;
    }

    public void UpdateItemCount(int amount) // 필요 아이템 수집
    {
        if (status == QuestStatus.InProgress) // 진행 중인 퀘스트만 수집 체크
        {
            itemCount += amount;
            if (itemCount >= itemCountRequired)
            {
                SetQuestStatus(QuestStatus.Completed);
            }
        }
    }

    public QuestStatus GetQuestStatus() => status;
    public void SetQuestStatus(QuestStatus newStatus) => status = newStatus;
}

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    public List<QuestData> questList = new List<QuestData>();
    private Dictionary<int, QuestData> questData = new Dictionary<int, QuestData>();

    void Start()
    {
        LoadQuestData();
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
            return quest.GetQuestStatus() == QuestStatus.Available &&
                   (quest.requiresQuestId == 0 ||
                    (questData.TryGetValue(quest.requiresQuestId, out var prerequisiteQuest) &&
                     prerequisiteQuest.GetQuestStatus() == QuestStatus.Completed));
        }
        return false;
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
            bool npcDialogue = bool.Parse(data[2]);
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

            var questDataEntry = new QuestData(id, title, npcDialogue, requiresQuestId, itemToCollectId, itemCountRequired, questDialogues);
            questList.Add(questDataEntry);
            questData[id] = questDataEntry;
        }
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
}
