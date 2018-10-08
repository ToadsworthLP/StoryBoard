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
            List<ExposedPropertyBase> exposedPropertyNodes = GetExposedPropertyNodes();
            foreach (ExposedPropertyBase node in exposedPropertyNodes) {
                SerializabeArgs args = exposedProperties[Array.IndexOf(exposedPropertyNodeNames, node.name)];
                node.SetValue(args.GetValue());
            }

            storyboard.Play(OnFinished);
        }

        public List<ExposedPropertyBase> GetExposedPropertyNodes() {
            return storyboard.nodes.FindAll(node => node is ExposedPropertyBase).ConvertAll(node => (ExposedPropertyBase)node);
        }

    }

}