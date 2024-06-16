using System.Collections.Generic;

namespace Zenvin.Util.Hierarchy {
	/// <summary>
	/// Extension class for <see cref="ILogicalHierarchy{TParent}"/>.
	/// </summary>
	public static class LogicalHierarchyExtensions {
		/// <summary>
		/// Gets the root of any given <see cref="ILogicalHierarchy{TParent}"/>.<br></br>
		/// The root is determined by there being no further <see cref="ILogicalHierarchy{TParent}.LogicalParent"/>.
		/// </summary>
		/// <param name="depth"> How far the original <paramref name="target"/> was nested inside the hierarchy. </param>
		public static T GetRoot<T> (this T target, out int depth) where T : ILogicalHierarchy<T> {
			depth = 0;
			if (target == null)
				return target;

			T parent;
			while ((parent = target.LogicalParent) != null) {
				target = parent;
				depth++;
			}
			return target;
		}

		/// <summary>
		/// Ascends the hierarchy of the given <see cref="ILogicalHierarchy{TParent}"/> until the current level is a <typeparamref name="TParent"/>.
		/// </summary>
		public static bool TryGetNearestParent<TParent> (this TParent target, out TParent parent) where TParent : ILogicalHierarchy<TParent> {
			parent = default;
			if (target == null)
				return false;

			var current = target;
			do {
				if (current is TParent p) {
					parent = p;
					return true;
				}
			} while ((current = current.LogicalParent) != null);

			return false;
		}

		/// <summary>
		/// Ascends the hierarchy of the given <see cref="ILogicalHierarchy{TParent}"/> and returns the <typeparamref name="TParent"/> closest to the root.<br></br>
		/// This will always ascend all the way to the root and thus is more costly than <see cref="TryGetNearestParent{TParent}(TParent, out TParent)"/>.
		/// </summary>
		/// <typeparam name="TParent"></typeparam>
		/// <param name="target"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static bool TryGetTopmostParent<TParent> (this TParent target, out TParent parent) where TParent : ILogicalHierarchy<TParent> {
			parent = default;
			if (target == null)
				return false;

			var current = target;
			do {
				if (current is TParent p) {
					parent = p;
				}
			} while ((current = current.LogicalParent) != null);

			return parent != null;
		}

		/// <summary>
		/// Ascends the hierarchy of the given <see cref="ILogicalHierarchy{TParent}"/> and yields each level.
		/// </summary>
		public static IEnumerable<T> Ascend<T> (this T target) where T : ILogicalHierarchy<T> {
			if (target == null)
				yield break;

			var current = target;
			do {
				yield return current;
				current = current.LogicalParent;
			} while (current != null);
		}
	}
}
