using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MovementManager))]
public class MovementManagerCustomEditor : Editor
{
    #region SerializedProperties
    SerializedProperty RotationAxis;

    SerializedProperty Scaling;
    SerializedProperty Rotating;
    SerializedProperty Replacement;
    SerializedProperty Movement;

    SerializedProperty ObjectContainer;
    SerializedProperty ObjectToAffect;
    SerializedProperty useExternalScript;
    SerializedProperty useElevetionManager;
    SerializedProperty fullAR;

    SerializedProperty targetTag;
    SerializedProperty throughUI;

    SerializedProperty maxObjectScale;
    SerializedProperty minObjectScale;

    SerializedProperty rotationAxis;
    SerializedProperty rotationSpeed;
    SerializedProperty loadingObject;

    SerializedProperty elevationManager;

    bool AllowedOptionsSection = true;
    bool GeneralSettingsSection = false;
    bool ScriptTargetSection = false;
    bool UISettingsSection = false;
    bool ScaleSettingsSection = false;
    bool RotationSettingsSection = false;
    bool ReplacementSettingsSection = false;
    bool AdditionalScriptsSection = false;
    #endregion

    #region OnEnable()
    private void OnEnable()
    {
        RotationAxis = serializedObject.FindProperty("RotationAxis");

        Scaling = serializedObject.FindProperty("Scaling");
        Rotating = serializedObject.FindProperty("Rotating");
        Replacement = serializedObject.FindProperty("Replacement");
        Movement = serializedObject.FindProperty("Movement");

        ObjectContainer = serializedObject.FindProperty("ObjectContainer");
        ObjectToAffect = serializedObject.FindProperty("ObjectToAffect");
        useExternalScript = serializedObject.FindProperty("useExternalScript");
        useElevetionManager = serializedObject.FindProperty("useElevetionManager");
        fullAR = serializedObject.FindProperty("fullAR");

        targetTag = serializedObject.FindProperty("targetTag");
        throughUI = serializedObject.FindProperty("throughUI");

        maxObjectScale = serializedObject.FindProperty("maxObjectScale");
        minObjectScale = serializedObject.FindProperty("minObjectScale");

        rotationAxis = serializedObject.FindProperty("rotationAxis");
        rotationSpeed = serializedObject.FindProperty("rotationSpeed");
        loadingObject = serializedObject.FindProperty("loadingObject");

        elevationManager = serializedObject.FindProperty("elevationManager");
    }
    #endregion

    #region OnInspectorGUI()
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        AllowedOptionsSection = EditorGUILayout.BeginFoldoutHeaderGroup(AllowedOptionsSection, "Allowed Options");
        if (AllowedOptionsSection)
        {
            EditorGUILayout.PropertyField(Scaling);
            EditorGUILayout.PropertyField(Rotating);
            EditorGUILayout.PropertyField(Replacement);
            EditorGUILayout.PropertyField(Movement);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GeneralSettingsSection = EditorGUILayout.BeginFoldoutHeaderGroup(GeneralSettingsSection, "General Settings");
        if (GeneralSettingsSection)
        {
            EditorGUILayout.PropertyField(useExternalScript);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        ScriptTargetSection = EditorGUILayout.BeginFoldoutHeaderGroup(ScriptTargetSection, "Script Target");
        if (ScriptTargetSection)
        {
            EditorGUILayout.PropertyField(ObjectContainer);
            EditorGUILayout.PropertyField(ObjectToAffect);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        UISettingsSection = EditorGUILayout.BeginFoldoutHeaderGroup(UISettingsSection, "UI");
        if (UISettingsSection)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(throughUI);
            if (throughUI.boolValue)
            {
                EditorGUILayout.PropertyField(targetTag);
                if (targetTag.stringValue == "")
                {
                    targetTag.stringValue = "UI";
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (Scaling.boolValue || Rotating.boolValue || Replacement.boolValue || useExternalScript.boolValue)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.label);
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Selected options settings", centeredStyle);
        }

        if (Scaling.boolValue)
        {
            EditorGUILayout.Space(5);
            ScaleSettingsSection = EditorGUILayout.BeginFoldoutHeaderGroup(ScaleSettingsSection, "Scale");
            if (ScaleSettingsSection)
            {
                EditorGUILayout.PropertyField(maxObjectScale);
                EditorGUILayout.PropertyField(minObjectScale);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (Rotating.boolValue)
        {
            EditorGUILayout.Space(5);
            RotationSettingsSection = EditorGUILayout.BeginFoldoutHeaderGroup(RotationSettingsSection, "Rotation");
            if (RotationSettingsSection)
            {
                EditorGUILayout.PropertyField(rotationAxis);
                EditorGUILayout.PropertyField(rotationSpeed);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (Replacement.boolValue)
        {
            EditorGUILayout.Space(5);
            ReplacementSettingsSection = EditorGUILayout.BeginFoldoutHeaderGroup(ReplacementSettingsSection, "Replacement");
            if (ReplacementSettingsSection)
            {
                EditorGUILayout.PropertyField(loadingObject);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (useExternalScript.boolValue)
        {
            EditorGUILayout.Space(5);
            AdditionalScriptsSection = EditorGUILayout.BeginFoldoutHeaderGroup(AdditionalScriptsSection, "Additional Scripts");
            if (AdditionalScriptsSection)
            {
                EditorGUILayout.PropertyField(useElevetionManager);
                if (useElevetionManager.boolValue)
                    EditorGUILayout.PropertyField(elevationManager);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}
#endif
