using UnityEngine;

namespace WhiteCat.Tween
{
	// Tweener
	public partial class Tweener
	{
		/// <summary>
		/// 插值动画组件基类
		/// </summary>
		[ExecuteInEditMode]
		public abstract partial class TweenAnimation : ScriptableComponentWithEditor
		{
			[SerializeField]
			[TweenerReference]
			Tweener _tweener;

			bool _attached = false;


			void Attach()
			{
				if (_attached)
				{
					if (!_tweener) _attached = false;
				}

				if (!_attached)
				{
					if (_tweener)
					{
						_tweener._animations.Add(this);
						_attached = true;
					}
				}
			}


			void Detach()
			{
				if (_attached)
				{
					if (_tweener) _tweener._animations.Remove(this);
					_attached = false;
				}
			}


			/// <summary>
			/// 附着的插值器对象
			/// </summary>
			public Tweener tweener
			{
				get { return _tweener; }
				set
				{
					if (_tweener != value)
					{
						Detach();

						_tweener = value;

						if (isActiveAndEnabled)
						{
							Attach();
						}

#if UNITY_EDITOR
						_lastTweener = value;
#endif
					}
				}
			}


			// 激活组件时添加动画到插值器
			void OnEnable()
			{
				Attach();
            }


			// 关闭组件时从插值器移除动画
			void OnDisable()
			{
				Detach();
            }


			/// <summary>
			/// 根据插值结果更新动画状态
			/// </summary>
			public abstract void OnTween(float factor);


			/// <summary>
			/// 记录当前的动画状态
			/// </summary>
			public abstract void OnRecord();


			/// <summary>
			/// 使用记录来恢复动画状态
			/// </summary>
			public abstract void OnRestore();

#if UNITY_EDITOR

			// 处理特殊情况下的数据错误 (比如 Undo Redo)

			Tweener _lastTweener;

			void OnValidate()
			{
				if (_lastTweener != _tweener)
				{
					var value = _tweener;
                    _tweener = _lastTweener;
					tweener = value;
                }
			}

#endif // UNITY_EDITOR

		} // class TweenAnimation

	} // class Tweener

} // namespace WhiteCat.Tween