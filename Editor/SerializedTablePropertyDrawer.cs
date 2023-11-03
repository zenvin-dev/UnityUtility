using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil;

namespace Zenvin.Util {
	[CustomPropertyDrawer (typeof (SerializedTable<,,>))]
	public class SerializedTablePropertyDrawer : PropertyDrawer {

		private static GUIContent openTableContent;
		private static GUIContent OpenTableContent {
			get {
				if (openTableContent == null) {
					openTableContent = EditorGUIUtility.IconContent ("d_Grid.Default");
					openTableContent.tooltip = "Open Table in Editor.";
				}
				return openTableContent;
			}
		}
		private static GUIStyle openTableStyle;
		private static GUIStyle OpenTableStyle {
			get {
				return openTableStyle == null ? openTableStyle = new GUIStyle (GUI.skin.button) { padding = new RectOffset (2, 2, 2, 2) } : openTableStyle;
			}
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			position = EditorGUI.PrefixLabel (position, label);
			position = position.Inset (RectTransform.Edge.Right, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, out Rect btnPos);

			var colProp = property.FindPropertyRelative ("columns");
			var rowProp = property.FindPropertyRelative ("rows");

			if(GUI.Button (position, $"Colums: {colProp.arraySize} | Rows: {rowProp.arraySize}", EditorStyles.label) || GUI.Button (btnPos, OpenTableContent, OpenTableStyle)) {
				SerializedTableEditorWindow.Open (property, label);
			}
		}

	}
}