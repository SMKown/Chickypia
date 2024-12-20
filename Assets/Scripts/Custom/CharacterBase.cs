using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public enum PartsType
{
    Hair,
    Face,
    Headgear,
    Top,
    Bottom,
    Bag,
    Shoes,
    Glove,
    Eyewear,
    Body
}

public class CharacterData
{
    public int HairIndex;
    public int FaceIndex;
    public int HeadGearIndex;
    public int TopIndex;
    public int BottomIndex;
    public int EyewearIndex;
    public int BagIndex;
    public int ShoesIndex;
    public int GloveIndex;
}

public class CharacterBase : MonoBehaviour
{
    public static CharacterBase Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public CustomControl customControl;
    private string filePath;

    private void Start()
    {
        SetRoot();
        filePath = Path.Combine(Application.persistentDataPath, "characterData.json");
        // C:\Users\[user name]\AppData\LocalLow\[company name]\[product name]

        if (SceneManager.GetActiveScene().name != "CustomScene")
            LoadInfo();
    }
    
    public List<GameObject> PartsBody { get; set; } = new();
    public List<GameObject> PartsHair { get; set; } = new();
    public List<GameObject> PartsFace { get; set; } = new();
    public List<GameObject> PartsHeadGear { get; set; } = new();
    public List<GameObject> PartsTop { get; set; } = new();
    public List<GameObject> PartsBottom { get; set; } = new();
    public List<GameObject> PartsEyewear { get; set; } = new();
    public List<GameObject> PartsBag { get; set; } = new();
    public List<GameObject> PartsShoes { get; set; } = new();
    public List<GameObject> PartsGlove { get; set; } = new();

    public int Index { get; set; } = 0;
    
    public void SetRandom()
    {
        SetRoot();
        SetItem(PartsType.Hair, Random.Range(0, PartsHair.Count - 1));
        SetItem(PartsType.Face,Random.Range(0, PartsFace.Count - 1));
        SetItem(PartsType.Headgear, Random.Range(-5, PartsHeadGear.Count - 1));
        SetItem(PartsType.Top, Random.Range(0, PartsTop.Count - 1));
        SetItem(PartsType.Bottom, Random.Range(0, PartsBottom.Count - 1));
        SetItem(PartsType.Eyewear, Random.Range(-5, PartsEyewear.Count - 1));
        SetItem(PartsType.Bag, Random.Range(-5, PartsBag.Count - 1));
        SetItem(PartsType.Shoes, Random.Range(0, PartsShoes.Count - 1));
        SetItem(PartsType.Glove, Random.Range(-5, PartsGlove.Count - 1));
    }
    
    protected void SetRoot()
    {
        PartsHair.Clear();
        PartsFace.Clear();
        PartsHeadGear.Clear();
        PartsTop.Clear();
        PartsBottom.Clear();
        PartsEyewear.Clear();
        PartsBag.Clear();
        PartsShoes.Clear();
        PartsGlove.Clear();
        PartsBody.Clear();

        Transform parts = null;
        foreach (Transform t in transform)
        {
            if (t.name == "Parts") parts = t;

            if (t.name == "Body")
            {
                foreach (Transform child in t.transform)
                {
                    if (child.gameObject.name.Contains("Body_1") || child.gameObject.name.Contains("Body_2"))
                    {
                        child.gameObject.SetActive(false);
                        PartsBody.Add(child.gameObject);
                    }  
                }
            }
        }

        foreach (Transform g in parts)
        {
            if (g.name.Contains($"{PartsType.Hair}")) foreach (Transform child in g.transform) PartsHair.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Face}")) foreach (Transform child in g.transform) PartsFace.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Headgear}")) foreach (Transform child in g.transform) PartsHeadGear.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Top}")) foreach (Transform child in g.transform) PartsTop.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Bottom}")) foreach (Transform child in g.transform) PartsBottom.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Eyewear}")) foreach (Transform child in g.transform) PartsEyewear.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Bag}")) foreach (Transform child in g.transform) PartsBag.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Shoes}")) foreach (Transform child in g.transform) PartsShoes.Add(child.gameObject);
            if (g.name.Contains($"{PartsType.Glove}")) foreach (Transform child in g.transform) PartsGlove.Add(child.gameObject);
        }
    }

    private bool IsEquipGlove { get; set; }
    public void CheckBody()
    {
        PartsBody[0].SetActive(IsEquipGlove);
        PartsBody[1].SetActive(!IsEquipGlove);
    }
    
    public void SetItem(PartsType partsType, int idx)
    {
        switch (partsType)
        {
            case PartsType.Hair:
                for (int i = 0; i < PartsHair.Count; i++) PartsHair[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Face:
                for (int i = 0; i < PartsFace.Count; i++) PartsFace[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Headgear:
                for (int i = 0; i < PartsHeadGear.Count; i++) PartsHeadGear[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Top:
                for (int i = 0; i < PartsTop.Count; i++) PartsTop[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Bottom:
                for (int i = 0; i < PartsBottom.Count; i++) PartsBottom[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Bag:
                for (int i = 0; i < PartsBag.Count; i++) PartsBag[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Shoes:
                for (int i = 0; i < PartsShoes.Count; i++) PartsShoes[i].gameObject.SetActive(i == idx);
                break;
            case PartsType.Glove:
                IsEquipGlove = false;
                
                for (int i = 0; i < PartsGlove.Count; i++)
                {
                    if (i == idx) IsEquipGlove = true; 
                    PartsGlove[i].gameObject.SetActive(i == idx);
                }
                break;
            case PartsType.Eyewear:
                for (int i = 0; i < PartsEyewear.Count; i++) PartsEyewear[i].gameObject.SetActive(i == idx);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(partsType), partsType, null);
        }

        CheckBody();

        if (customControl != null)
            customControl.Input_Button_IDX(partsType, idx);
    }

    public void SaveInfo()
    {
        var characterData = new CharacterData
        {
            HairIndex = GetActiveIndex(PartsHair),
            FaceIndex = GetActiveIndex(PartsFace),
            HeadGearIndex = GetActiveIndex(PartsHeadGear),
            TopIndex = GetActiveIndex(PartsTop),
            BottomIndex = GetActiveIndex(PartsBottom),
            EyewearIndex = GetActiveIndex(PartsEyewear),
            BagIndex = GetActiveIndex(PartsBag),
            ShoesIndex = GetActiveIndex(PartsShoes),
            GloveIndex = GetActiveIndex(PartsGlove)
        };

        string json = JsonUtility.ToJson(characterData);
        File.WriteAllText(filePath, json);
    }

    private int GetActiveIndex(List<GameObject> parts)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].activeInHierarchy)
                return i;
        }
        return -1;
    }

    public void LoadInfo()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CharacterData characterData = JsonUtility.FromJson<CharacterData>(json);

            SetItem(PartsType.Hair, characterData.HairIndex);
            SetItem(PartsType.Face, characterData.FaceIndex);
            SetItem(PartsType.Headgear, characterData.HeadGearIndex);
            SetItem(PartsType.Top, characterData.TopIndex);
            SetItem(PartsType.Bottom, characterData.BottomIndex);
            SetItem(PartsType.Eyewear, characterData.EyewearIndex);
            SetItem(PartsType.Bag, characterData.BagIndex);
            SetItem(PartsType.Shoes, characterData.ShoesIndex);
            SetItem(PartsType.Glove, characterData.GloveIndex);
        }
        else
        {
            SetItem(PartsType.Top, 8); // 기본 상의 인덱스
            SetItem(PartsType.Bottom, 47); // 기본 하의 인덱스
            SetItem(PartsType.Bag, 8); // 기본 가방 인덱스
            SetItem(PartsType.Shoes, 23); // 기본 신발 인덱스
        }
    }

    public void ClearInfo()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}