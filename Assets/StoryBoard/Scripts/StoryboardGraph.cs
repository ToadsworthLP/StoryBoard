using StoryBoard.Nodes;
using System;
using UnityEngine;
using XNode;

namespace StoryBoard {

    [CreateAssetMenu(menuName = "Storyboard Graph", fileName = "StoryBoard/Storyboard Graph")]
    public class StoryboardGraph : NodeGraph {
        
        private Action<object> OnFinished;
        private RootNode rootNode;

        public RootNode RootNode {
            get { if (rootNode == null) { rootNode = FindRootNode(); } return rootNode; }
        }

        public void Play(Action<object> OnFinished) {
            this.OnFinished = OnFinished;

            if (RootNode == null) { Debug.LogError("No root node was found in this graph! Please add one in order to play it."); return; }
            RootNode.OnEnter();
        }

        public RootNode FindRootNode() {
            foreach (Node node in nodes) {
                if (node is RootNode) return node as RootNode;
            }

            return null;
        }

        public void EndReached(object args) {
            if(OnFinished != null) OnFinished.Invoke(args);
        }

    }

    [Serializable]
    public class Flow { }

}