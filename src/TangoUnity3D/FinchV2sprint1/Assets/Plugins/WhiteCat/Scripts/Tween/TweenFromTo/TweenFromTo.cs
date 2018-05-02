using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 在 from 和 to 之间插值的动画
	/// </summary>
	/// <typeparam name="T"> 插值的数据类型 </typeparam>
	public abstract partial class TweenFromTo<T> : Tweener.TweenAnimation
	{
		[SerializeField] protected T _from;
		[SerializeField] protected T _to;

		T _original;


		/// <summary>
		/// 当前状态
		/// </summary>
		public abstract T current
		{
			get;
			set;
		}


		/// <summary>
		/// 开始状态
		/// </summary>
		public virtual T from
		{
			get { return _from; }
			set { _from = value; }
		}


		/// <summary>
		/// 结束状态
		/// </summary>
		public virtual T to
		{
			get { return _to; }
			set { _to = value; }
		}


		// 记录当前状态
		public override void OnRecord()
		{
			_original = current;
		}


		// 恢复当前状态
		public override void OnRestore()
		{
			current = _original;
		}


		[ContextMenu("Set 'From' to current")]
		public void SetFromToCurrent()
		{
			from = current;
		}


		[ContextMenu("Set 'To' to current")]
		public void SetToToCurrent()
		{
			to = current;
		}


		[ContextMenu("Set current to 'From'")]
		public void SetCurrentToFrom()
		{
			current = from;
		}


		[ContextMenu("Set current to 'To'")]
		public void SetCurrentToTo()
		{
			current = to;
		}


		public virtual void Reset()
		{
			from = current;
			to = current;
        }

	} // class TweenFromTo

} // namespace WhiteCat.Tween