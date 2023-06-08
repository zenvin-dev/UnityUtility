using UnityEngine;
using UnityEditor;

namespace Zenvin.Util {
	[CustomPropertyDrawer (typeof (Point))]
	public class PointPropertyDrawer : PropertyDrawer {

		private static GUIContent posContent = null;
		private static GUIContent PosContent => posContent == null ? (posContent = EditorGUIUtility.IconContent ("AvatarPivot", "Position")) : posContent;
		private static GUIContent objContent = null;
		private static GUIContent ObjContent => objContent == null ? (objContent = EditorGUIUtility.IconContent ("GameObject Icon", "Object")) : objContent;

		private static GUIStyle btnStyle;
		private static GUIStyle BtnStyle => btnStyle == null ? btnStyle = new GUIStyle (GUI.skin.button) { padding = new RectOffset (1, 1, 1, 1) } : btnStyle;


		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			position = EditorGUI.PrefixLabel (position, label);

			var btn = new Rect (position);
			btn.width = EditorGUIUtility.singleLineHeight;

			var offset = btn.width + EditorGUIUtility.standardVerticalSpacing;
			position.x += offset;
			position.width -= offset;

			var modeProp = property.FindPropertyRelative ("useObj");

			if (GUI.Button (btn, modeProp.boolValue ? ObjContent : PosContent, BtnStyle)) {
				modeProp.boolValue ^= true;
			}
			EditorGUI.PropertyField (position, property.FindPropertyRelative (modeProp.boolValue ? "obj" : "pos"), GUIContent.none);

			property.serializedObject.ApplyModifiedProperties ();
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return EditorGUIUtility.singleLineHeight;
		}

	}
}