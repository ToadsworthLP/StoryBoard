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

        private List<SceneObjectNode> sceneObjectNodes;
        private List<Type> sceneObjectNodeTypes;

        private List<ExposedPropertyBase> exposedPropertyNodes;

        public void OnEnable() {
            player = (StoryboardPlayer)target;
            previousGraph = player.storyboard;

            sceneObjectNodes = player.GetSceneObjectNodes();
            sceneObjectNodeTypes = sceneObjectNodes.ConvertAll(node => node.Object.GetObjectType());

            exposedPropertyNodes = player.GetExposedPropertyNodes();

            FixExposedSceneObjectSlots();
            FixExposedObjectSlots();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(player.storyboard != previousGraph) {
                OnEnable();
            }

            if (player.storyboard == null) return;

            GUILayout.Label("Exposed Properties", EditorStyles.boldLabel);

            for (int i = 0; i < sceneObjectNodes.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(sceneObjectNodes[i].name);
                player.sceneObjects[i] = EditorGUILayout.ObjectField(player.sceneObjects[i], sceneObjectNodeTypes[i], true);
                player.sceneObjectNodeNames[i] = sceneObjectNodes[i].name;
                EditorGUILayout.EndHorizontal();
            }

            for (int i = 0; i < exposedPropertyNodes.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(exposedPropertyNodes[i].name);

                SerializabeArgs args = player.exposedProperties[i];
                args.DrawEditor();

                player.exposedPropertyNodeNames[i] = exposedPropertyNodes[i].name;
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void FixExposedSceneObjectSlots() {
            UnityEngine.Object[] oldObjects = player.sceneObjects;
            string[] oldNodeNames = player.sceneObjectNodeNames;

            player.sceneObjects = new UnityEngine.Object[sceneObjectNodes.Count];
            player.sceneObjectNodeNames = new string[sceneObjectNodes.Count];

            for (int i = 0; i < sceneObjectNodes.Count; i++) {
                if (oldObjects.Length < i) continue;

                SceneObjectNode node = sceneObjectNodes[i];
                if (!oldNodeNames.Contains(node.name)) continue;

                int oldIndex = Array.IndexOf(oldNodeNames, node.name);
                player.sceneObjects[i] = oldObjects[oldIndex];
                player.sceneObjectNodeNames[i] = oldNodeNames[oldIndex];
            }
        }

        private void FixExposedObjectSlots() {
            SerializabeArgs[] oldProperties = player.exposedProperties;
            string[] oldNodeNames = player.exposedPropertyNodeNames;

            player.exposedProperties = new SerializabeArgs[exposedPropertyNodes.Count];
            player.exposedPropertyNodeNames = new string[exposedPropertyNodes.Count];

            for (int i = 0; i < exposedPropertyNodes.Count; i++) {
                if (oldProperties.Length < i) continue;

                ExposedPropertyBase node = exposedPropertyNodes[i];
                player.exposedProperties[i] = new SerializabeArgs();
                player.exposedProperties[i].argType = SerializabeArgs.ArgTypeOfType(node.GetPropertyType());

                if (!oldNodeNames.Contains(node.name)) continue;

                int oldIndex = Array.IndexOf(oldNodeNames, node.name);
                player.exposedProperties[i] = oldProperties[oldIndex];
                player.exposedPropertyNodeNames[i] = oldNodeNames[oldIndex];
            }
        }

    }

}
