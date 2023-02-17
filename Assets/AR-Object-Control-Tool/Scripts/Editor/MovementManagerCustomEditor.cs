/////////////////////////////////////////////////////////////////////////
// AR-Object-Control-Tool -- MovementManagerCustomEditor
// Author: Léo Séry
// Date created: 26/01/2023
// Last updated: 16/02/2023
// Purpose: Modifies the Unity3D GUI to display the MovementManager script component of the tool in a more organized way.
// Documentation: https://github.com/LeoSery/AR-Object-Control-Tool--Unity3DTool
/////////////////////////////////////////////////////////////////////////

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

    SerializedProperty objectContainer;
    SerializedProperty objectToAffect;
    SerializedProperty targetCamera;
    SerializedProperty useExternalScript;
    SerializedProperty useElevetionManager;
    SerializedProperty fullAR;

    SerializedProperty targetTag;
    SerializedProperty blockingIU;

    SerializedProperty maxObjectScale;
    SerializedProperty minObjectScale;
    SerializedProperty useScaleIndicator;
    SerializedProperty showRawScale;
    SerializedProperty scaleIndicatorImagePrefab;
    SerializedProperty targetUICameraPrefab;
    SerializedProperty scaleText;
    SerializedProperty baseScale;

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

        objectContainer = serializedObject.FindProperty("objectContainer");
        objectToAffect = serializedObject.FindProperty("objectToAffect");
        targetCamera = serializedObject.FindProperty("targetCamera");
        useExternalScript = serializedObject.FindProperty("useExternalScript");
        useElevetionManager = serializedObject.FindProperty("useElevetionManager");
        fullAR = serializedObject.FindProperty("fullAR");

        targetTag = serializedObject.FindProperty("targetTag");
        blockingIU = serializedObject.FindProperty("blockingIU");
        useScaleIndicator = serializedObject.FindProperty("useScaleIndicator");
        showRawScale = serializedObject.FindProperty("showRawScale");
        scaleIndicatorImagePrefab = serializedObject.FindProperty("scaleIndicatorImagePrefab");
        targetUICameraPrefab = serializedObject.FindProperty("targetUICameraPrefab");
        scaleText = serializedObject.FindProperty("scaleText");
        baseScale = serializedObject.FindProperty("baseScale");

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
            EditorGUILayout.PropertyField(targetCamera);
            EditorGUILayout.PropertyField(useExternalScript);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        ScriptTargetSection = EditorGUILayout.BeginFoldoutHeaderGroup(ScriptTargetSection, "Script Target");
        if (ScriptTargetSection)
        {
            EditorGUILayout.PropertyField(objectContainer);
            EditorGUILayout.PropertyField(objectToAffect);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        UISettingsSection = EditorGUILayout.BeginFoldoutHeaderGroup(UISettingsSection, "UI");
        if (UISettingsSection)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(blockingIU);
            if (blockingIU.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(targetTag);
                EditorGUI.indentLevel--;
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

                EditorGUILayout.Space(3);

                EditorGUILayout.PropertyField(useScaleIndicator);
                if (useScaleIndicator.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(scaleIndicatorImagePrefab);
                    EditorGUILayout.PropertyField(targetUICameraPrefab);
                    EditorGUILayout.PropertyField(scaleText);
                    EditorGUILayout.PropertyField(baseScale);
                    EditorGUILayout.PropertyField(showRawScale);
                    EditorGUI.indentLevel--;
                }
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
                EditorGUI.indentLevel++;
                if (useElevetionManager.boolValue)
                    EditorGUILayout.PropertyField(elevationManager);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}
#endif
