﻿using System;
using UnityEditor;
using UnityEngine;

namespace AnimFlex.Core
{
    [CustomEditor(typeof(AnimFlexInitializer))]
    internal class AnimFlexInitializerEditor : Editor
    {
        private AnimFlexInitializer instance;
        
        private void OnEnable() => instance = target as AnimFlexInitializer;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_animFlexSettings"));
                if (GUILayout.Button("new", GUILayout.Width(50)))
                {
                    CreateNewSettings();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateNewSettings()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create new AnimFlexSettings",
                "AnimFlexSettings", 
                "asset", 
                "Select where to save the new AnimFlexSettings");
            instance.m_animFlexSettings = ScriptableObject.CreateInstance<AnimFlexSettings>();

            try
            {
                AssetDatabase.CreateAsset(instance.m_animFlexSettings, path);
                AssetDatabase.Refresh();
            }
            catch
            {
                // if not have a file, it'll be unsavable
                instance.m_animFlexSettings = null;
            }
        }
    }
}