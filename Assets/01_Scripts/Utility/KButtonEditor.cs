#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(KButton), true)]
[CanEditMultipleObjects]
public class KButtonEditor : ButtonEditor
{
    // KButton�� �ִ� SerializedProperty�� ��ƿ� ������
    SerializedProperty scaleUpMultiplier;
    SerializedProperty animationDuration;
    SerializedProperty easeType;
    SerializedProperty popupType;

    protected override void OnEnable()
    {
        base.OnEnable();

        // KButton �� [SerializeField] �Ǵ� public �ʵ带 ã�ƿɴϴ�
        scaleUpMultiplier = serializedObject.FindProperty("scaleUpMultiplier");
        animationDuration = serializedObject.FindProperty("animationDuration");
        easeType = serializedObject.FindProperty("easeType");
        popupType = serializedObject.FindProperty("PopupType");
    }

    public override void OnInspectorGUI()
    {
        // ButtonEditor(�θ�)�� �⺻ Inspector ���� �׸��� (Transition, Navigation, OnClick ��)
        base.OnInspectorGUI();

        // KButton�� �߰� �ʵ� �׸���
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