using UnityEngine;

namespace Zenvin.Util.Presets {
	public abstract class ValuePreset<T> : ScriptableObject {
		[SerializeField] private T value;

		public T Value { get => value; set => this.value = value; }
	}
}
