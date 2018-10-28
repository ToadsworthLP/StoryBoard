using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("Logic/Equals")]
    public class EqualsNode : StoryboardNode {

        [Output] public bool equal;

        [Input] public Object a;
        [Input] public Object b;

        public override object GetValue(NodePort port) {
            return GetInputValue<bool>("a").Equals(GetInputValue<bool>("b"));
        }
    } 
}