using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game
{
    [CustomEditor(typeof(Tile), true)]
    public class TileEditor : Editor
    {
        private Tile.NodeData   m_selected;
        private Tool            m_oldTool;
        private Vector3         m_vMovePosition;
        private bool            m_bPrefabEdit;

        private void OnEnable()
        {
            m_bPrefabEdit = PrefabStageUtility.GetCurrentPrefabStage() != null;
            m_oldTool = Tools.current;
        }

        private void OnDisable()
        {
            Tools.current = m_oldTool;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            if (!m_bPrefabEdit)
            {
                return;
            }

            // Edit Node Data
            GUILayout.Space(10);
            Tile tile = target as Tile;
            EditorGUILayout.LabelField("Node Data (" + tile.m_nodeData.Count + ")", EditorStyles.boldLabel);
            foreach (Tile.NodeData nodeData in tile.m_nodeData)
            {
                // select / unselect node data
                if (GUILayout.Button("Node " + nodeData.m_vLocalPosition.ToString() + " (" + nodeData.m_nodeType + ")"))
                {
                    m_selected = m_selected == nodeData ? null : nodeData;
                    m_vMovePosition = m_selected != null ? tile.transform.TransformPoint(m_selected.m_vLocalPosition) : m_vMovePosition;
                    SceneView.RepaintAll();
                }

                // edit selected node?
                if (m_selected == nodeData)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;

                    // position
                    m_selected.m_vLocalPosition = EditorGUILayout.Vector3IntField("Position", m_selected.m_vLocalPosition);

                    // node type
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Type");
                    if (GUILayout.Button(string.IsNullOrEmpty(m_selected.m_nodeType) ? "<None>" : m_selected.m_nodeType))
                    {
                        GenericMenu gm = new GenericMenu();
                        foreach (System.Type type in typeof(Dungeon).Assembly.GetTypes())
                        {
                            if (!type.IsAbstract && typeof(Dungeon.Node).IsAssignableFrom(type))
                            {
                                gm.AddItem(new GUIContent(type.Name), type.Name == m_selected.m_nodeType, SetNodeType, type.Name);
                            }
                        }
                        gm.ShowAsContext();
                    }
                    GUILayout.EndHorizontal();

                    // delete?
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(120)))
                    {
                        tile.m_nodeData.Remove(m_selected);
                        m_selected = null;
                        break;
                    }
                    GUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                    GUILayout.EndVertical();
                }
            }

            // Add new Node Data?
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(60)))
            {
                m_selected = new Tile.NodeData();
                m_vMovePosition = Vector3.zero;
                tile.m_nodeData.Add(m_selected);
            }
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        private void SetNodeType(object userData)
        {
            if (m_selected != null)
            {
                m_selected.m_nodeType = userData as string;
                EditorUtility.SetDirty(target);
            }
        }

        private void OnSceneGUI()
        {
            if (!m_bPrefabEdit)
            {
                return;
            }

            Tools.current = Tool.None;
            Tile tile = target as Tile;
            foreach (Tile.NodeData nodeData in tile.m_nodeData)
            {
                bool bSelected = m_selected == nodeData;

                // draw node cube
                Handles.color = new Color(1.0f, bSelected ? 1.0f : 0.5f, 0.0f, bSelected ? 1.0f : 0.5f);
                Vector3 vWorldPosition = tile.transform.TransformPoint(nodeData.m_vLocalPosition);

                if (bSelected)
                {
                    Handles.CubeHandleCap(0, vWorldPosition, Quaternion.identity, 0.12f, EventType.Repaint);
                }
                else if(Handles.Button(vWorldPosition, Quaternion.identity, 0.08f, 0.1f, Handles.CubeHandleCap))
                {
                    m_selected = nodeData;
                    m_vMovePosition = tile.transform.TransformPoint(m_selected.m_vLocalPosition);
                    Repaint();
                }

                // move handle?
                if (bSelected)
                {
                    Vector3 vNewWorld = Handles.DoPositionHandle(m_vMovePosition, Quaternion.identity);
                    if (Vector3.Distance(vNewWorld, m_vMovePosition) > 0.001f)
                    {
                        m_vMovePosition = vNewWorld;
                        Vector3 vNewLocal = tile.transform.InverseTransformPoint(vNewWorld);
                        nodeData.m_vLocalPosition = new Vector3Int(Mathf.RoundToInt(vNewLocal.x), Mathf.RoundToInt(vNewLocal.y), Mathf.RoundToInt(vNewLocal.z));
                        EditorUtility.SetDirty(tile);
                        Repaint();
                    }
                }
            }
        }
    }
}