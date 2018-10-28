using StoryBoard.Misc;
using StoryBoard.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace StoryBoardEditor {

    [CustomNodeEditor(typeof(MethodCallNode))]
    public class MethodCallNodeEditor : NodeEditor {

        private MethodCallNode node;

        private static bool showSearchOptions = false;
        private static bool hideUnityClasses = true;
        private static string searchString = "";

        public override int GetWidth() {
            return 300;
        }

        public override void OnBodyGUI() {
            base.OnBodyGUI();

            if (node == null) node = (MethodCallNode)target;

            showSearchOptions = EditorGUILayout.Foldout(showSearchOptions, "Chooser Options");
            if (showSearchOptions) {
                hideUnityClasses = EditorGUILayout.Toggle("Hide Unity classes", hideUnityClasses);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Search");
                searchString = EditorGUILayout.TextField(searchString);
                EditorGUILayout.EndHorizontal();
            }

            GUIContent label = (node.method.DeclaringType == null) ? new GUIContent("Select a type...") : new GUIContent(ObjectNames.NicifyVariableName(node.method.DeclaringType.Name));
            if (EditorGUILayout.DropdownButton(label, FocusType.Keyboard)) { ShowTypeSelectMenu(); }

            if (node.method.DeclaringType == null) return;

            label = (node.method.Method == null) ? new GUIContent("Select a method...") : new GUIContent(node.method.Method.Name);
            if (EditorGUILayout.DropdownButton(label, FocusType.Keyboard)) { ShowMethodSelectMenu(); }

            if (node.method.Method == null) return;

            DrawInstancePorts();

            if (node.methodArgs == null) return;

            DrawParameterSection();
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
            node.methodArgs = null;
            node.method.DeclaringType = type;

            node.ClearInstancePorts();
            node.AddInstanceInput(node.method.DeclaringType, Node.ConnectionType.Override, "Target");
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

        private void DrawInstancePorts() {
            if(node.method.ReturnType == typeof(void)) {
                NodeEditorGUILayout.PortField(new GUIContent("Target"), node.GetInputPort("Target"));
            } else {
                NodeEditorGUILayout.PortPair(node.GetInputPort("Target"), node.GetOutputPort("Return"));
            }
        }

        private void ShowMethodSelectMenu() {
            GenericMenu genericMenu = new GenericMenu();
            MethodInfo[] methods = node.method.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            genericMenu.AddDisabledItem(new GUIContent("Select a method..."));

            foreach (MethodInfo method in methods) {
                if (method.IsGenericMethod) continue;
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
            node.method = new SerializableMethodInfo(method);
            node.methodArgs = new SerializabeArgs[parameters.Length];

            node.method = new SerializableMethodInfo(method);

            node.ClearInstancePorts();
            node.AddInstanceInput(node.method.DeclaringType, Node.ConnectionType.Override, "Target");

            if(node.method.ReturnType != typeof(void)) {
                node.AddInstanceOutput(node.method.ReturnType, Node.ConnectionType.Override, "Return");
            }

            for (int i = 0; i < parameters.Length; i++) {
                SerializabeArgs args = new SerializabeArgs();
                args.argumentName = parameters[i].Name;
                args.argType = SerializabeArgs.ArgTypeOfType(parameters[i].ParameterType);
                args.objectTypeName = parameters[i].ParameterType.AssemblyQualifiedName;
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

            MethodInfo method = node.method.Method;
            if (method == null) return;

            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < node.methodArgs.Length; i++) {

                SerializabeArgs args = node.methodArgs[i];
                if (args.argType == SerializabeArgs.ArgType.Unsupported) continue;

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(ObjectNames.NicifyVariableName(parameters[i].Name)); //Ensures that the port field has something to calculate it's draw position with

                NodePort argPort = node.GetInputPort(parameters[i].Name);
                NodeEditorGUILayout.AddPortField(argPort);

                if (!argPort.IsConnected) {
                    args.DrawEditor(false);
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

}
