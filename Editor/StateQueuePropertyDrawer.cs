using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil;

namespace Zenvin.Util {
	[CustomPropertyDrawer(typeof(StateQueue<>))]
	[CustomPropertyDrawer (typeof (EventBasedStateQueue<>))]
	public class StateQueuePropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			var prop = property.FindPropertyRelative ("defaultValue");
			if (prop == null) {
				return;
			}

			position = PropertyDrawerUtility.DrawInfo (
				position, "This field represents the default value of a StateQueue. It cannot be changed at runtime, and does not inform about the queue's current value."
			);

			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			EditorGUI.PropertyField (position, prop, label);
			EditorGUI.EndDisabledGroup ();
		}

	}
}
