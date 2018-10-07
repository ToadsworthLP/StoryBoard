using UnityEngine;

namespace StoryBoardEditor {

    public static class StoryboardResources {

        private static bool isImportantLabelStyleInitialized = false;
        private static GUIStyle importantLabel;

        public static GUIStyle WarningLabel() {
            if (isImportantLabelStyleInitialized) {
                return importantLabel;
            }

            importantLabel = new GUIStyle();
            importantLabel.alignment = TextAnchor.UpperLeft;
            importantLabel.fontStyle = FontStyle.Bold;
            importantLabel.normal.textColor = Color.yellow;

            isImportantLabelStyleInitialized = true;
            return importantLabel;
        }

    }

}
