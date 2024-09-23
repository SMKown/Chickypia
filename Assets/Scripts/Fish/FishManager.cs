using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
                var newFish = new Fish(lineData[0], lineData[1], Int32.Parse(lineData[2]), Int32.Parse(lineData[3]));
                allFish.Add(newFish);
            }
        }
    }

    public static Fish GetRandomFish()
    {
        return allFish[Random.Range(0, allFish.Count)];
    }

    public static Fish GetRandomFishWeighted()
    {
        var totalSpoke = 0;
        foreach (var fish in allFish)
        {
            totalSpoke += fish.spokeWeight;
        }
        var valueChosen = Random.Range(0, totalSpoke);
        foreach (var fish in allFish) 
        {
            if (valueChosen < fish.spokeWeight) 
            {
                return fish;
            } 
            else
            {
                valueChosen -= fish.spokeWeight;
            }
        }
        return null;
    }
}
