using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("Logic/Negate")]
    public class NegateNode : StoryboardNode {

        [Output] public bool output;
        [Input] public bool input;

        public override object GetValue(NodePort port) {
            return !GetInputValue<bool>("input");
        }
    }
}