namespace Zenvin.Util.Presets {
	public interface IOverriddenValue<T> {
		/// <summary> The current value of the object. Should be the preset's value, unless <see cref="UseValue"/> is <see langword="true"/>. </summary>
		T Value { get; }
		/// <summary> The preset overriding the object's <see cref="Value"/>. </summary>
		ValuePreset<T> Preset { get; set; }
		/// <summary> Whether to use the object's value instead of the preset, even if one is assigned. </summary>
		bool UseValue { get; set; }
	}
}
