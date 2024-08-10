using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil;

namespace Zenvin.Util {
	[CustomPropertyDrawer(typeof(InterfaceStateQueue<>))]
	[CustomPropertyDrawer (typeof (EventStateQueue<>))]
	public class StateQueuePropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			label = EditorGUI.BeginProperty (position, label, property);
			var prop = property.FindPropertyRelative ("defaultValue");
			if (prop == null) {
				return;
			}

			position = PropertyDrawerUtility.DrawInfo (
				position,
				"This field represents the default value of a StateQueue. Changing the value will NOT update the queue's current value on its own."
			);

			EditorGUI.PropertyField (position, prop, label);
			EditorGUI.EndProperty ();
		}

	}
}
