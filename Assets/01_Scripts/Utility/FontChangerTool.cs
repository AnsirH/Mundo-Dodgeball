#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class FontChangerTool : EditorWindow
{
    private Font legacyFont; // Unity UI (Text)�� ��Ʈ
    private TMP_FontAsset tmpFont; // TextMeshPro (TMP_Text)�� ��Ʈ

    [MenuItem("Tools/Font Changer")]
    public static void ShowWindow()
    {
        GetWindow<FontChangerTool>("Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("�� �� ��� ��Ʈ ����", EditorStyles.boldLabel);

        legacyFont = (Font)EditorGUILayout.ObjectField("Legacy Font (Text)", legacyFont, typeof(Font), false);
        tmpFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font (TMP_Text)", tmpFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("��Ʈ ���� ����"))
        {
            ChangeAllFonts();
        }
    }

    private void ChangeAllFonts()
    {
        if (legacyFont == null && tmpFont == null)
        {
            Debug.LogWarning("��Ʈ�� �����ϼ���!");
            return;
        }

        int textCount = 0;
        int tmpCount = 0;

        // Unity UI�� Text ã��
        Text[] allTextComponents = FindObjectsOfType<Text>(true);
        foreach (Text text in allTextComponents)
        {
            if (legacyFont != null)
            {
                text.font = legacyFont;
                textCount++;
            }
        }

        // TMP_Text ã��
        TMP_Text[] allTMPTextComponents = FindObjectsOfType<TMP_Text>(true);
        foreach (TMP_Text tmpText in allTMPTextComponents)
        {
            if (tmpFont != null)
            {
                tmpText.font = tmpFont;
                tmpCount++;
            }
        }

        Debug.Log($"��Ʈ ���� �Ϸ�! Text: {textCount}��, TMP_Text: {tmpCount}��");
        EditorUtility.DisplayDialog("Font Changer", $"��Ʈ ���� �Ϸ�!\nText: {textCount}��\nTMP_Text: {tmpCount}��", "Ȯ��");
    }
}
#endif
