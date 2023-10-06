using UnityEditor;
using UnityEngine;
using System;

namespace Zenvin.Util {
	[CustomPropertyDrawer (typeof (UnityGuid))]
	public class UnityGuidPropertyDrawer : PropertyDrawer {

		private static readonly byte[] bytes = new byte[16];

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			property = property.FindPropertyRelative ("id");
			if (property == null) {
				return;
			}

			position = EditorGUI.PrefixLabel (position, label);
			if (!property.isArray || property.arraySize != bytes.Length) {	
				EditorGUI.LabelField (position, "Empty", EditorStyles.helpBox);
				return;
			}

			for (int i = 0; i < bytes.Length; i++) {
				bytes[i] = (byte)property.GetArrayElementAtIndex (i).intValue;
			}

			EditorGUI.LabelField (position, new UnityGuid(bytes).ToString(), EditorStyles.helpBox);
		}

	}
}