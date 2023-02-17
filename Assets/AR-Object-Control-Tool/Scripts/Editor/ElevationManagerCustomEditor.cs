/////////////////////////////////////////////////////////////////////////
// AR-Object-Control-Tool -- ElevationManagerCustomEditor
// Author: Léo Séry
// Date created: 26/01/2023
// Last updated: 16/02/2023
// Purpose: Modifies the Unity3D GUI to display the Script ElevationManager component of the tool in a more organized way.
// Documentation: https://github.com/LeoSery/AR-Object-Control-Tool--Unity3DTool
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ElevationManager))]
public class ElevationManagerCustomEditor : Editor
{
    #region SerializedProperties
    SerializedProperty isItLevitate;
    SerializedProperty groundPosition;

    SerializedProperty objectToAffect;
    SerializedProperty objectContainer;
    SerializedProperty objectAnimator;

    SerializedProperty playAnimation;
    SerializedProperty objectFloatHeight;
    SerializedProperty Speed;

    SerializedProperty objectDefinesHeightGround;
    SerializedProperty heightReferenceObject;

    SerializedProperty stopButton;

    SerializedProperty movementManager;
    SerializedProperty useExternalScript;
    SerializedProperty useMovementManager;
    SerializedProperty useVuforia;
    SerializedProperty useButton;

    bool GeneralSettingsSection = true;
    bool GameObjectSection = false;
    bool AnimationSettingsSection = false;
    bool VurforiaSection = false;
    bool UISection = false;
    bool AdditionalScriptsSection = false;
    #endregion

    #region OnEnable()
    private void OnEnable()
    {
        isItLevitate = serializedObject.FindProperty("isItLevitate");
        groundPosition = serializedObject.FindProperty("groundPosition");

        objectToAffect = serializedObject.FindProperty("objectToAffect");
        objectContainer = serializedObject.FindProperty("objectContainer");
        objectAnimator = serializedObject.FindProperty("objectAnimator");

        playAnimation = serializedObject.FindProperty("playAnimation");
        objectFloatHeight = serializedObject.FindProperty("objectFloatHeight");
        Speed = serializedObject.FindProperty("Speed");

        objectDefinesHeightGround = serializedObject.FindProperty("objectDefinesHeightGround");
        heightReferenceObject = serializedObject.FindProperty("heightReferenceObject");

        stopButton = serializedObject.FindProperty("stopButton");

        useExternalScript = serializedObject.FindProperty("useExternalScript");
        movementManager = serializedObject.FindProperty("movementManager");
        useMovementManager = serializedObject.FindProperty("useMovementManager");
        useVuforia = serializedObject.FindProperty("useVuforia");
        useButton = serializedObject.FindProperty("useButton");
    }
    #endregion

    #region OnInspectorGUI()

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(isItLevitate);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(groundPosition);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.Space(5);
        GeneralSettingsSection = EditorGUILayout.Foldout(GeneralSettingsSection, "General Settings");
        if (GeneralSettingsSection)
        {
            EditorGUILayout.PropertyField(playAnimation);
            EditorGUILayout.PropertyField(useExternalScript);
            EditorGUILayout.PropertyField(useVuforia);
        }

        EditorGUILayout.Space(5);
        GameObjectSection = EditorGUILayout.Foldout(GameObjectSection, "GameObjects");
        if (GameObjectSection)
        {
            EditorGUILayout.PropertyField(objectContainer);
            EditorGUILayout.PropertyField(objectToAffect);
            if (playAnimation.boolValue)
                EditorGUILayout.PropertyField(objectAnimator);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);
        UISection = EditorGUILayout.Foldout(UISection, "UI");
        if (UISection)
        {
            EditorGUILayout.PropertyField(useButton);
            if (useButton.boolValue)
            {
                EditorGUILayout.PropertyField(stopButton);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (playAnimation.boolValue || useExternalScript.boolValue || useVuforia.boolValue)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.label);
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Selected options settings", centeredStyle);
        }

        if (playAnimation.boolValue)
        {
            EditorGUILayout.Space(5);
            AnimationSettingsSection = EditorGUILayout.Foldout(AnimationSettingsSection, "Animation");
            if (AnimationSettingsSection)
            {
                EditorGUILayout.PropertyField(objectFloatHeight);
                EditorGUILayout.PropertyField(Speed);
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        if (useVuforia.boolValue)
        {
            EditorGUILayout.Space(5);
            VurforiaSection = EditorGUILayout.Foldout(VurforiaSection, "Vuforia");
            if (VurforiaSection)
            {
                EditorGUILayout.PropertyField(objectDefinesHeightGround);
                if (objectDefinesHeightGround.boolValue)
                {
                    EditorGUILayout.PropertyField(heightReferenceObject);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (useExternalScript.boolValue)
        {
            EditorGUILayout.Space(5);
            AdditionalScriptsSection = EditorGUILayout.Foldout(AdditionalScriptsSection, "Additional Scripts");
            if (AdditionalScriptsSection)
            {
                EditorGUILayout.PropertyField(useMovementManager);
                if (useMovementManager.boolValue)
                {
                    EditorGUILayout.PropertyField(movementManager);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
#endif
