using StoryBoard.Misc;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    public class SceneMethodCallNode : StoryboardNode {

        [Input] public Flow Previous;
        [Output] public Flow Next;

        [Input] public SceneObject Target;

        [HideInInspector]
        public string methodName;
        [HideInInspector]
        public SerializabeArgs[] methodArgs;

        private Type targetType;
        private MethodInfo method;

        public override void OnEnter() {
            base.OnEnter();

            Target = GetInputValue("Target", Target);
            if (Target != null && Target.GetObject() != null) { InvokeTargetMethod(); } else { Debug.LogWarning("No target object was passed into node " + name + ". Skipping!"); }

            Proceed("Next");
        }

        private void InvokeTargetMethod() {
            if (methodName == null || methodName == "") { Debug.LogError("No method name was set in node " + name); return; }

            targetType = Target.GetObjectType();
            method = targetType.GetMethod(methodName, new List<SerializabeArgs>(methodArgs).ConvertAll<Type>(arg => arg.GetActualType()).ToArray());
            if (method == null) { Debug.LogError("The method " + methodName + " could not be found!"); return; }

            object[] args = new object[methodArgs.Length];
            for (int i = 0; i < methodArgs.Length; i++) {
                args[i] = GetInputValue(methodArgs[i].argumentName, methodArgs[i].GetValue());
            }

            method.Invoke(Target.GetObject(), args);
        }

        public override object GetValue(NodePort port) {
            return Next;
        }
    }



}
