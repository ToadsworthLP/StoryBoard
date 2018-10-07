using StoryBoard.Nodes;
using StoryBoard.Misc;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StoryBoard {

    public class StoryboardPlayer : MonoBehaviour {

        public StoryboardGraph storyboard;

        [HideInInspector]
        [SerializeField]
        public string[] sceneObjectNodeNames;
        [HideInInspector]
        [SerializeField]
        public UnityEngine.Object[] sceneObjects;

        [HideInInspector]
        [SerializeField]
        public string[] exposedPropertyNodeNames;
        [HideInInspector]
        [SerializeField]
        public SerializabeArgs[] exposedProperties;


        [ContextMenu("Force Playback")]
        public void ForcePlay() { Play(); }

        public void Play(Action<object> OnFinished = null) {
            List<SceneObjectNode> sceneObjectNodes = GetSceneObjectNodes();
            foreach (SceneObjectNode node in sceneObjectNodes) {
                UnityEngine.Object obj = sceneObjects[Array.IndexOf(sceneObjectNodeNames, node.name)];
                node.Object.SetObject(obj);
            }

            List<ExposedPropertyBase> exposedPropertyNodes = GetExposedPropertyNodes();
            foreach (ExposedPropertyBase node in exposedPropertyNodes) {
                SerializabeArgs args = exposedProperties[Array.IndexOf(exposedPropertyNodeNames, node.name)];
                node.SetValue(args.GetValue());
            }

            storyboard.Play(OnFinished);
        }

        public List<SceneObjectNode> GetSceneObjectNodes() {
            return storyboard.nodes.FindAll(node => node is SceneObjectNode).ConvertAll(node => (SceneObjectNode)node);
        }

        public List<ExposedPropertyBase> GetExposedPropertyNodes() {
            return storyboard.nodes.FindAll(node => node is ExposedPropertyBase).ConvertAll(node => (ExposedPropertyBase)node);
        }

    }

}