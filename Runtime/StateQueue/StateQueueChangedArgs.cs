namespace Zenvin.Util {
	public class StateQueueChangedArgs<T> {
		/// <summary>
		/// The queue that triggered the event.
		/// </summary>
		public readonly IStateQueue<T> Origin;
		/// <summary>
		/// The value that the queue had prior to the change that triggered the event.
		/// </summary>
		public readonly T Previous;
		/// <summary>
		/// The value that the queue has now.
		/// </summary>
		public readonly T Current;


		private StateQueueChangedArgs () { }

		internal StateQueueChangedArgs (IStateQueue<T> origin, T previous, T current) {
			Origin = origin;
			Previous = previous;
			Current = current;
		}
	}
}
