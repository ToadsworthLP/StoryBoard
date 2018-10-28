using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    [CreateNodeMenu("Flow Control/If")]
    public class IfNode : StoryboardNode {

        [Input] public Flow Previous;

        [Output] public Flow True;
        [Output] public Flow False;

        [Input] public bool condition;

        public override void OnEnter() {
            base.OnEnter();
            if (GetInputValue<bool>("condition")) { Proceed("True"); } else { Proceed("False"); }
        }

        public override object GetValue(NodePort port) {
            return Previous;
        }

    }

}
