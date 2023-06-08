using System.Collections.Generic;

namespace Zenvin.Util {
	internal static class TableUtility {

		public static int ForceListLength<T> (this List<T> list, int length, T @default = default) {
			int diff = 0;

			if (list == null) {
				return diff;
			}

			if (list.Capacity < length) {
				list.Capacity = length;
			}
			while (list.Count < length) {
				list.Add (@default);
				diff++;
			}

			while (list.Count > length) {
				list.RemoveAt (list.Count - 1);
				diff--;
			}

			return diff;
		}

	}
}