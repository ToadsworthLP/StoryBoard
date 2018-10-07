using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace StoryBoard.Misc {

    [Serializable]
    public class SerializabeArgs {
        public string argumentName;

        public enum ArgType { Unsupported, Bool, Int, Float, String, Color, Vector2, Vector3, Object }
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public Color colorValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public UnityEngine.Object objectValue;
        public string valueTypeName;
        public ArgType argType;

        public object GetValue() {
            return GetValue(argType);
        }

        public object GetValue(ArgType type) {
            switch (type) {
                case ArgType.Bool:
                    return boolValue;
                case ArgType.Int:
                    return intValue;
                case ArgType.Float:
                    return floatValue;
                case ArgType.String:
                    return stringValue;
                case ArgType.Color:
                    return colorValue;
                case ArgType.Vector2:
                    return vector2Value;
                case ArgType.Vector3:
                    return vector3Value;
                case ArgType.Object:
                    return objectValue;
                default:
                    return null;
            }
        }

        public Type GetActualType() {
            switch (argType) {
                case ArgType.Bool:
                    return typeof(bool);
                case ArgType.Int:
                    return typeof(int);
                case ArgType.Float:
                    return typeof(float);
                case ArgType.String:
                    return typeof(string);
                case ArgType.Color:
                    return typeof(Color);
                case ArgType.Vector2:
                    return typeof(Vector2);
                case ArgType.Vector3:
                    return typeof(Vector3);
                case ArgType.Object:
                    return Type.GetType(valueTypeName);
                default:
                    return null;
            }
        }

#if UNITY_EDITOR
        public void DrawEditor() {
            switch (argType) {
                case ArgType.Bool:
                    boolValue = EditorGUILayout.Toggle(boolValue);
                    break;
                case ArgType.Int:
                    intValue = EditorGUILayout.IntField(intValue);
                    break;
                case ArgType.Float:
                    floatValue = EditorGUILayout.FloatField(floatValue);
                    break;
                case ArgType.String:
                    stringValue = EditorGUILayout.TextField(stringValue);
                    break;
                case ArgType.Color:
                    colorValue = EditorGUILayout.ColorField(colorValue);
                    break;
                case ArgType.Vector2:
                    vector2Value = EditorGUILayout.Vector2Field("", vector2Value);
                    break;
                case ArgType.Vector3:
                    vector3Value = EditorGUILayout.Vector3Field("", vector3Value);
                    break;
                case ArgType.Object:
                    objectValue = EditorGUILayout.ObjectField(objectValue, Type.GetType(valueTypeName), allowSceneObjects: false);
                    break;
            }
        }
#endif

        public static Type TypeOfArgType(ArgType argType) {
            switch (argType) {
                case ArgType.Bool:
                    return typeof(bool);
                case ArgType.Int:
                    return typeof(int);
                case ArgType.Float:
                    return typeof(float);
                case ArgType.String:
                    return typeof(string);
                case ArgType.Color:
                    return typeof(Color);
                case ArgType.Vector2:
                    return typeof(Vector2);
                case ArgType.Vector3:
                    return typeof(Vector3);
                case ArgType.Object:
                    return typeof(UnityEngine.Object);
                default:
                    return null;
            }
        }

        public static ArgType ArgTypeOfType(Type type) {
            if (type == typeof(bool)) return ArgType.Bool;
            else if (type == typeof(int)) return ArgType.Int;
            else if (type == typeof(float)) return ArgType.Float;
            else if (type == typeof(String)) return ArgType.String;
            else if (type == typeof(Color)) return ArgType.Color;
            else if (type == typeof(Vector2)) return ArgType.Vector2;
            else if (type == typeof(Vector3)) return ArgType.Vector3;
            else if (type == typeof(UnityEngine.Object) || type.IsSubclassOf(typeof(UnityEngine.Object))) return ArgType.Object;
            else return ArgType.Unsupported;
        }

        public static bool IsSupported(Type type) {
            return ArgTypeOfType(type) != ArgType.Unsupported;
        }
    }

}