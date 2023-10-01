using System;
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
	public class EventBasedStateQueue<T> : IStateQueue<T> {

		private readonly List<EventQueueEntry<T>> entries = new List<EventQueueEntry<T>> ();
		private uint lastHandle = 1;
		private T currentValue;

		[SerializeField] private T defaultValue;

		/// <inheritdoc/>
		public event OnStateQueueChanged<T> ValueChanged;

		/// <inheritdoc/>
		public IStateQueueTarget<T> Target { get; set; }
		/// <inheritdoc/>
		public T Default { get => defaultValue; set => SetDefault (value); }
		/// <inheritdoc/>
		public T Current { get => Count == 0 ? Default : currentValue; private set => currentValue = value; }
		/// <inheritdoc/>
		public int Count => entries.Count;


		public bool AddSource (EventQueueEntry<T> entry, out uint handle) {
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

			Update ();
			return true;
		}

		public bool RemoveSource (uint handle) {
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Handle == handle) {
					entries.RemoveAt (i);
					Update ();
					return true;
				}
			}
			return false;
		}

		/// <inheritdoc/>
		public void ClearSources () {
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


		private void SetDefault (T value) {
			if ((value == null && defaultValue == null) || (value != null && value.Equals (defaultValue))) {
				return;
			}
			defaultValue = value;
			Update ();
		}

	}

	public struct EventQueueEntry<T> {
		internal uint Handle;

		public readonly int Order;
		public readonly ProcessQueueState<T> Callback;


		public EventQueueEntry (ProcessQueueState<T> callback) : this (0, callback) { }

		public EventQueueEntry (int order, ProcessQueueState<T> callback) : this () {
			Order = order;
			Callback = callback;
		}


		public static implicit operator EventQueueEntry<T> ((int order, ProcessQueueState<T> callback) data) {
			return new EventQueueEntry<T> (data.order, data.callback);
		}
	}
}