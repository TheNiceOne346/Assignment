using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Graphs;

namespace Game
{
    [CustomEditor(typeof(Dungeon), true)]
    public class DungeonEditor : Editor
    {
        private void OnSceneGUI()
        {
            Dungeon dungeon = target as Dungeon;
            EditorGraphUtils.DrawGraph(dungeon);
        }
    }
}