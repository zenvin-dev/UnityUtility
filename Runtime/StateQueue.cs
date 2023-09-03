using System;
using System.Collections.Generic;

namespace Zenvin.Utility {
	public class StateQueue<T> {
		private readonly List<IStateQueueSource<T>> sources;
		private T defaultValue;

		public IStateQueueTarget<T> Target { get; set; }
		public IEqualityComparer<IStateQueueSource<T>> SourceComparer { get; set; }
		public T Default { get => defaultValue; set => SetDefault (value); }
		public T Current { get; private set; }
		public int Count => sources.Count;


		public StateQueue (IStateQueueTarget<T> target) {
			sources = new List<IStateQueueSource<T>> ();
			Target = target;
		}

		public StateQueue (IStateQueueTarget<T> target, T @default) : this (target) {
			Default = @default;
			Current = @default;
		}


		public bool AddSource (IStateQueueSource<T> source) {
			if (source == null) {
				return false;
			}

			bool added = false;
			for (int i = 0; i < sources.Count; i++) {
				if (Equal (source, sources[i])) {
					return false;
				}

				if (i == sources.Count - 1) {
					break;
				}

				if (sources[i + 1].StateQueueOrder < source.StateQueueOrder) {
					sources.Insert (i, source);
					added = true;
					break;
				}
			}
			if (!added) {
				sources.Add (source);
			}

			Update ();
			if (source is IActiveStateQueueSource<T> activeSource) {
				activeSource.StateChanged += Update;
			}
			return true;
		}

		public bool RemoveSource (IStateQueueSource<T> source) {
			if (source == null) {
				return true;
			}
			bool removed = false;
			for (int i = 0; i < sources.Count; i++) {
				if (!Equal (source, sources[i])) {
					continue;
				}

				if (sources[i] is IActiveStateQueueSource<T> activeSource) {
					activeSource.StateChanged -= Update;
				}

				sources.RemoveAt (i);
				removed = true;
				break;
			}
			if (removed) {
				Update ();
			}
			return removed;
		}

		public int RemoveSources (IStateQueueSource<T> source) {
			if (source == null) {
				return 0;
			}

			int removed = 0;

			for (int i = sources.Count - 1; i >= 0; i--) {
				if (!Equal(source, sources[i])) {
					continue;
				}

				if (sources[i] is IActiveStateQueueSource<T> activeSource) {
					activeSource.StateChanged -= Update;
				}

				sources.RemoveAt (i);
				removed++;
				i--;
			}

			if (removed > 0) {
				Update ();
			}

			return removed;
		}

		public void ClearSources () {
			for (int i = 0; i < sources.Count; i++) {
				if (sources[i] is IActiveStateQueueSource<T> activeSource) {
					activeSource.StateChanged -= Update;
				}
				i--;
			}
			sources.Clear ();
			Update ();
		}

		public void Update () {
			var current = Default;
			for (int i = sources.Count - 1; i >= 0; i--) {
				if (sources[i] != null) {
					sources[i].Update (Target, ref current);
				}
			}
			if ((Current == null && current != null) || !Current.Equals (current)) {
				Target?.StateChanged (new StateQueueChangedArgs<T>(this, Current, current));
				Current = current;
			}
		}


		public override string ToString () {
			if (Current == null) {
				return "";
			}
			return Current.ToString ();
		}

		public string ToString (bool includeStateHint) {
			return includeStateHint ? $"StateQueue<{typeof (T).FullName}>: {ToString ()}" : ToString ();
		}


		private void SetDefault (T value) {
			if ((value == null && defaultValue == null) || (value != null && value.Equals (defaultValue))) {
				return;
			}
			defaultValue = value;
			Update ();
		}

		private bool Equal (IStateQueueSource<T> x, IStateQueueSource<T> y) {
			if (SourceComparer != null) {
				return SourceComparer.Equals (x, y);
			}
			return x == y;
		}


		public static implicit operator T (StateQueue<T> queue) {
			if (queue == null) {
				return default;
			}
			return queue.Current;
		}
	}

	public interface IStateQueueTarget<T> {
		void StateChanged (StateQueueChangedArgs<T> args);
	}

	public interface IStateQueueSource<T> {
		int StateQueueOrder { get; }
		void Update (IStateQueueTarget<T> target, ref T state);
	}

	public interface IActiveStateQueueSource<T> : IStateQueueSource<T> {
		event Action StateChanged;
	}

	public class StateQueueChangedArgs<T> {
		public readonly StateQueue<T> Origin;
		public readonly T Previous;
		public readonly T Current;


		private StateQueueChangedArgs () { }

		internal StateQueueChangedArgs (StateQueue<T> origin, T previous, T current) {
			Origin = origin;
			Previous = previous;
			Current = current;
		}
	}
}
