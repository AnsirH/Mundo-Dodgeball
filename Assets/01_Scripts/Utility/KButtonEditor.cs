#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(KButton), true)]
[CanEditMultipleObjects]
public class KButtonEditor : ButtonEditor
{
    // KButton에 있는 SerializedProperty를 잡아올 변수들
    SerializedProperty scaleUpMultiplier;
    SerializedProperty animationDuration;
    SerializedProperty easeType;
    SerializedProperty popupType;

    protected override void OnEnable()
    {
        base.OnEnable();

        // KButton 내 [SerializeField] 또는 public 필드를 찾아옵니다
        scaleUpMultiplier = serializedObject.FindProperty("scaleUpMultiplier");
        animationDuration = serializedObject.FindProperty("animationDuration");
        easeType = serializedObject.FindProperty("easeType");
        popupType = serializedObject.FindProperty("PopupType");
    }

    public override void OnInspectorGUI()
    {
        // ButtonEditor(부모)의 기본 Inspector 먼저 그리기 (Transition, Navigation, OnClick 등)
        base.OnInspectorGUI();

        // KButton의 추가 필드 그리기
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("KButton Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(scaleUpMultiplier);
        EditorGUILayout.PropertyField(animationDuration);
        EditorGUILayout.PropertyField(easeType);
        EditorGUILayout.PropertyField(popupType);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif