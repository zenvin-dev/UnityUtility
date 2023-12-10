using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil;

namespace Zenvin.Util.Presets {
	[CustomPropertyDrawer (typeof (IOverriddenValue<>), true)]
	public class OverriddenValuePropertyDrawer : PropertyDrawer {

		private static readonly Rect[] parts = new Rect[2];

		private static GUIContent showContent;
		private static GUIContent ShowContent {
			get {
				if (showContent == null) {
					showContent = EditorGUIUtility.IconContent ("d_animationvisibilitytoggleon");
					showContent.tooltip = "Pre-viewing preset value.";
				}
				return showContent;
			}
		}
		private static GUIContent hideContent;
		private static GUIContent HideContent {
			get {
				if (hideContent == null) {
					hideContent = EditorGUIUtility.IconContent ("d_animationvisibilitytoggleoff");
					hideContent.tooltip = "Pre-viewing default value.";
				}
				return hideContent;
			}
		}

		private SerializedObject presetObject = null;
		private bool preview = true;


		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			label = EditorGUI.BeginProperty (position, label, property);

			var valueProp = property.FindPropertyRelative ("value");
			var presetProp = property.FindPropertyRelative ("preset");
			UpdatePreset (presetProp.objectReferenceValue);

			var hasPreset = presetProp.objectReferenceValue != null;
			var presetValueProp = presetObject?.FindProperty ("value");

			position = EditorGUI.PrefixLabel (position, label);
			position.SplitNonAlloc (RectTransform.Axis.Horizontal, parts, EditorGUIUtility.standardVerticalSpacing);

			DrawValueProperty (valueProp, parts[0], hasPreset, presetValueProp);
			DrawPresetProperty (presetProp, parts[1], hasPreset);

			property.serializedObject.ApplyModifiedProperties ();
			EditorGUI.EndProperty ();
		}

		private void DrawValueProperty (SerializedProperty prop, Rect position, bool hasPreset, SerializedProperty presetValueProp) {
			if (hasPreset && preview) {
				GUI.enabled = false;
				EditorGUI.PropertyField (position, presetValueProp, GUIContent.none);
				GUI.enabled = true;
				return;
			}
			EditorGUI.PropertyField (position, prop, GUIContent.none);
		}

		private void DrawPresetProperty (SerializedProperty prop, Rect position, bool hasPreset) {
			if (hasPreset) {
				var spacing = EditorGUIUtility.standardVerticalSpacing;
				position = position.Inset (RectTransform.Edge.Right, EditorGUIUtility.singleLineHeight + spacing, out var inset);
				inset.x += spacing;
				inset.width -= spacing;
				if (GUI.Button(inset, preview ? ShowContent : HideContent, EditorStyles.label)) {
					preview ^= true;
				}
			}
			EditorGUI.PropertyField (position, prop, GUIContent.none);
		}

		private void UpdatePreset (Object target) {
			if (target == null) {
				presetObject = null;
				return;
			}
			if (presetObject != null && presetObject.targetObject == target) {
				return;
			}
			presetObject = new SerializedObject (target);
		}
	}
}
