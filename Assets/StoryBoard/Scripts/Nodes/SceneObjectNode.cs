using System;
using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("Exposed Property/Scene Object")]
    public class SceneObjectNode : StoryboardNode {

        [Output] public SceneObject Object;

        protected override void Init() {
            if(Object == null) {
                Object = new SceneObject(typeof(UnityEngine.Object), null);
            }
        }

        public override object GetValue(NodePort port) {
            return Object;
        }

    }

    [Serializable]
    public class SceneObject {
        [HideInInspector]
        [SerializeField]
        private string typeName;
        private Type type;
        private object reference;

        public SceneObject(Type type, object reference) {
            this.typeName = type.AssemblyQualifiedName;
            this.type = type;
            this.reference = reference;
        }

        public void SetObject(object reference) {
            this.reference = reference;
        }

        public Type GetObjectType() {
            if(type == null) {
                type = Type.GetType(typeName);
            }

            return type;
        }

        public string GetTypeName() {
            if(typeName == null) {
                typeName = type.AssemblyQualifiedName;
            }

            return typeName;
        }

        public object GetObject() {
            return reference;
        }
    }

}
