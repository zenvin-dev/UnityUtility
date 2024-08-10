using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.Util {
	/// <summary>
	/// Wrapper for a state variable that can be influenced by multiple parties at once.<br></br>
	/// Each of those parties is represented by an <see cref="IStateQueueManipulator{T}"/> instance.
	/// </summary>
	/// <typeparam name="T"> The type of value wrapped by the queue. </typeparam>
	[Serializable]
	public class InterfaceStateQueue<T> : IStateQueue<T> {

		private readonly List<IStateQueueManipulator<T>> manipulators = new List<IStateQueueManipulator<T>>();
		private T currentValue;

		[SerializeField] private T defaultValue;

		/// <inheritdoc/>
		public event OnStateQueueChanged<T> ValueChanged;

		/// <inheritdoc/>
		public IStateQueueTarget<T> Target { get; set; }

		/// <summary>
		/// Comparer for manipulator instances. Default equality will be used if this is left <see langword="null"/>.<br></br>
		/// Used during <see cref="AddManipulator(IStateQueueManipulator{T})"/> to make sure the same manipulator does not get added multiple times.<br></br>
		/// Used during <see cref="RemoveManipulator(IStateQueueManipulator{T})"/> and <see cref="RemoveManipulators(IStateQueueManipulator{T})"/> to find manipulator(s) to remove.
		/// </summary>
		public IEqualityComparer<IStateQueueManipulator<T>> ManipulatorComparer { get; set; }

		/// <inheritdoc/>
		public T Default { get => defaultValue; set => defaultValue = value; }

		/// <inheritdoc/>
		public T Current { get => Count == 0 ? Default : currentValue; private set => currentValue = value; }

		/// <inheritdoc/>
		public int Count => manipulators.Count;


		public InterfaceStateQueue () {
			
		}

		public InterfaceStateQueue (IStateQueueTarget<T> target) {
			Target = target;
		}

		public InterfaceStateQueue (IStateQueueTarget<T> target, T @default) : this (target) {
			Default = @default;
			Current = @default;
		}

		public InterfaceStateQueue (T @default) {
			Default = @default;
			Current = @default;
		}


		/// <summary>
		/// Attempts to add a <see cref="IStateQueueManipulator{T}"/> to the queue.<br></br>
		/// Will fail if the manipulator already was added previously, or the value is <see langword="null"/>.
		/// </summary>
		/// <param name="manipulator"> The manipulator to add. </param>
		/// <returns> Whether the manipulator was added successfully. </returns>
		public bool AddManipulator (IStateQueueManipulator<T> manipulator) {
			return AddManipulator (manipulator, false);
		}

		/// <summary>
		/// Attempts to add a <see cref="IStateQueueManipulator{T}"/> to the queue.<br></br>
		/// Will fail if the manipulator already was added previously, or the value is <see langword="null"/>.
		/// </summary>
		/// <param name="manipulator"> The manipulator to add. </param>
		/// <param name="suppressUpdate"> If true, the queue's current value will not automatically be updated. Useful if multiple manipulators are to be added. </param>
		/// <returns> Whether the manipulator was added successfully. </returns>
		public bool AddManipulator (IStateQueueManipulator<T> manipulator, bool suppressUpdate) {
			if (manipulator == null) {
				return false;
			}

			bool added = false;
			for (int i = 0; i < manipulators.Count; i++) {
				if (Equal (manipulator, manipulators[i])) {
					return false;
				}

				if (i == manipulators.Count - 1) {
					break;
				}

				if (manipulators[i + 1].StateQueueOrder < manipulator.StateQueueOrder) {
					manipulators.Insert (i, manipulator);
					added = true;
					break;
				}
			}
			if (!added) {
				manipulators.Add (manipulator);
			}

			if (!suppressUpdate) {
				Update ();
			}
			if (manipulator is IActiveStateQueueManipulator<T> activeManipulator) {
				activeManipulator.StateChanged += Update;
			}
			return true;
		}

		/// <summary>
		/// Attempts to remove a <see cref="IStateQueueManipulator{T}"/> from the queue.<br></br>
		/// Will fail if the manipulator does not exist in the queue, or the value is <see langword="null"/>.
		/// </summary>
		/// <param name="manipulator"> The manipulator to remove. </param>
		/// <returns> Whether the manipulator was removed successfully. </returns>
		public bool RemoveManipulator (IStateQueueManipulator<T> manipulator) {
			return RemoveManipulator (manipulator, false);
		}

		/// <summary>
		/// Attempts to remove a <see cref="IStateQueueManipulator{T}"/> from the queue.<br></br>
		/// Will fail if the manipulator does not exist in the queue, or the value is <see langword="null"/>.
		/// </summary>
		/// <param name="manipulator"> The manipulator to remove. </param>
		/// <param name="suppressUpdate"> If true, the queue's current value will not automatically be updated. Useful if multiple manipulators are to be removed. </param>
		/// <returns> Whether the manipulator was removed successfully. </returns>
		public bool RemoveManipulator (IStateQueueManipulator<T> manipulator, bool suppressUpdate) {
			if (manipulator == null) {
				return true;
			}
			bool removed = false;
			for (int i = 0; i < manipulators.Count; i++) {
				if (!Equal (manipulator, manipulators[i])) {
					continue;
				}

				if (manipulators[i] is IActiveStateQueueManipulator<T> activeManipulator) {
					activeManipulator.StateChanged -= Update;
				}

				manipulators.RemoveAt (i);
				removed = true;
				break;
			}
			if (removed && !suppressUpdate) {
				Update ();
			}
			return removed;
		}

		/// <summary>
		/// Removes all instances matching the given <paramref name="manipulator"/> from the queue.
		/// </summary>
		/// <param name="manipulator"> The manipulator to remove. </param>
		/// <returns> The number of manipulators removed. </returns>
		public int RemoveManipulators (IStateQueueManipulator<T> manipulator) {
			if (manipulator == null) {
				return 0;
			}

			int removed = 0;

			for (int i = manipulators.Count - 1; i >= 0; i--) {
				if (!Equal(manipulator, manipulators[i])) {
					continue;
				}

				if (manipulators[i] is IActiveStateQueueManipulator<T> activeManipulator) {
					activeManipulator.StateChanged -= Update;
				}

				manipulators.RemoveAt (i);
				removed++;
				i--;
			}

			if (removed > 0) {
				Update ();
			}

			return removed;
		}

		/// <inheritdoc/>
		public void ClearManipulators () {
			if (manipulators.Count == 0)
				return;

			for (int i = 0; i < manipulators.Count; i++) {
				if (manipulators[i] is IActiveStateQueueManipulator<T> activeManipulator) {
					activeManipulator.StateChanged -= Update;
				}
				i--;
			}
			manipulators.Clear ();
			Update ();
		}

		/// <inheritdoc/>
		public void Update () {
			var current = Default;
			for (int i = manipulators.Count - 1; i >= 0; i--) {
				if (manipulators[i] != null) {
					manipulators[i].Update (Target, ref current);
				}
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

		public override string ToString () {
			if (Current == null) {
				return "";
			}
			return Current.ToString ();
		}

		public string ToString (bool includeStateHint) {
			return includeStateHint ? $"StateQueue<{typeof (T).FullName}>: {ToString ()}" : ToString ();
		}


		private bool Equal (IStateQueueManipulator<T> x, IStateQueueManipulator<T> y) {
			if (ManipulatorComparer != null) {
				return ManipulatorComparer.Equals (x, y);
			}
			return x == y;
		}


		public static implicit operator T (InterfaceStateQueue<T> queue) {
			if (queue == null) {
				return default;
			}
			return queue.Current;
		}
	}

	public interface IStateQueueTarget<T> {
		void StateChanged (StateQueueChangedArgs<T> args);
	}

	public interface IStateQueueManipulator<T> {
		int StateQueueOrder { get; }
		void Update (IStateQueueTarget<T> target, ref T state);
	}

	public interface IActiveStateQueueManipulator<T> : IStateQueueManipulator<T> {
		event Action StateChanged;
	}
}
