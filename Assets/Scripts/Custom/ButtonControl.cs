using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    private GameObject[] _parts;
    private int idx;
    [field: SerializeField] private bool IsEmpty { get; set; }
    [SerializeField] private Image imageIcon;

    private PartsType CurrentPartType;

    public void SetButton(PartsType partsType, GameObject[] parts, Sprite icon, bool isNone)
    {
        CurrentPartType = partsType;
        IsEmpty = isNone;
        imageIcon.sprite = icon;
        imageIcon.SetNativeSize();
        _parts = parts;
        SetParts();
    }

    private void SetParts()
    {
        if (IsEmpty)
        {
            CharacterBase.Instance.SetItem(CurrentPartType, -1);
            idx = -1;
        }
        else
        {
            if (_parts.Length > 0)
            {
                CharacterBase.Instance.SetItem(CurrentPartType, idx % _parts.Length);
            }
        }
    }

    public void OnClick_Next()
    {
        idx++;
        if (idx >= _parts.Length) idx = IsEmpty ? -1 : 0;
        CharacterBase.Instance.SetItem(CurrentPartType, idx);
    }

    public void OnClick_Previous()
    {
        idx--;
        if (idx < (IsEmpty ? -1 : 0)) idx = _parts.Length - 1;
        CharacterBase.Instance.SetItem(CurrentPartType, idx);
    }
}