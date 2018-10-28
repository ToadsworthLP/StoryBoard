using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("Logic/Reference Equals")]
    public class ReferenceEqualsNode : StoryboardNode {

        [Output] public bool equal;

        [Input] public Object a;
        [Input] public Object b;

        public override object GetValue(NodePort port) {
            return ReferenceEquals(GetInputValue<bool>("a"), GetInputValue<bool>("b"));
        }
    }
}