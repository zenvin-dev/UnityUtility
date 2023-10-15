using System;
using System.Collections;
using System.Collections.Generic;

namespace Zenvin.Util {
	/// <summary>
	/// A wrapper for <see cref="List{T}"/>, which automatically sorts the items inside it.<br></br>
	/// Accepts only <c>T</c> that implement <see cref="IComparable{T}"/>.
	/// </summary>
	public class OrderedList<T> : IList<T> where T : IComparable<T> {

		private readonly List<T> list;
		public readonly bool Reversed;

		public int Count => list.Count;
		public bool IsReadOnly => false;
		public int Capacity => list.Capacity;

		public T this[int index] {
			get => list[index];
			set => list[index] = value;
		}


		public OrderedList () {
			list = new List<T> ();
		}

		public OrderedList (int capacity) {
			list = new List<T> (capacity);
		}

		public OrderedList (IEnumerable<T> collection) {
			list = new List<T> (collection);
			list.Sort (Compare);
		}

		public OrderedList (bool reverse) : this () {
			Reversed = reverse;
		}

		public OrderedList (bool reverse, int capacity) : this (capacity) {
			Reversed = reverse;
		}


		public void Add (T item) {
			if (item == null) {
				return;
			}
			for (int i = 0; i < list.Count; i++) {
				var _item = list[i];
				if (Compare (_item, item) > 0) {
					list.Insert (i, item);
					return;
				}
			}
			list.Add (item);
		}

		public void Clear () {
			list.Clear ();
		}

		public bool Contains (T item) {
			return list.Contains (item);
		}

		public void CopyTo (T[] array, int arrayIndex) {
			list.CopyTo (array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator () {
			return list.GetEnumerator ();
		}

		public int IndexOf (T item) {
			return list.IndexOf (item);
		}

		public void Insert (int index, T item) {
			throw new InvalidOperationException ("Cannot insert into ordered list.");
		}

		public bool Remove (T item) {
			return list.Remove (item);
		}

		public void RemoveAt (int index) {
			list.RemoveAt (index);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return list.GetEnumerator ();
		}

		public void TrimExcess () {
			list.TrimExcess ();
		}


		private int Compare (T a, T b) {
			if (a == null && b == null) {
				return 0;
			}
			if (a == null) {
				return Reversed ? 1 : -1;
			}
			if (b == null) {
				return Reversed ? -1 : 1;
			}
			return Reversed ? -a.CompareTo (b) : a.CompareTo (b);
		}
	}
}