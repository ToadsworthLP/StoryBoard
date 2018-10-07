using UnityEngine;
using XNode;

namespace StoryBoard {

    public class StoryboardNode : Node {

        protected StoryboardGraph storyboard;

        public virtual void OnEnter() {
            storyboard = graph as StoryboardGraph;
        }

        public void Proceed(string outputName) {
            NodePort port = GetOutputPort(outputName);
            if (port == null || !port.IsConnected) { //If the port doesn't exist or doesn't have a next node, stop execution.
                storyboard.EndReached(null);
                return;
            }

            StoryboardNode next = port.Connection.node as StoryboardNode;
            next.OnEnter();
        }


    }

}
