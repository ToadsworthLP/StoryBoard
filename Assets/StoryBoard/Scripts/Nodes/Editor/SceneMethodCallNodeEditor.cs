using StoryBoard.Misc;
using StoryBoard.Nodes;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace StoryBoardEditor {

    [CustomNodeEditor(typeof(SceneMethodCallNode))]
    public class SceneMethodCallNodeEditor : NodeEditor {

        private SceneMethodCallNode node;
        private Type targetType;

        private static bool showSearchOptions = false;
        private static string searchString = "";

        public override int GetWidth() {
            return 300;
        }

        public override void OnBodyGUI() {
            base.OnBodyGUI();

            if (node == null) node = (SceneMethodCallNode)target;

            node.Target = node.GetInputValue<SceneObject>("Target");
            SceneObject targetObj = node.Target;

            targetType = (targetObj == null) ? null : targetObj.GetObjectType();

            try {
                if (targetType == null) {
                    GUILayout.Label("No target type found.");
                } else {
                    GUILayout.Label("Type: " + targetType.Name);

                    showSearchOptions = EditorGUILayout.Foldout(showSearchOptions, "Chooser Options");
                    if (showSearchOptions) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Search");
                        searchString = EditorGUILayout.TextField(searchString);
                        EditorGUILayout.EndHorizontal();
                    }

                    GUIContent buttonText = (node.methodName == "") ? new GUIContent("Select a method...") : new GUIContent(node.methodName);
                    if (EditorGUILayout.DropdownButton(buttonText, FocusType.Keyboard)) {
                        ShowMethodSelectMenu();
                    }

                    if(node.methodName != "") {
                        DrawParameterSection();
                    }
                }
            }
            catch (ArgumentException) {} //Ignore the layout argument exception, too lazy to fix properly now
        }

        private void ShowMethodSelectMenu() {
            GenericMenu genericMenu = new GenericMenu();
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            genericMenu.AddDisabledItem(new GUIContent("Select a method..."));

            foreach (MethodInfo method in methods) {
                if (method.IsGenericMethod) continue;
                if (method.ReturnType != typeof(void)) continue;
                if (!method.Name.StartsWith(searchString)) continue;

                ParameterInfo[] parameters = method.GetParameters();
                string paramString = "";

                if (parameters.Any(x => !SerializabeArgs.IsSupported(x.ParameterType))) continue;

                if (parameters != null && parameters.Length > 0) {
                    foreach (ParameterInfo parameter in parameters) {
                        paramString += parameter.ParameterType.Name + " " + parameter.Name + ", ";
                    }
                }

                if (paramString == "") {
                    genericMenu.AddItem(new GUIContent(method.Name + "()"), false, () => OnSelectMethod(method, parameters));
                } else {
                    genericMenu.AddItem(new GUIContent(method.Name + "(" + paramString.Remove(paramString.Length - 2) + ")"), false, () => OnSelectMethod(method, parameters));
                }
            }

            if (genericMenu.GetItemCount() <= 1) {
                genericMenu = new GenericMenu();
                genericMenu.AddDisabledItem(new GUIContent("No matching methods found."));
            }
            genericMenu.ShowAsContext();
        }

        private void OnSelectMethod(MethodInfo method, ParameterInfo[] parameters) {
            node.methodName = method.Name;
            node.methodArgs = new SerializabeArgs[parameters.Length];

            node.ClearInstancePorts();

            for (int i = 0; i < parameters.Length; i++) {
                SerializabeArgs args = new SerializabeArgs();
                args.argumentName = parameters[i].Name;
                args.argType = SerializabeArgs.ArgTypeOfType(parameters[i].ParameterType);
                args.valueTypeName = parameters[i].ParameterType.AssemblyQualifiedName;
                node.methodArgs[i] = args;

                node.AddInstanceInput(parameters[i].ParameterType, Node.ConnectionType.Override, parameters[i].Name);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawParameterSection() {
            if (node.methodArgs == null || node.methodArgs.Length == 0) return;

            Type[] argTypes = new Type[node.methodArgs.Length];

            for (int i = 0; i < node.methodArgs.Length; i++) {
                argTypes[i] = node.methodArgs[i].GetActualType();
            }

            MethodInfo method = targetType.GetMethod(node.methodName, argTypes);
            if (method == null) return;

            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < node.methodArgs.Length; i++) {

                SerializabeArgs args = node.methodArgs[i];
                if (args.argType == SerializabeArgs.ArgType.Unsupported) continue;

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(ObjectNames.NicifyVariableName(parameters[i].Name)); //Ensures that the port field has something to calculate it's draw position with

                NodePort argPort = node.GetInputPort(parameters[i].Name);
                NodeEditorGUILayout.AddPortField(argPort);

                if(!argPort.IsConnected) {
                    args.DrawEditor();
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

}
