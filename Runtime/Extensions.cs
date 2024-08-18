using UnityEngine;

namespace Zenvin.Util {
	public static class Extensions {
		public static T OrNull<T> (this T obj) where T : Object {
			if (obj == null)
				return null;

			return obj;
		}
	}
}
