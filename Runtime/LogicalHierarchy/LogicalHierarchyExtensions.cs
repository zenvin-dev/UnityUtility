using System.Collections.Generic;

namespace Zenvin.Util.Hierarchy {
	/// <summary>
	/// Extension class for <see cref="ILogicalHierarchy{TParent}"/>.
	/// </summary>
	public static class LogicalHierarchyExtensions {
		/// <summary>
		/// Gets the root of any given <see cref="ILogicalHierarchy{TParent}"/>.<br></br>
		/// The root is determined by there being no further <see cref="ILogicalHierarchy{TParent}.LogicalParent"/>.<br></br>
		/// When a <paramref name="recursionTrap"/> is passed, the code will stop if it encounters the same object twice while traversing the hierarchy.
		/// </summary>
		/// <param name="depth"> How far the original <paramref name="target"/> was nested inside the hierarchy. </param>
		public static T GetRoot<T> (this T target, out int depth, HashSet<ILogicalHierarchy<T>> recursionTrap = null) where T : ILogicalHierarchy<T> {
			depth = 0;
			if (target == null)
				return target;

			T parent;
			while ((parent = target.LogicalParent) != null && (recursionTrap?.Add (parent) ?? true)) {
				target = parent;
				depth++;
			}
			return target;
		}

		/// <summary>
		/// Ascends the hierarchy of the given <see cref="ILogicalHierarchy{TParent}"/> until the current level is a <typeparamref name="TParent"/>.<br></br>
		/// When a <paramref name="recursionTrap"/> is passed, the code will stop if it encounters the same object twice while traversing the hierarchy.
		/// </summary>
		public static bool TryGetNearestParent<TParent> (this TParent target, out TParent parent, HashSet<ILogicalHierarchy<TParent>> recursionTrap)
			where TParent : ILogicalHierarchy<TParent> {

			parent = default;
			if (target == null)
				return false;

			var current = target;
			do {
				if (!(recursionTrap?.Add (current) ?? false)) {
					return false;
				}

				if (current is TParent p) {
					parent = p;
					return true;
				}
			} while ((current = current.LogicalParent) != null);

			return false;
		}

		/// <summary>
		/// Ascends the hierarchy of the given <see cref="ILogicalHierarchy{TParent}"/> and returns the <typeparamref name="TParent"/> closest to the root.<br></br>
		/// This will always ascend all the way to the root and thus is more costly than 
		/// <see cref="TryGetNearestParent{TParent}(TParent, out TParent, HashSet{ILogicalHierarchy{TParent}})"/>.<br></br>
		/// When a <paramref name="recursionTrap"/> is passed, the code will stop if it encounters the same object twice while traversing the hierarchy.
		/// </summary>
		public static bool TryGetTopmostParent<TParent> (this TParent target, out TParent parent, HashSet<ILogicalHierarchy<TParent>> recursionTrap = null)
			where TParent : ILogicalHierarchy<TParent> {

			parent = default;
			if (target == null)
				return false;

			var current = target;
			do {
				if (!(recursionTrap?.Add (current) ?? false)) {
					return false;
				}

				if (current is TParent p) {
					parent = p;
				}
			} while ((current = current.LogicalParent) != null);

			return parent != null;
		}

		/// <summary>
		/// Ascends the hierarchy of the given <see cref="ILogicalHierarchy{TParent}"/> and yields each level.<br></br>
		/// When a <paramref name="recursionTrap"/> is passed, the code will stop if it encounters the same object twice while traversing the hierarchy.
		/// </summary>
		public static IEnumerable<T> Ascend<T> (this T target, HashSet<T> recursionTrap = null) where T : ILogicalHierarchy<T> {
			if (target == null)
				yield break;

			var current = target;
			do {
				if (!(recursionTrap?.Add (current) ?? false))
					yield break;

				yield return current;
				current = current.LogicalParent;
			} while (current != null);
		}

		/// <summary>
		/// Returns the <paramref name="target"/>'s <see cref="ILogicalHierarchy{TParent}.LogicalParent"/>, 
		/// but only if that parent is not equal to the <paramref name="target"/> itself.
		/// </summary>
		public static T ParentNotSelf<T> (this ILogicalHierarchy<T> target) where T : ILogicalHierarchy<T> {
			var parent = target.LogicalParent;

			if (target == null || parent == null)
				return default;
			if (target.Equals (parent))
				return default;

			return parent;
		}

		/// <summary>
		/// Returns the <paramref name="target"/>'s <see cref="ILogicalHierarchy{TParent}.LogicalParent"/> if it has one, 
		/// otherwise the <paramref name="target"/> itself.
		/// </summary>
		public static T ParentOrSelf<T> (this T target) where T : ILogicalHierarchy<T> {
			if (target == null)
				return default;

			var parent = target.LogicalParent;
			if (parent == null)
				return target;

			return parent;
		}
	}
}
