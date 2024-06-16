namespace Zenvin.Util.Hierarchy {
	/// <summary>
	/// An interface to represent a logical hierarchical order between arbitrary objects of a given type (<typeparamref name="TParent"/>).
	/// </summary>
	/// <typeparam name="TParent"></typeparam>
	public interface ILogicalHierarchy<TParent> where TParent : ILogicalHierarchy<TParent> {
		/// <summary>
		/// The logical parent of this object (if any).
		/// </summary>
		TParent LogicalParent { get; }
	}
}
