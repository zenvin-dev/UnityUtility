namespace Zenvin.Util {
	/// <summary>
	/// A wrapper for ordering a <see cref="ProcessQueueState{T}"/> delegate inside a <see cref="IStateQueue{T}"/>.
	/// </summary>
	/// <typeparam name="T"> The type of value processed by the entry. </typeparam>
	public struct EventQueueEntry<T> {
		internal uint Handle;
		internal readonly ProcessQueueState<T> Callback;

		/// <summary> Value used for sorting entries. </summary>
		public readonly int Order;
		/// <summary> Optionally gives information about the origin of an entry.  </summary>
		/// <remarks> Can be used, for example, to describe an entry on UI. Or to remove all entries with a common origin. </remarks> 
		public readonly object Origin;

		/// <summary>
		/// Creates a new <see cref="EventQueueEntry{T}"/> from the given callback, with an order value of 0.
		/// </summary>
		public EventQueueEntry (ProcessQueueState<T> callback) : this (0, callback) { }

		/// <summary>
		/// Creates a new <see cref="EventQueueEntry{T}"/> from the given callback and order.
		/// </summary>
		public EventQueueEntry (int order, ProcessQueueState<T> callback) : this (callback, order, null) { }

		/// <summary>
		/// Creates a new <see cref="EventQueueEntry{T}"/> from the given callback, order and origin.
		/// </summary>
		public EventQueueEntry (ProcessQueueState<T> callback, int order, object origin) : this () {
			Origin = origin;
			Order = order;
			Callback = callback;
		}


		/// <summary>
		/// Creates a new <see cref="EventQueueEntry{T}"/> given a tuple of a delegate for processing, and an order value.
		/// </summary>
		/// <param name="data"></param>
		public static implicit operator EventQueueEntry<T> ((int order, ProcessQueueState<T> callback) data) {
			return new EventQueueEntry<T> (data.order, data.callback);
		}
	}
}
