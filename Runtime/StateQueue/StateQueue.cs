using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.Util {
	/// <summary>
	/// Wrapper for a state variable that can be influenced by multiple parties at once.<br></br>
	/// Each of those parties is represented by an <see cref="IStateQueueSource{T}"/> instance.
	/// </summary>
	/// <typeparam name="T"> The type of value wrapped by the queue. </typeparam>
	[Serializable]
	public class StateQueue<T> : IStateQueue<T> {

		private readonly List<IStateQueueSource<T>> sources;
		private T currentValue;

		[SerializeField] private T defaultValue;

		/// <inheritdoc/>
		public event OnStateQueueChanged<T> ValueChanged;

		/// <inheritdoc/>
		public IStateQueueTarget<T> Target { get; set; }

		/// <summary>
		/// Comparer for source instances. Default equality will be used if this is left <see langword="null"/>.<br></br>
		/// Used during <see cref="AddSource(IStateQueueSource{T})"/> to make sure the same source does not get added multiple times.<br></br>
		/// Used during <see cref="RemoveSource(IStateQueueSource{T})"/> and <see cref="RemoveSources(IStateQueueSource{T})"/> to find source(s) to remove.
		/// </summary>
		public IEqualityComparer<IStateQueueSource<T>> SourceComparer { get; set; }

		/// <inheritdoc/>
		public T Default { get => defaultValue; set => SetDefault (value); }

		/// <inheritdoc/>
		public T Current { get => Count == 0 ? Default : currentValue; private set => currentValue = value; }

		/// <inheritdoc/>
		public int Count => sources.Count;


		public StateQueue (IStateQueueTarget<T> target) {
			sources = new List<IStateQueueSource<T>> ();
			Target = target;
		}

		public StateQueue (IStateQueueTarget<T> target, T @default) : this (target) {
			Default = @default;
			Current = @default;
		}


		/// <summary>
		/// Attempts to add a <see cref="IStateQueueSource{T}"/> to the queue.<br></br>
		/// Will fail if the source already was added previously, or the value is <see langword="null"/>.
		/// </summary>
		/// <param name="source"> The source to add. </param>
		/// <returns> Whether the source was added successfully. </returns>
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

		/// <summary>
		/// Attempts to remove a <see cref="IStateQueueSource{T}"/> from the queue.<br></br>
		/// Will fail if the source does not exist in the queue, or the value is <see langword="null"/>.
		/// </summary>
		/// <param name="source"> The source to remove. </param>
		/// <returns> Whether the source was removed successfully. </returns>
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

		/// <summary>
		/// Removes all instances matching the given <paramref name="source"/> from the queue.
		/// </summary>
		/// <param name="source"> The source to remove. </param>
		/// <returns> The number of sources removed. </returns>
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

		/// <inheritdoc/>
		public void ClearSources () {
			if (sources.Count == 0)
				return;

			for (int i = 0; i < sources.Count; i++) {
				if (sources[i] is IActiveStateQueueSource<T> activeSource) {
					activeSource.StateChanged -= Update;
				}
				i--;
			}
			sources.Clear ();
			Update ();
		}

		/// <inheritdoc/>
		public void Update () {
			var current = Default;
			for (int i = sources.Count - 1; i >= 0; i--) {
				if (sources[i] != null) {
					sources[i].Update (Target, ref current);
				}
			}
			if ((Current == null && current != null) || !Current.Equals (current)) {
				var args = new StateQueueChangedArgs<T> (this, Current, current);
				Target?.StateChanged (args);
				ValueChanged?.Invoke (args);
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
}
