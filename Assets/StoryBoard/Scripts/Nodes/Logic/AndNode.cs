using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("Logic/And")]
    public class AndNode : StoryboardNode {

        [Output] public bool output;

        [Input] public bool a;
        [Input] public bool b;

        public override object GetValue(NodePort port) {
            return (GetInputValue<bool>("a") && GetInputValue<bool>("b"));
        }
    }
}