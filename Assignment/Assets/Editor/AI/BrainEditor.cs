using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AI
{
    [CustomEditor(typeof(Brain), true)]
    public class BrainEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // debug blackboard
            Brain brain = target as Brain;
            if (brain.Tree != null && 
                Application.isPlaying &&
                brain.Tree.Blackboard != null)
            {
                Blackboard bb = brain.Tree.Blackboard;
                GUILayout.Space(10);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Blackboard (" + bb.Count + ")", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // draw each blackboard item
                foreach (KeyValuePair<string, object> kvp in bb.Items)
                {
                    if (kvp.Value == null)
                    {
                        EditorGUILayout.LabelField(kvp.Key, "NULL");
                    }
                    else if (kvp.Value is IEnumerable)
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField(kvp.Key, kvp.Value.GetType().Name);
                        IEnumerable objects = (IEnumerable)kvp.Value;
                        foreach (object obj in objects)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.PrefixLabel("");
                            EditorGUILayout.LabelField(obj.ToString());
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorGUILayout.LabelField(kvp.Key, kvp.Value.ToString());
                    }
                }

                EditorGUI.indentLevel--;
                GUILayout.EndVertical();

                Repaint();
            }
        }

    }
}