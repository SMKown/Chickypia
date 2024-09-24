using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fish
{
    public string name;
    public string sprite;

    public Fish(string a, string b)
    {
        name = a;
        sprite = b;
    }
}

public static class FishManager
{
    public static List<Fish> allFish = new List<Fish>();

    [RuntimeInitializeOnLoadMethod]
    public static void InitFishManager()
    {
        var textAsset = Resources.Load<TextAsset>("CSV/Fish");
        
        var splitData = textAsset.text.Split('\n');
        foreach (var line in splitData)
        {
            var lineData = line.Split(',');
            if (lineData[0] != "Fish Name")
            {
                var newFish = new Fish(lineData[0], lineData[1]);
                allFish.Add(newFish);
            }
        }
    }

    public static Fish GetRandomFish()
    {
        return allFish[Random.Range(0, allFish.Count)];
    }
}
