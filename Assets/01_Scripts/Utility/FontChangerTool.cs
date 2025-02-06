#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class FontChangerTool : EditorWindow
{
    private Font legacyFont; // Unity UI (Text)용 폰트
    private TMP_FontAsset tmpFont; // TextMeshPro (TMP_Text)용 폰트

    [MenuItem("Tools/Font Changer")]
    public static void ShowWindow()
    {
        GetWindow<FontChangerTool>("Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("씬 내 모든 폰트 변경", EditorStyles.boldLabel);

        legacyFont = (Font)EditorGUILayout.ObjectField("Legacy Font (Text)", legacyFont, typeof(Font), false);
        tmpFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font (TMP_Text)", tmpFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("폰트 변경 실행"))
        {
            ChangeAllFonts();
        }
    }

    private void ChangeAllFonts()
    {
        if (legacyFont == null && tmpFont == null)
        {
            Debug.LogWarning("폰트를 설정하세요!");
            return;
        }

        int textCount = 0;
        int tmpCount = 0;

        // Unity UI의 Text 찾기
        Text[] allTextComponents = FindObjectsOfType<Text>(true);
        foreach (Text text in allTextComponents)
        {
            if (legacyFont != null)
            {
                text.font = legacyFont;
                textCount++;
            }
        }

        // TMP_Text 찾기
        TMP_Text[] allTMPTextComponents = FindObjectsOfType<TMP_Text>(true);
        foreach (TMP_Text tmpText in allTMPTextComponents)
        {
            if (tmpFont != null)
            {
                tmpText.font = tmpFont;
                tmpCount++;
            }
        }

        Debug.Log($"폰트 변경 완료! Text: {textCount}개, TMP_Text: {tmpCount}개");
        EditorUtility.DisplayDialog("Font Changer", $"폰트 변경 완료!\nText: {textCount}개\nTMP_Text: {tmpCount}개", "확인");
    }
}
#endif
