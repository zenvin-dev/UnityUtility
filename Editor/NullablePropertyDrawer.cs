using UnityEditor;
using UnityEngine;

namespace Zenvin.Util {
	[CustomPropertyDrawer (typeof (UnityNullable<>))]
	public class NullablePropertyDrawer : PropertyDrawer {

		private static GUIContent addContent;
		private static GUIContent remContent;
		private static GUIStyle btnStyle;

		private static GUIContent AddContent => addContent == null ? (addContent = EditorGUIUtility.IconContent ("d_Toolbar Plus@2x")) : addContent;
		private static GUIContent RemContent => remContent == null ? (remContent = EditorGUIUtility.IconContent ("CrossIcon")) : remContent;
		private static GUIStyle BtnStyle => btnStyle == null ? (btnStyle = new GUIStyle (GUI.skin.button) { padding = new RectOffset () }) : btnStyle;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			var valueProp = property.FindPropertyRelative ("value");
			var hasValueProp = property.FindPropertyRelative ("hasValue");

			position = EditorGUI.PrefixLabel (position, label);

			var fieldRect = new Rect (position);
			fieldRect.width -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			var btnRect = new Rect (fieldRect);
			btnRect.x += btnRect.width + EditorGUIUtility.standardVerticalSpacing;
			btnRect.width = EditorGUIUtility.singleLineHeight;

			// Draw empty label to prevent PrefixLabel from being disabled with succeeding PropertyField
			EditorGUI.LabelField (fieldRect, GUIContent.none, GUIStyle.none);

			EditorGUI.BeginDisabledGroup (!hasValueProp.boolValue);
			EditorGUI.PropertyField (fieldRect, valueProp, GUIContent.none);
			EditorGUI.EndDisabledGroup ();

			if (GUI.Button (btnRect, hasValueProp.boolValue ? RemContent : AddContent, BtnStyle)) {
				hasValueProp.boolValue ^= true;
			}

			property.serializedObject.ApplyModifiedProperties ();
		}

	}
}