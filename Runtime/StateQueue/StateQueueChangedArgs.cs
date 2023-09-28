namespace Zenvin.Utility {
	public class StateQueueChangedArgs<T> {
		public readonly IStateQueue<T> Origin;
		public readonly T Previous;
		public readonly T Current;


		private StateQueueChangedArgs () { }

		internal StateQueueChangedArgs (IStateQueue<T> origin, T previous, T current) {
			Origin = origin;
			Previous = previous;
			Current = current;
		}
	}
}
