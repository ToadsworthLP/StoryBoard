using StoryBoard.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace StoryBoardEditor {

    [CustomNodeEditor(typeof(SceneObjectNode))]
    public class SceneObjectNodeEditor : NodeEditor {

        private SceneObjectNode node;

        private static bool showSearchOptions = false;
        private static bool hideUnityClasses = true;
        private static string searchString = "";

        public override void OnBodyGUI() {
            base.OnBodyGUI();

            if (node == null) {
                node = (SceneObjectNode)target;
            }

            showSearchOptions = EditorGUILayout.Foldout(showSearchOptions, "Chooser Options");
            if (showSearchOptions) {
                hideUnityClasses = EditorGUILayout.Toggle("Hide Unity classes", hideUnityClasses);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Search");
                searchString = EditorGUILayout.TextField(searchString);
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUILayout.DropdownButton(new GUIContent(node.Object.GetTypeName()), FocusType.Keyboard)) {
                ShowTypeSelectMenu();
            }
        }

        private void ShowTypeSelectMenu() {
            GenericMenu genericMenu = new GenericMenu();
            Type[] types = GetAllSubclasses(typeof(Component), hideUnityClasses);

            if (types.Length == 0) {
                genericMenu.AddDisabledItem(new GUIContent("No matching classes found."));
            } else {
                genericMenu.AddDisabledItem(new GUIContent("Select a class..."));
            }

            foreach (Type type in types) {
                genericMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => OnSelectType(type));
            }

            genericMenu.ShowAsContext();
        }

        private void OnSelectType(Type type) {
            node.Object = new SceneObject(type, null);
            node.name = type.Name + node.GetInstanceID();
            serializedObject.ApplyModifiedProperties();
        }

        private static Type[] GetAllSubclasses(Type parentType, bool excludeUnityClasses) {
            IEnumerable<Type> children;

            if (excludeUnityClasses) {
                children = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                           from type in assembly.GetTypes()
                           where type.Namespace == null || !type.Namespace.StartsWith("Unity")
                           where type.IsSubclassOf(parentType)
                           where !type.IsAbstract
                           where type.Name.StartsWith(searchString)
                           select type;
            } else {
                children = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                           from type in assembly.GetTypes()
                           where type.IsSubclassOf(parentType)
                           where !type.IsAbstract
                           where type.Name.StartsWith(searchString)
                           select type;
            }

            return children.ToArray();
        }

    }

}
