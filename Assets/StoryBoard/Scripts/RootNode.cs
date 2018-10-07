using XNode;

namespace StoryBoard.Nodes {

    public class RootNode : StoryboardNode {

        [Output] public Flow Next;

        public override void OnEnter() {
            base.OnEnter();
            Proceed("Next");
        }

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Next") return GetInputValue<Flow>("Next", Next);
            else return null;
        }

    }

}