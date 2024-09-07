using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBase))]
public class CharacterBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        CharacterBase character = (CharacterBase)target;

        if (GUILayout.Button("SetRandom"))
        {
            character.SetRandom();
        }
    }
}