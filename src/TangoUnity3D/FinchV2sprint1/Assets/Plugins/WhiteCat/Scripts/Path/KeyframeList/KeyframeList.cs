using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteCat.Paths
{
	public partial class Path
	{
		/// <summary>
		/// 路径关键帧列表
		/// </summary>
		// [RequireComponent(typeof(Path))]
		public abstract partial class KeyframeList : ScriptableComponentWithEditor
		{
			[SerializeField] Path _path;


			/// <summary>
			/// 路径
			/// </summary>
			public Path path
			{
				get { return _path; }
			}


			// 初始化
			void Awake()
			{
				// 运行时初始化通过脚本添加的组件
				if (!_path) Reset();
			}


			/// <summary>
			/// 关键帧是否排序
			/// </summary>
			public abstract bool isSorted { get; }


			/// <summary>
			/// 目标属性名称
			/// </summary>
			public abstract string targetPropertyName { get; }


			/// <summary>
			/// 目标组件类型
			/// </summary>
			public abstract Type targetComponentType { get; }


			/// <summary>
			/// 排序关键帧
			/// </summary>
			public abstract void Sort();


			/// <summary>
			/// 重置组件
			/// </summary>
			public virtual void Reset()
			{
				_path = GetComponent<Path>();
			}


			/// <summary>
			/// 更新移动对象, 由 MoveAlongPath 调用
			/// </summary>
			public abstract void UpdateMovingObject(KeyframeListTargetComponentPair pair, MoveAlongPath movingObject);

		} // KeyframeList


		/// <summary>
		/// 包含移动对象的路径关键帧列表
		/// </summary>
		public abstract partial class KeyframeList<KeyValue, Keyframe, Component> : KeyframeList
			where KeyValue : struct
			where Keyframe : Keyframe<KeyValue>, new()
			where Component : UnityEngine.Component
		{
			// 关键帧列表
			[SerializeField] List<Keyframe> _keyframes;

			// 关键帧是否排序
			[SerializeField] bool _sorted;


			// 关键帧默认值, 在初始化时调用
			protected abstract KeyValue defaultKeyValue { get; }

			// 线性插值方法
			protected abstract KeyValue Lerp(KeyValue from, KeyValue to, float t);

			// 应用插值结果
			protected abstract void Apply(Component target, KeyValue value, MoveAlongPath movingObject);


			/// <summary>
			/// 关键帧是否排序
			/// </summary>
			public sealed override bool isSorted
			{
				get { return _sorted; }
			}


			/// <summary>
			/// 目标组件类型
			/// </summary>
			public sealed override Type targetComponentType
			{
				get { return typeof(Component); }
			}


			/// <summary>
			/// 关键帧总数
			/// </summary>
			public int count
			{
				get { return _keyframes.Count; }
			}


			/// <summary>
			/// 重置组件
			/// </summary>
			public override void Reset()
			{
				base.Reset();

				_keyframes = new List<Keyframe>(4);
				Add(0f, defaultKeyValue);
				_sorted = true;
			}


			/// <summary>
			/// 排序关键帧
			/// </summary>
			public sealed override void Sort()
			{
				if (!_sorted)
				{
					_keyframes.Sort(
						(a, b) =>
						{
							if (a.position < b.position) return -1;
							if (a.position > b.position) return 1;
							return 0;
						}
					);

					_sorted = true;
				}
			}


			/// <summary>
			/// 添加关键帧
			/// </summary>
			public void Add(float position, KeyValue value)
			{
				Keyframe key = new Keyframe();
				key.position = Mathf.Max(0f, position);
				key.value = value;

				_keyframes.Add(key);
				_sorted = false;
			}


			/// <summary>
			/// 移除指定下标的关键帧
			/// </summary>
			public void RemoveAt(int index)
			{
				_keyframes.RemoveAt(index);
			}


			/// <summary>
			/// 获取关键帧位置
			/// </summary>
			public float GetPosition(int index)
			{
				return _keyframes[index].position;
			}


			/// <summary>
			/// 获取关键帧的值
			/// </summary>
			public KeyValue GetValue(int index)
			{
				return _keyframes[index].value;
			}


			/// <summary>
			/// 设置关键帧的位置
			/// </summary>
			public void SetPosition(int index, float position)
			{
				_keyframes[index].position = Mathf.Max(0f, position);
				_sorted = false;
			}


			/// <summary>
			/// 设置关键帧的值
			/// </summary>
			public void SetValue(int index, KeyValue value)
			{
				_keyframes[index].value = value;
			}


			/// <summary>
			/// 更新移动对象, 由 MoveAlongPath 调用
			/// </summary>
			public sealed override void UpdateMovingObject(KeyframeListTargetComponentPair pair, MoveAlongPath movingObject)
			{
				var path = base.path;
				int count = _keyframes.Count;
				float totalLength = path.length;
				float position = movingObject.distance;

				if (!_sorted) Sort();

				// 确定有效的 Key 数量
				while (_keyframes[count - 1].position > totalLength)
				{
					if (count == 1) return;
					count--;
				}

				// 将环形路径下的位置转化为线性路径下
				if (path.circular)
				{
					position = (totalLength + position % totalLength) % totalLength;
				}

				// 确定有效的查找起点
				int lastIndex = Mathf.Clamp(pair.lastIndex, 0, count - 1);

				// 向前查找
				while (lastIndex >= 0 && _keyframes[lastIndex].position > position)
				{
					lastIndex--;
				}

				// 向后查找
				if (lastIndex >= 0)
				{
					while (lastIndex < count && _keyframes[lastIndex].position <= position)
					{
						lastIndex++;
					}
				}
				else lastIndex = 0;

				pair.lastIndex = lastIndex;

				// 两端插值
				if (lastIndex == 0 || lastIndex == count)
				{
					// 环形路径两端插值
					if (path.circular)
					{
						float len = totalLength - _keyframes[count - 1].position;
						if (position <= _keyframes[0].position) position += len;
						else position -= _keyframes[count - 1].position;

						Apply(
							pair.targetComponent as Component,
							Lerp(_keyframes[count - 1].value, _keyframes[0].value, position / (len + _keyframes[0].position)),
							movingObject);
					}
					// 非环形路径两端插值
					else
					{
						if (lastIndex == 0) Apply(pair.targetComponent as Component, _keyframes[0].value, movingObject);
						else Apply(pair.targetComponent as Component, _keyframes[count - 1].value, movingObject);
					}
				}
				// 中间插值
				else
				{
					float t = _keyframes[lastIndex - 1].position;
					t = (position - t) / (_keyframes[lastIndex].position - t);

					Apply(
						pair.targetComponent as Component,
						Lerp(_keyframes[lastIndex - 1].value, _keyframes[lastIndex].value, t),
						movingObject);
				}
			}

		} // class KeyframeList<KeyValue, Keyframe>

	} // class Path

} // namespace WhiteCat.Paths