using StoryBoard.Misc;
using System;
using System.Reflection;
using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    public class MethodCallNode : StoryboardNode {

        [Input] public Flow Previous;
        [Output] public Flow Next;

        [HideInInspector]
        public SerializableMethodInfo method;
        [HideInInspector]
        public SerializabeArgs[] methodArgs;

        private object cachedReturn;

        protected override void Init() {
            base.Init();
            
            if (method == null) method = new SerializableMethodInfo();
        }

        public override void OnEnter() {
            base.OnEnter();

            object target = GetInputValue<object>("Target", null);
            if (target != null && method.IsInitialized()) {
                cachedReturn = InvokeTargetMethod(target);
            } else {
                Debug.LogWarning("No object was provided to call a function on in node " + name + ". Skipping!");
            }

            Proceed("Next");
        }

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Next") {
                return Previous;
            } else if (port.fieldName == "Return" && method.ReturnType != typeof(void)) {
                object target = GetInputValue<object>("Target", null);
                if (target != null && method.IsInitialized()) {
                    if(cachedReturn == null) cachedReturn = InvokeTargetMethod(target);
                    return cachedReturn;
                } else {
#if !UNITY_EDITOR
                    Debug.LogWarning("No object was provided to call a function on in node " + name + ". Returning null!");
#endif
                    return null;
                }
            }

            return null;
        }

        private object InvokeTargetMethod(object target) {
            object[] args = new object[methodArgs.Length];
            for (int i = 0; i < methodArgs.Length; i++) {
                args[i] = GetInputValue(methodArgs[i].argumentName, methodArgs[i].GetValue());
            }

            MethodInfo methodInfo = method.Method;
            return methodInfo.Invoke(target, args);
        }

    }

    [Serializable]
    public class SerializableMethodInfo {
        [SerializeField]
        private string declaringTypeName;
        [SerializeField]
        private string methodName;
        [SerializeField]
        private string returnTypeName;
        [SerializeField]
        private string[] argTypeNames;

        private Type declaringType;
        private Type returnType;
        private Type[] argTypes;
        private MethodInfo method;

        public SerializableMethodInfo() {
            declaringTypeName = "";
            methodName = "";
            returnTypeName = "";
            argTypeNames = new string[0];
        }

        public SerializableMethodInfo(MethodInfo method) {
            if (method == null) {
                declaringTypeName = "";
                methodName = "";
                returnTypeName = "";
                argTypeNames = new string[0];
                return;
            }

            declaringTypeName = method.DeclaringType.AssemblyQualifiedName;
            methodName = method.Name;
            returnTypeName = method.ReturnType.AssemblyQualifiedName;
            argTypeNames = Array.ConvertAll(method.GetParameters(), param => param.ParameterType.AssemblyQualifiedName);
        }

        public bool IsInitialized() {
            return (DeclaringType != null && ArgTypes != null && Method != null);
        }

        public Type DeclaringType {
            get { if (declaringType == null) { declaringType = Type.GetType(declaringTypeName); } return declaringType; }
            set { declaringType = value; declaringTypeName = declaringType.AssemblyQualifiedName; method = null; }
        }

        public Type[] ArgTypes {
            get { if (argTypes == null) { argTypes = Array.ConvertAll(argTypeNames, name => Type.GetType(name)); } return argTypes; }
        }

        public Type ReturnType {
            get { if (returnType == null) { returnType = Type.GetType(returnTypeName); } return returnType; }
        }

        public MethodInfo Method {
            get { if (method == null) { method = DeclaringType.GetMethod(methodName, ArgTypes); } return method; }
        }
    }

}
