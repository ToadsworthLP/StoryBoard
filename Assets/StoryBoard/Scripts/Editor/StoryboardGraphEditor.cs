using StoryBoard;
using UnityEngine;
using XNodeEditor;

namespace StoryBoardEditor {

    [CustomNodeGraphEditor(typeof(StoryboardGraph))]
    public class StoryboardGraphEditor : NodeGraphEditor {

        private const string NO_ROOT_ERROR = "No Root Nodes were found in this graph! Always make sure there is exactly one!";

        private StoryboardGraph storyboard;

        public override string GetNodeMenuName(System.Type type) {
            if (type.Namespace == "StoryBoard.Nodes") {
                return base.GetNodeMenuName(type).Replace("Story Board/Nodes/", "");
            } else return null;
        }

        public override void OnGUI() {
            base.OnGUI();

            if (storyboard == null) {
                storyboard = target as StoryboardGraph;
            }

            if(storyboard.RootNode == null) {
                Rect noRootWarningRect = new Rect(10, position.height - 25, 50, 10);
                GUI.Label(noRootWarningRect, NO_ROOT_ERROR, StoryboardResources.WarningLabel());
            }
        }

    }

}
