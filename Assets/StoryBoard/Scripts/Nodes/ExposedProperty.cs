using System;
using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("")]
    public abstract class ExposedPropertyBase : StoryboardNode {
        public abstract Type GetPropertyType();
        public abstract void SetValue(object value);

        [ContextMenu("Generate Name")]
        public void ResetName() {
            name = GetPropertyType().Name + GetInstanceID();
        }
    }

    [CreateNodeMenu("")]
    public class ExposedPropertyNode<T> : ExposedPropertyBase {

        [Output] public T value;

        public override object GetValue(NodePort port) {
            return value;
        }

        public override Type GetPropertyType() {
            return typeof(T);
        }

        public override void SetValue(object value) {
            this.value = (T)value;
        }
    }

}