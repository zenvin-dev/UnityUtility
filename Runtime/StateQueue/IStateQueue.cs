namespace Zenvin.Util {
	/// <summary>
	/// Event type for <see cref="IStateQueue{T}"/> value changes.
	/// </summary>
	/// <typeparam name="T">The type of value represented by the queue.</typeparam>
	/// <param name="args">Information about the change that occurred.</param>
	public delegate void OnStateQueueChanged<T> (StateQueueChangedArgs<T> args);

	/// <summary>
	/// An interface common to all variations of a state queue.
	/// </summary>
	/// <typeparam name="T">The type of value represented by the queue.</typeparam>
	public interface IStateQueue<T> {
		/// <summary>
		/// An event that is invoked every time the queue's current value is changed.<br></br>
		/// Should be called after <see cref="Update"/>.
		/// </summary>
		public event OnStateQueueChanged<T> ValueChanged;

		/// <summary>
		/// A target that gets passed to manipulators during value changes, and also gets notified when the queue's value changes post update.<br></br>
		/// Intended to be set to the object holding the queue instance. May be <see langword="null"/>.
		/// </summary>
		IStateQueueTarget<T> Target { get; set; }
		/// <summary>
		/// The default value of the queue. Will be used as a starting point each time the queue's current value is updated. <br></br>
		/// <b>Changing this will NOT automatically update the <see cref="Current"/> value.</b> 
		/// To automatically update the <see cref="Current"/> value after setting a new <see cref="Default"/>, use <see cref="SetDefault(T)"/>.
		/// </summary>
		/// <seealso cref="Update"/>
		T Default { get; set; }
		/// <summary>
		/// The current value of the queue.
		/// </summary>
		T Current { get; }
		/// <summary>
		/// The number of manipulators influencing the queue's value.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Updates the queue's current value, based on all manipulators influencing the queue.
		/// </summary>
		void Update ();
		/// <summary>
		/// Removes all influencing manipulators from the queue and resets its current value to the queue's default.
		/// </summary>
		void ClearManipulators ();
		/// <summary>
		/// Sets the queue's default value and updates its current value, in case the default value changed.
		/// </summary>
		/// <param name="value"> The new default value. </param>
		void SetDefault (T value);
	}
}
