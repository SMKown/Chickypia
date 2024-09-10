using UnityEngine;
using System.Collections.Generic;

public class CustomControl : MonoBehaviour
{
    [SerializeField] private ButtonControl button;
    [SerializeField] private Transform content;
    [SerializeField] private Sprite[] spriteIcons;

    private List<ButtonControl> buttons = new();

    private void Start()
    {
        SpawnPartsButton(PartsType.Hair, CharacterBase.Instance.PartsHair.ToArray(), "hair", false);
        SpawnPartsButton(PartsType.Face, CharacterBase.Instance.PartsFace.ToArray(), "face", false);
        SpawnPartsButton(PartsType.Headgear, CharacterBase.Instance.PartsHeadGear.ToArray(), "headgear", true);
        SpawnPartsButton(PartsType.Top, CharacterBase.Instance.PartsTop.ToArray(), "top", false);
        SpawnPartsButton(PartsType.Glove, CharacterBase.Instance.PartsGlove.ToArray(), "glove", true);
        SpawnPartsButton(PartsType.Bottom, CharacterBase.Instance.PartsBottom.ToArray(), "bottom", false);
        SpawnPartsButton(PartsType.Shoes, CharacterBase.Instance.PartsShoes.ToArray(), "shoes", false);
        SpawnPartsButton(PartsType.Bag, CharacterBase.Instance.PartsBag.ToArray(), "bag", true);
        SpawnPartsButton(PartsType.Eyewear, CharacterBase.Instance.PartsEyewear.ToArray(), "eyewear", true);

        button.gameObject.SetActive(false);
        CharacterBase.Instance.LoadInfo();
    }

    private Sprite GetSprite(string name)
    {
        foreach (var sprite in spriteIcons)
        {
            if (sprite.name.Contains(name))
            {
                return sprite;
            }
        }
        return spriteIcons[0];
    }

    private void SpawnPartsButton(PartsType partsType, GameObject[] parts, string name, bool isEmpty)
    {
        ButtonControl buttonControl = Instantiate(button, content, false);
        buttonControl.SetButton(partsType, parts, GetSprite(name), isEmpty);
        buttons.Add(buttonControl);
    }

    public void Input_Button_IDX(PartsType partsType, int inputidx)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetIdx(partsType, inputidx);
        }
    }
}
