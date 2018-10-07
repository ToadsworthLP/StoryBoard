using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    public class ConsoleLogNode : StoryboardNode {

        [Input] public Flow Previous;
        [Output] public Flow Next;

        [TextArea]
        public string message;

        public override void OnEnter() {
            base.OnEnter();
            Debug.Log(message);
            Proceed("Next");
        }

        public override object GetValue(NodePort port) {
            return Next;
        }

    }

}
