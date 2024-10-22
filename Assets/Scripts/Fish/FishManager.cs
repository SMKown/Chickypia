using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fish
{
    public string name;
    public Sprite sprite;

    public Fish(string fishName)
    {
        name = fishName;  
        sprite = Resources.Load<Sprite>($"Sprites/{fishName}");
    }
}

public static class FishManager
{
    public static List<Fish> allFish = new List<Fish>();

    [RuntimeInitializeOnLoadMethod]
    public static void InitFishManager()
    {
        var textAsset = Resources.Load<TextAsset>("CSV/Fish");
        var lines = textAsset.text.Split('\n');
        
        foreach (var line in lines)
        {
            var fishName = line.Trim();
            if (!string.IsNullOrEmpty(fishName) && fishName != "Fish Name")
            {
                var newFish = new Fish(fishName);
                allFish.Add(newFish);
            }
        }
    }

    public static Fish GetRandomFish()
    {
        return allFish[Random.Range(0, allFish.Count)];
    }
}
