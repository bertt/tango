using System;

namespace WhiteCat
{
	/// <summary>
	/// 树节点
	/// 在内部, 一个节点的子节点被组织为双向循环链表
	/// </summary>
	public class TreeNode<T>
	{
		TreeNode<T> _next;
		TreeNode<T> _prev;
		TreeNode<T> _parent;
		TreeNode<T> _firstChild;
		int _childCount;


		/// <summary>
		/// 节点包含的数据
		/// </summary>
		public T value;


		/// <summary>
		/// 构造一个包含默认数据的节点
		/// </summary>
		public TreeNode()
		{
			_next = this;
			_prev = this;
		}


		/// <summary>
		/// 构造一个初始化数据的节点
		/// </summary>
		public TreeNode(T value) : this()
		{
			this.value = value;
		}


		/// <summary>
		/// 同层级中后一个节点. 如果此节点是最后一个则返回 null
		/// </summary>
		public TreeNode<T> next
		{
			get
			{
				if (_parent == null || _next == _parent._firstChild)
				{
					return null;
				}
				return _next;
			}
		}


		/// <summary>
		/// 同层级中后一个循环节点. 该返回值永远不为 null
		/// </summary>
		public TreeNode<T> circularNext
		{
			get { return _next; }
		}


		/// <summary>
		/// 同层级中前一个节点. 如果此节点是第一个则返回 null
		/// </summary>
		public TreeNode<T> previous
		{
			get
			{
				if (_parent == null || this == _parent._firstChild)
				{
					return null;
				}
				return _prev;
			}
		}


		/// <summary>
		/// 同层级中前一个循环节点. 该返回值永远不为 null
		/// </summary>
		public TreeNode<T> circularPrevious
		{
			get { return _prev; }
		}


		/// <summary>
		/// 第一个子节点. 如果没有子节点返回 null
		/// </summary>
		public TreeNode<T> firstChild
		{
			get { return _firstChild; }
		}


		/// <summary>
		/// 最后一个子节点. 如果没有子节点返回 null
		/// </summary>
		public TreeNode<T> lastChild
		{
			get
			{
				return _firstChild == null ? null : _firstChild._prev;
			}
		}


		/// <summary>
		/// 父节点. 如果没有父节点返回 null
		/// </summary>
		public TreeNode<T> parent
		{
			get { return _parent; }
		}


		/// <summary>
		/// 子节点总数
		/// </summary>
		public int childCount
		{
			get { return _childCount; }
		}


		/// <summary>
		/// 深度. 一个根节点的深度是 0
		/// 此属性运算复杂度为 O(d), d 为节点深度
		/// </summary>
		public int depth
		{
			get
			{
				int value = 0;
				var node = _parent;

				while (node != null)
				{
					value++;
					node = node._parent;
				}

				return value;
			}
		}


		/// <summary>
		/// 根节点
		/// 此属性运算复杂度为 O(d), d 为节点深度
		/// </summary>
		public TreeNode<T> root
		{
			get
			{
				var node = this;

				while (node._parent != null)
				{
					node = node._parent;
				}

				return node;
			}
		}


		/// <summary>
		/// 是否为根节点
		/// </summary>
		public bool isRoot
		{
			get { return _parent == null; }
		}


		/// <summary>
		/// 是否为叶子节点
		/// </summary>
		public bool isLeaf
		{
			get { return _firstChild == null; }
		}


		/// <summary>
		/// 作为第一个子节点附着到一个父节点下
		/// 注意: 如果 parent 存在于当前节点为根的子树中, 那么此操作的结果是未定义的
		/// </summary>
		public void AttachFirst(TreeNode<T> parent)
		{
			InternalValidateAttaching(parent);

			if (parent._firstChild == null)
			{
				InternalAttachChildless(parent);
			}
			else
			{
				InternalAttachBefore(parent, parent._firstChild);
				parent._firstChild = this;
			}
		}


		/// <summary>
		/// 作为最后一个子节点附着到一个父节点下
		/// 注意: 如果 parent 存在于当前节点为根的子树中, 那么此操作的结果是未定义的
		/// </summary>
		public void AttachLast(TreeNode<T> parent)
		{
			InternalValidateAttaching(parent);

			if (parent._firstChild == null)
			{
				InternalAttachChildless(parent);
			}
			else
			{
				InternalAttachBefore(parent, parent._firstChild);
			}
		}


		/// <summary>
		/// 附着到一个父节点下的某个子节点之前, 如果该子节点是第一个子节点, 那么父节点的 firstChild 也会改变
		/// 注意: 如果 parent 存在于当前节点为根的子树中, 那么此操作的结果是未定义的
		/// </summary>
		public void AttachBefore(TreeNode<T> parent, TreeNode<T> next)
		{
			InternalValidateAttaching(parent);
			parent.InternalValidateChild(next);
			InternalAttachBefore(parent, next);

			if (parent._firstChild == next)
			{
				parent._firstChild = this;
			}
		}


		/// <summary>
		/// 附着到一个父节点下的某个子节点之后
		/// 注意: 如果 parent 存在于当前节点为根的子树中, 那么此操作的结果是未定义的
		/// </summary>
		public void AttachAfter(TreeNode<T> parent, TreeNode<T> previous)
		{
			InternalValidateAttaching(parent);
			parent.InternalValidateChild(previous);
			InternalAttachBefore(parent, previous._next);
		}


		/// <summary>
		/// 从父节点脱离
		/// </summary>
		public void DetachFromParent()
		{
			if (_parent != null)
			{
				if (_parent._firstChild == this)
				{
					_parent._firstChild = _next == this ? null : _next;
				}
				_parent._childCount--;

				_next._prev = _prev;
				_prev._next = _next;

				_parent = null;
				_next = this;
				_prev = this;
			}
		}


		/// <summary>
		/// 分离所有的子节点
		/// </summary>
		public void DetachAllChildren()
		{
			TreeNode<T> child;

			while (_childCount > 0)
			{
				child = _firstChild;
				_firstChild = child._next;

				child._parent = null;
				child._next = child;
				child._prev = child;

				_childCount--;
			}

			_firstChild = null;
		}


		/// <summary>
		/// 是否存在于某个节点为根的子树中. subtreeRoot 为 null 时会返回 false
		/// </summary>
		public bool IsInSubtreeOf(TreeNode<T> subtreeRoot)
		{
			var node = this;

			do
			{
				if (node == subtreeRoot) return true;
				node = node._parent;
			}
			while (node != null);

			return false;
		}


		/// <summary>
		/// 遍历子树 (包含自己)
		/// 注意: 在遍历过程中修改树的结构是未定义的行为
		/// </summary>
		public void TraverseSubtree(Action<TreeNode<T>> action)
		{
			action(this);

			if (_firstChild != null)
			{
				var node = _firstChild;
				do
				{
					node.TraverseSubtree(action);
					node = node._next;
				}
				while (node != _firstChild);
			}
		}


		/// <summary>
		/// 遍历所有父级节点 (包含自己)
		/// 注意: 在遍历过程中修改树的结构是未定义的行为
		/// </summary>
		public void TraverseParents(Action<TreeNode<T>> action)
		{
			var node = this;

			do
			{
				action(node);
				node = node._parent;
			}
			while (node != null);
		}


		/// <summary>
		/// 在子树中查找 (包含自己). 查找失败返回 null
		/// 注意: 在匹配方法中修改树的结构是未定义的行为
		/// </summary>
		public TreeNode<T> FindInSubtree(Predicate<TreeNode<T>> match)
		{
			if (match(this)) return this;

			if (_firstChild != null)
			{
				var node = _firstChild;
				TreeNode<T> result;

				do
				{
					result = node.FindInSubtree(match);
					if (result != null) return result;
					node = node._next;
				}
				while (node != _firstChild);
			}

			return null;
		}


		/// <summary>
		/// 在所有父级节点中查找 (包含自己). 查找失败返回 null
		/// 注意: 在匹配方法中修改树的结构是未定义的行为
		/// </summary>
		public TreeNode<T> FindInParents(Predicate<TreeNode<T>> match)
		{
			var node = this;

			do
			{
				if (match(node)) return node;
				node = node._parent;
			}
			while (node != null);

			return null;
		}


		#region Internal

		// 验证 Attach 操作是否合法
		void InternalValidateAttaching(TreeNode<T> parent)
		{
			if (_parent != null)
			{
				throw new InvalidOperationException("node is attached");
			}
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			//if (parent.IsInSubtreeOf(this))
			//{
			//	throw new InvalidOperationException("new parent is child of node");
			//}
		}


		// 验证一个节点是否为 parent 的子节点
		void InternalValidateChild(TreeNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node._parent != this)
			{
				throw new InvalidOperationException("node is not child of parent");
			}
		}


		// 附着到一个没有子节点的节点下
		void InternalAttachChildless(TreeNode<T> parent)
		{
			_parent = parent;
			parent._childCount++;
			parent._firstChild = this;
		}


		// 附着到一个父节点下的某个子节点之前
		void InternalAttachBefore(TreeNode<T> parent, TreeNode<T> next)
		{
			_parent = parent;
			_next = next;
			_prev = next._prev;

			parent._childCount++;

			next._prev._next = this;
			next._prev = this;
		}

		#endregion

	} // class TreeNode<T>

} // namespace WhiteCat.Collections