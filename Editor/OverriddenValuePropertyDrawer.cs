using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil;

namespace Zenvin.Util.Presets {
	[CustomPropertyDrawer (typeof (IOverriddenValue<>), true)]
	public class OverriddenValuePropertyDrawer : PropertyDrawer {

		private static readonly Rect[] parts = new Rect[2];


		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			label = EditorGUI.BeginProperty (position, label, property);

			var valueProp = property.FindPropertyRelative ("value");
			var presetProp = property.FindPropertyRelative ("preset");

			position = EditorGUI.PrefixLabel (position, label);
			position.SplitNonAlloc (RectTransform.Axis.Horizontal, parts, EditorGUIUtility.standardVerticalSpacing);

			EditorGUI.PropertyField (parts[0], valueProp, GUIContent.none);
			EditorGUI.PropertyField (parts[1], presetProp, GUIContent.none);

			property.serializedObject.ApplyModifiedProperties ();
			EditorGUI.EndProperty ();
		}
	}
}
