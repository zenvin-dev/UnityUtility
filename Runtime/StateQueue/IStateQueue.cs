namespace Zenvin.Util {
	/// <summary>
	/// Event handler for <see cref="IStateQueue{T}"/> value changes.
	/// </summary>
	/// <typeparam name="T">The type of value represented by the queue.</typeparam>
	/// <param name="args">Information about the change that occurred.</param>
	public delegate void StateQueueChangedHandler<T> (StateQueueChangedArgs<T> args);

	/// <summary>
	/// An interface common to all variations of a state queue.
	/// </summary>
	/// <typeparam name="T">The type of value represented by the queue.</typeparam>
	public interface IStateQueue<T> {
		/// <summary>
		/// An event that is invoked every time the queue's value is changed.
		/// </summary>
		public event StateQueueChangedHandler<T> ValueChanged;

		/// <summary>
		/// A target that gets passed to sources during value changes, and also gets notified when the queue's value changes post update.<br></br>
		/// Intended to be set to the object holding the queue instance. May be <see langword="null"/>.
		/// </summary>
		IStateQueueTarget<T> Target { get; set; }
		/// <summary>
		/// The default value of the queue. Will be used as a starting point each time the queue's influencing parties change.
		/// </summary>
		T Default { get; set; }
		/// <summary>
		/// The current value of the queue.
		/// </summary>
		T Current { get; }
		/// <summary>
		/// The number of parties influencing the queue's value.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Updates the queue's current value, based on all parties influencing the queue.
		/// </summary>
		void Update ();
	}
}