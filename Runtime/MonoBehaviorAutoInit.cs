using static System.Reflection.BindingFlags;
using static System.AttributeTargets;
using System.Reflection;
using UnityEngine;
using System;

namespace Zenvin.BaseClasses {
	public abstract class MonoBehaviorAutoInit : MonoBehavior {

		private void Awake () {
			InitComponentReferences ();
			OnAwake ();
		}

		/// <summary>
		/// Gets called on Awake(), after component references have been initialized.
		/// </summary>
		protected virtual void OnAwake () { }


		private void InitComponentReferences () {

			MemberInfo[] members = GetType ().GetMembers (DeclaredOnly | Public | NonPublic | Instance);    //get an array of all relevant class members
			foreach (MemberInfo m in members) {                                                             //iterate through the array

				AutoInitAttribute aia = m.GetCustomAttribute<AutoInitAttribute> ();                         //if the member does not have the [AutoInit] attribute, continue

				if (aia is null) {
					continue;
				}

				switch (m) {
					case FieldInfo field:                                                                   //if the member is a FIELD
						InitField (field, aia);                                                                 //init it as field
						break;
					case PropertyInfo property:                                                             //if the member is a PROPERTY
						InitProp (property, aia);                                                               //init it as property
						break;
				}
			}

		}

		private void InitField (FieldInfo field, AutoInitAttribute attr) {
			Type t = field.FieldType;                                                                       //get the type of the field

			bool array = false;
			if (t.IsArray) {                                                                                //check if field type is an array
				array = true;
				t = t.GetElementType ();                                                                        //get the underlying type
			}

			if (!t.IsSubclassOf (typeof (Component)) || (!array && field.GetValue (this) != null)) {        //if the field's type is not a Component or its value is not null, return
				return;
			}

			if (array) {
				Component[] comps = CustomGetComponents (t, attr.Type, attr.inactive);
				field.SetValue (this, Convert (t, comps));
				if (attr.Disable && (comps == null || comps.Length == 0)) {
					enabled = false;
					Debug.LogError ($"{GetType ().Name} on {name} was disabled because [AutoInit] on {field.Name} could not find references to {t}. Initialization will continue.", this);
				}
			} else {
				Component c = GetComponent (t);
				field.SetValue (this, c);
				if (c == null && attr.Disable) {
					enabled = false;
					Debug.LogError ($"{GetType ().Name} on {name} was disabled because [AutoInit] on {field.Name} could not find a reference to {t}. Initialization will continue.", this);
				}
			}
		}

		private void InitProp (PropertyInfo property, AutoInitAttribute attr) {
			Type t = property.PropertyType;                                                                 //get the type of the property

			bool array = false;
			if (t.IsArray) {                                                                                //check if property is an array
				array = true;
				t = t.GetElementType ();                                                                        //get the underlying type
			}

			if (!t.IsSubclassOf (typeof (Component)) || (!array && property.GetValue (this) != null)) {     //if the property's type is not a Component or its value is not null, return
				return;
			}

			if (array) {
				Component[] comps = CustomGetComponents (t, attr.Type, attr.inactive);
				property.SetValue (this, Convert (t, comps));												//set the property's value (array)
				if (attr.Disable && (comps == null || comps.Length == 0)) {
					enabled = false;
					Debug.LogError ($"{GetType ().Name} on {name} was disabled because [AutoInit] on {property.Name} could not find references to {t}. Initialization will continue.", this);
				}
			} else {
				Component c = GetComponent (t);
				property.SetValue (this, c);
				if (c == null && attr.Disable) {
					enabled = false;
					Debug.LogError ($"{GetType().Name} on {name} was disabled because [AutoInit] on {property.Name} could not find a reference to {t}. Initialization will continue.", this);
				}
			}
		}

		private Component[] CustomGetComponents (Type type, AutoInitAttribute.GetComponentType getType, bool inactive) {
			Component[] comps = null;

			switch (getType) {
				case AutoInitAttribute.GetComponentType.Normal:
					comps = GetComponents (type);
					break;
				case AutoInitAttribute.GetComponentType.Parent:
					comps = GetComponentsInParent (type, inactive);
					break;
				case AutoInitAttribute.GetComponentType.Children:
					comps = GetComponentsInChildren (type, inactive);
					break;
			}

			return comps;
		}

		private Array Convert (Type type, Component[] array) {
			Array arr = Array.CreateInstance (type, array.Length);
			Array.Copy (array, arr, array.Length);
			return arr;
		}

	}

	/// <summary>
	/// Use this attribute on any Component field or property in a class that inherits <see cref="MonoBehaviorAutoInit"/> to have the field or property automatically use GetComponent on Awake.
	/// </summary>
	[AttributeUsage (Field | Property)]
	public class AutoInitAttribute : Attribute {

		public enum GetComponentType {
			Normal,
			Parent,
			Children
		}

		/// <summary>
		/// When set to true, disables the Component that contains the member with the attribute if a reference is not found.
		/// </summary>
		public readonly bool Disable = false;

		/// <summary>
		/// Specifies where the AutoInitialization is supposed to look for component references.
		/// </summary>
		public readonly GetComponentType Type = GetComponentType.Normal;

		/// <summary>
		/// Determines whether the component search includes inactive components. Will only make a difference when <see cref="Type"/> is not set to <see cref="GetComponentType.Normal"/>.
		/// </summary>
		public readonly bool inactive = false;


		public AutoInitAttribute () { }

		/// <param name="disable">If set to true, the Behavior containing the field/property will be disabled if no Component was found to assign to the field/property.</param>
		public AutoInitAttribute (bool disable) { Disable = disable; }

		/// <param name="getComponentType">If the target field/property is an Array, determine which <see cref="Component.GetComponents(Type)"/> function should be used.</param>
		/// <param name="considerInactiveComponents">Determines whether inactive components should be found as well.</param>
		public AutoInitAttribute (GetComponentType getComponentType, bool considerInactiveComponents) {
			Type = getComponentType;
			inactive = considerInactiveComponents;
		}

	}
}