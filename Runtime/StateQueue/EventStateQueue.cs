using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.Util {
	public delegate void ProcessQueueState<T> (IStateQueueTarget<T> target, ref T value);

	/// <summary>
	/// Wrapper for a state variable that can be influenced by multiple parties at once.<br></br>
	/// Each of those parties is represented by an <see cref="EventQueueEntry{T}"/>.
	/// </summary>
	/// <typeparam name="T"> The type of value wrapped by the queue. </typeparam>
	[Serializable]
	public class EventStateQueue<T> : IStateQueue<T> {

		private readonly List<EventQueueEntry<T>> entries = new List<EventQueueEntry<T>> ();
		private uint lastHandle = 1;
		private T currentValue;

		[SerializeField] private T defaultValue;

		/// <inheritdoc/>
		public event OnStateQueueChanged<T> ValueChanged;

		/// <inheritdoc/>
		public IStateQueueTarget<T> Target { get; set; }
		/// <inheritdoc/>
		public T Default { get => defaultValue; set => defaultValue = value; }
		/// <inheritdoc/>
		public T Current { get => Count == 0 ? Default : currentValue; private set => currentValue = value; }
		/// <inheritdoc/>
		public int Count => entries.Count;

		/// <summary>
		/// Returns the <see cref="EventQueueEntry{T}"/> at the given index, or <see langword="null"/> if the index was out of range.
		/// </summary>
		public EventQueueEntry<T>? this[int index] => index < 0 || index >= Count ? (EventQueueEntry<T>?)null : entries[index];


		public EventStateQueue () { }

		public EventStateQueue (IStateQueueTarget<T> target) {
			Target = target;
		}

		public EventStateQueue (IStateQueueTarget<T> target, T @default) : this (target) {
			Default = @default;
			Current = @default;
		}

		public EventStateQueue (T @default) {
			Default = @default;
			Current = @default;
		}


		/// <summary>
		/// Attempts to add an <see cref="EventQueueEntry{T}"/> to the queue.<br></br>
		/// Will fail if the entry already was added previously, or the value's callback is <see langword="null"/>.
		/// </summary>
		/// <param name="entry"> The entry to add. </param>
		/// <param name="suppressUpdate"> If true, the queue's current value will not automatically be updated. Useful if multiple manipulators are to be added. </param>
		/// <returns> Whether the entry was added successfully. </returns>
		public bool AddManipulator (EventQueueEntry<T> entry, out uint handle) {
			return AddManipulator (entry, out handle, false);
		}

		/// <summary>
		/// Attempts to add an <see cref="EventQueueEntry{T}"/> to the queue.<br></br>
		/// Will fail if the entry already was added previously, or the value's callback is <see langword="null"/>.
		/// </summary>
		/// <param name="entry"> The entry to add. </param>
		/// <param name="handle"> The handle under which the entry was added. Must be used to remove the entry. </param>
		/// <param name="suppressUpdate"> If true, the queue's current value will not automatically be updated. Useful if multiple entries are to be added. </param>
		/// <returns> Whether the entry was added successfully. </returns>
		public bool AddManipulator (EventQueueEntry<T> entry, out uint handle, bool suppressUpdate) {
			if (entry.Callback == null) {
				handle = 0;
				return false;
			}

			handle = lastHandle;
			entry.Handle = handle;
			lastHandle++;

			bool added = false;
			for (int i = 0; i < entries.Count; i++) {
				if (i == entries.Count - 1) {
					break;
				}

				if (entries[i + 1].Order < entry.Order) {
					entries.Insert (i, entry);
					added = true;
					break;
				}
			}
			if (!added) {
				entries.Add (entry);
			}

			if (!suppressUpdate) {
				Update ();
			}
			return true;
		}

		/// <summary>
		/// Attempts to remove an <see cref="EventQueueEntry{T}"/> from the queue by its handle.<br></br>
		/// Will fail if the entry's handle does not exist in the queue.
		/// </summary>
		/// <param name="handle"> The handle of the entry to remove. </param>.
		/// <returns> Whether the entry was removed successfully. </returns>
		public bool RemoveManipulator (uint handle) {
			return RemoveManipulator (handle, false);
		}

		/// <summary>
		/// Attempts to remove an <see cref="EventQueueEntry{T}"/> from the queue by its handle.<br></br>
		/// Will fail if the entry's handle does not exist in the queue.
		/// </summary>
		/// <param name="handle"> The handle of the entry to remove. </param>.
		/// <param name="suppressUpdate"> If true, the queue's current value will not automatically be updated. Useful if multiple entries are to be removed. </param>
		/// <returns> Whether the entry was removed successfully. </returns>
		public bool RemoveManipulator (uint handle, bool suppressUpdate) {
			if (handle == 0) {
				return true;
			}

			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Handle == handle) {
					entries.RemoveAt (i);
					if (!suppressUpdate) {
						Update ();
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Removes all entries whose <see cref="EventQueueEntry{T}.Origin"/> matches the given <paramref name="origin"/>, using the default comparer.<br></br>
		/// If any entries were removed, the queue's current value will be updated.
		/// </summary>
		/// <param name="origin"> The origin to look for in entries. </param>
		/// <returns> The number of removed entries. </returns>
		public int RemoveManipulatorsByOrigin (object origin) {
			return RemoveManipulatorsByOrigin (origin, null);
		}

		/// <summary>
		/// Removes all entries whose <see cref="EventQueueEntry{T}.Origin"/> matches the given <paramref name="origin"/>, using a given <paramref name="comparer"/>,
		/// or the default comparer if <see langword="null"/> is passed.<br></br>
		/// If any entries were removed, the queue's current value will be updated.
		/// </summary>
		/// <param name="origin"> The origin to look for in entries. </param>
		/// <param name="comparer"> The comparer to use to equate entries' origins with the one given though <paramref name="origin"/>. </param>
		/// <returns> The number of removed entries. </returns>
		public int RemoveManipulatorsByOrigin (object origin, IEqualityComparer comparer) {
			int removed = 0;
			for (int i = Count - 1; i >= 0; i--) {
				var entryOrigin = entries[i].Origin;
				if (comparer != null && !comparer.Equals (entryOrigin, origin))
					continue;
				if (comparer == null && !CompareEquality (entryOrigin, origin))
					continue;

				entries.RemoveAt (i);
				removed++;
			}

			if (removed > 0)
				Update ();

			return removed;
		}

		/// <inheritdoc/>
		public void ClearManipulators () {
			if (entries.Count > 0) {
				entries.Clear ();
				Update ();
			}
		}

		/// <inheritdoc/>
		public void Update () {
			var current = Default;
			for (int i = entries.Count - 1; i >= 0; i--) {
				entries[i].Callback?.Invoke (Target, ref current);
			}
			if ((Current == null && current != null) || !Current.Equals (current)) {
				var args = new StateQueueChangedArgs<T> (this, Current, current);
				Target?.StateChanged (args);
				ValueChanged?.Invoke (args);
				Current = current;
			}
		}

		/// <inheritdoc/>
		public void SetDefault (T value) {
			if ((value == null && defaultValue == null) || (value != null && value.Equals (defaultValue))) {
				return;
			}
			defaultValue = value;
			Update ();
		}


		private static bool CompareEquality (object a, object b) {
			if (a == null && b == null)
				return true;

			if (a != null)
				return a.Equals (b);
			if (b != null)
				return b.Equals (a);

			return false;
		}
	}
}
