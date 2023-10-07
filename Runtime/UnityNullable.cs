using System;
using UnityEngine;

namespace Zenvin.Util {
	[Serializable]
	public struct UnityNullable<T> where T : struct {
		public static readonly UnityNullable<T> Null = new UnityNullable<T> () { hasValue = false, value = default };

		[SerializeField] private T value;
		[SerializeField] private bool hasValue;

		public readonly bool HasValue => hasValue;
		public readonly T Value => hasValue ? value : throw new NullReferenceException (nameof (value));


		public static implicit operator UnityNullable<T> (T? value) {
			return new UnityNullable<T> () {
				value = value.HasValue ? value.Value : default,
				hasValue = value.HasValue
			};
		}

		public static implicit operator T (UnityNullable<T> value) {
			return value.Value;
		}
	}
}