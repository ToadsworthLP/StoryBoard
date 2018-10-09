using UnityEngine;
using XNode;

namespace StoryBoard.Nodes {

    public class ConsoleLogNode : StoryboardNode {

        [Input] public Flow Previous;
        [Output] public Flow Next;

        [TextArea]
        [Input] public string Message;

        public override void OnEnter() {
            base.OnEnter();
            Debug.Log(GetInputValue("Message", Message));
            Proceed("Next");
        }

        public override object GetValue(NodePort port) {
            return Next;
        }

    }

}
