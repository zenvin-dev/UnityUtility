using System;
using UnityEngine;

namespace Zenvin.Util {
	[Serializable]
	public struct UNullable<T> where T : struct {
		[SerializeField] private T value;
		[SerializeField] private bool hasValue;

		public readonly bool HasValue => hasValue;
		public readonly T Value => hasValue ? value : throw new NullReferenceException (nameof (value));


		public static implicit operator UNullable<T> (T? value) {
			return new UNullable<T> () {
				value = value.HasValue ? value.Value : default,
				hasValue = value.HasValue
			};
		}

		public static implicit operator T (UNullable<T> value) {
			return value.Value;
		}
	}
}