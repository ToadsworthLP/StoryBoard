using StoryBoard;
using StoryBoard.Nodes;
using StoryBoard.Misc;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace StoryBoardEditor {

    [CustomEditor(typeof(StoryboardPlayer))]
    public class StoryboardPlayerEditor : Editor {

        private StoryboardPlayer player;
        private StoryboardGraph previousGraph;

        private List<ExposedPropertyBase> exposedPropertyNodes;

        public void OnEnable() {
            player = (StoryboardPlayer)target;
            previousGraph = player.storyboard;

            if (player.storyboard == null) return;

            exposedPropertyNodes = player.GetExposedPropertyNodes();

            FixExposedObjectSlots();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (player.storyboard == null) return;

            if (player.storyboard != previousGraph) {
                previousGraph = player.storyboard;

                exposedPropertyNodes = player.GetExposedPropertyNodes();

                FixExposedObjectSlots();
            }

            GUILayout.Label("Exposed Properties", EditorStyles.boldLabel);

            Undo.RecordObject(target, "Modify Exposed Property");

            for (int i = 0; i < exposedPropertyNodes.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(exposedPropertyNodes[i].name);

                SerializabeArgs args = player.exposedProperties[i];

                args.DrawEditor(true);

                player.exposedPropertyNodeNames[i] = exposedPropertyNodes[i].name;
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void FixExposedObjectSlots() {
            SerializabeArgs[] oldProperties = player.exposedProperties;
            string[] oldNodeNames = player.exposedPropertyNodeNames;

            player.exposedProperties = new SerializabeArgs[exposedPropertyNodes.Count];
            player.exposedPropertyNodeNames = new string[exposedPropertyNodes.Count];

            for (int i = 0; i < exposedPropertyNodes.Count; i++) {
                ExposedPropertyBase node = exposedPropertyNodes[i];
                player.exposedProperties[i] = new SerializabeArgs();
                player.exposedProperties[i].argType = SerializabeArgs.ArgTypeOfType(node.GetPropertyType());
                player.exposedProperties[i].valueTypeName = node.GetPropertyType().AssemblyQualifiedName;

                if (!oldNodeNames.Contains(node.name)) continue;

                int oldIndex = Array.IndexOf(oldNodeNames, node.name);
                player.exposedProperties[i] = oldProperties[oldIndex];
                player.exposedPropertyNodeNames[i] = oldNodeNames[oldIndex];
            }
        }

    }

}
