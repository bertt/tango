using System.Collections.Generic;
using UnityEngine;

namespace WhiteCat.Paths
{
	/// <summary>
	/// 基数样条路径
	/// </summary>
	[AddComponentMenu("White Cat/Path/Cardinal Path")]
	public partial class CardinalPath : Path
	{
		[SerializeField] List<Vector3> _localNodes;
		[SerializeField] List<float> _tensions;
		[SerializeField] bool _circular;


		// 根据节点更新样条
		void UpdateSegment(int segmentIndex)
		{
			int count = _localNodes.Count;
			int index0 = (segmentIndex == 0) ? (_circular ? (count - 1) : 0) : (segmentIndex - 1);
			int index3 = (segmentIndex == count - 2) ? (_circular ? 0 : (count - 1)) : ((segmentIndex + 2) % count);

			SetLocalCardinalSegment(
				segmentIndex,
				_localNodes[index0],
				_localNodes[segmentIndex],
				_localNodes[(segmentIndex + 1) % count],
				_localNodes[index3],
				_tensions[segmentIndex]);
		}


		// 插入节点
		void InsertLocalNode(int nodeIndex, Vector3 point)
		{
			_localNodes.Insert(nodeIndex, point);

			if (_circular)
			{
				int prevIndex = nodeIndex - 1;

				if (nodeIndex == 0) prevIndex = _tensions.Count - 1;
				_tensions.Insert(nodeIndex, _tensions[prevIndex]);
				InsertSegment(nodeIndex);

				int count = _tensions.Count;
				UpdateSegment((nodeIndex - 2 + count) % count);
				UpdateSegment((nodeIndex - 1 + count) % count);
				UpdateSegment(nodeIndex);
				UpdateSegment((nodeIndex + 1) % count);
			}
			else
			{
				if (nodeIndex == 0)
				{
					_tensions.Insert(0, _tensions[0]);
					InsertSegment(0);
					UpdateSegment(0);
					UpdateSegment(1);
				}
				else if (nodeIndex == _localNodes.Count - 1)
				{
					_tensions.Insert(nodeIndex - 1, _tensions[nodeIndex - 2]);
					InsertSegment(nodeIndex - 1);
					UpdateSegment(nodeIndex - 1);
					UpdateSegment(nodeIndex - 2);
				}
				else
				{
					_tensions.Insert(nodeIndex, _tensions[nodeIndex - 1]);
					InsertSegment(nodeIndex);
					if (nodeIndex > 1) UpdateSegment(nodeIndex - 2);
					if (nodeIndex > 0) UpdateSegment(nodeIndex - 1);
					UpdateSegment(nodeIndex);
					if (nodeIndex < _tensions.Count - 1) UpdateSegment(nodeIndex + 1);
				}
			}
		}


		/// <summary>
		/// 初始化路径, 保证至少含有一个样条
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			_localNodes = new List<Vector3>(8);
			_tensions = new List<float>(8);
			_circular = false;

			_localNodes.Add(Vector3.zero);
			_localNodes.Add(new Vector3(0f, 0f, 5f));

			_tensions.Add(0.5f);

			InsertSegment(0);
			UpdateSegment(0);
        }


		/// <summary>
		/// 路径是否首尾相接
		/// </summary>
		public override bool circular
		{
			get { return _circular; }
			set
			{
				if (_circular != value)
				{
					_circular = value;
					int count = segmentCount;

					if (value)
					{
						_tensions.Insert(count, _tensions[count - 1]);
						InsertSegment(count);
						UpdateSegment(0);
						UpdateSegment(count - 1);
						UpdateSegment(count);
					}
					else
					{
						_tensions.RemoveAt(count - 1);
						RemoveSegment(count - 1);
						UpdateSegment(0);
						UpdateSegment(count - 2);
					}
				}
			}
		}


		/// <summary>
		/// 获取路径段的张力
		/// </summary>
		public float GetTension(int segmentIndex)
		{
			return _tensions[segmentIndex];
		}


		/// <summary>
		/// 设置路径段的张力
		/// </summary>
		public void SetTension(int segmentIndex, float tension)
		{
			_tensions[segmentIndex] = Mathf.Clamp(tension, 0f, 1000f);
			UpdateSegment(segmentIndex);
		}


		/// <summary>
		/// 节点总数
		/// </summary>
		public int nodeCount
		{
			get { return _localNodes.Count; }
		}


		/// <summary>
		/// 移除节点. 节点数量超过 2 个的情况下才会执行
		/// </summary>
		public void RemoveNode(int nodeIndex)
		{
			if (_localNodes.Count > 2)
			{
				_localNodes.RemoveAt(nodeIndex);

				if (_circular)
				{
					_tensions.RemoveAt(nodeIndex);
					RemoveSegment(nodeIndex);

					int count = segmentCount;
					UpdateSegment((nodeIndex - 2 + count) % count);
					UpdateSegment((nodeIndex - 1 + count) % count);
					UpdateSegment(nodeIndex % count);
				}
				else
				{
					if (nodeIndex == segmentCount)
					{
						_tensions.RemoveAt(nodeIndex - 1);
						RemoveSegment(nodeIndex - 1);
						UpdateSegment(nodeIndex - 2);
					}
					else
					{
						_tensions.RemoveAt(nodeIndex);
						RemoveSegment(nodeIndex);
						if (nodeIndex < segmentCount) UpdateSegment(nodeIndex);
						if (nodeIndex > 0) UpdateSegment(nodeIndex - 1);
						if (nodeIndex > 1) UpdateSegment(nodeIndex - 2);
					}
				}
			}
		}


		/// <summary>
		/// 插入节点. 根据给定的参数初始化节点
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		public void InsertNode(int nodeIndex, Vector3 point, Space space = Space.World)
		{
			InsertLocalNode(nodeIndex, space == Space.Self ? point : InverseTransformPoint(point));
		}


		/// <summary>
		/// 插入节点. 自动初始化节点数据
		/// </summary>
		public void InsertNode(int nodeIndex)
		{
			Vector3 point;

			if (_circular)
			{
				point = GetPoint(new Location(nodeIndex == 0 ? (_tensions.Count - 1) : (nodeIndex - 1), 0.5f), Space.Self);
			}
			else
			{
				if (nodeIndex == 0)
				{
					point = _localNodes[0] - (_localNodes[0] - _localNodes[1]).magnitude * GetTangent(new Location(), Space.Self);
				}
				else if (nodeIndex == _localNodes.Count)
				{
					int lastIndex = nodeIndex - 1;
					point = _localNodes[lastIndex] + (_localNodes[lastIndex] - _localNodes[lastIndex - 1]).magnitude * GetTangent(new Location(lastIndex - 1, 1f), Space.Self);
				}
				else
				{
					point = GetPoint(new Location(nodeIndex - 1, 0.5f), Space.Self);
				}
			}

			InsertLocalNode(nodeIndex, point);
		}


		/// <summary>
		/// 获取节点的位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 节点的位置 </returns>
		public Vector3 GetNodePosition(int nodeIndex, Space space = Space.World)
		{
			if (space == Space.Self) return _localNodes[nodeIndex];
			else return TransformPoint(_localNodes[nodeIndex]);
		}


		/// <summary>
		/// 设置节点的位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="position"> 节点的位置 </param>
		public void SetNodePosition(int nodeIndex, Vector3 position, Space space = Space.World)
		{
			if (space == Space.World) position = InverseTransformPoint(position);
			_localNodes[nodeIndex] = position;

			int count = _tensions.Count;

			if (_circular)
			{
				UpdateSegment((nodeIndex - 2 + count) % count);
				UpdateSegment((nodeIndex - 1 + count) % count);
				UpdateSegment(nodeIndex);
				UpdateSegment((nodeIndex + 1) % count);
			}
			else
			{
				if (nodeIndex > 1) UpdateSegment(nodeIndex - 2);
				if (nodeIndex > 0) UpdateSegment(nodeIndex - 1);
				if (nodeIndex < count) UpdateSegment(nodeIndex);
				if (nodeIndex < count - 1) UpdateSegment(nodeIndex + 1);
			}
		}


		/// <summary>
		/// 通过一个 IList 来重新初始化路径的所有节点
		/// </summary>
		/// <param name="nodes"> 节点列表 </param>
		/// <param name="isCircular"> 路径是否首尾相接 </param>
		/// <param name="startIndex"> 第一个节点的下标 </param>
		/// <param name="count"> 节点总数, 非正值表示直到列表尾部 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <returns> 如果操作成功返回 true, 否则返回 false </returns>
		public bool SetNodes(IList<Vector3> nodes, bool isCircular, int startIndex = 0, int count = 0, Space space = Space.World)
		{
			if (count <= 0) count = nodes.Count;

			if (startIndex >= 0 && count >= 2 && startIndex + count <= nodes.Count)
			{
				int targetSegmentCount = isCircular ? count : count - 1;

				// 移除多余的路径段
				while (segmentCount > targetSegmentCount)
				{
					RemoveSegment(segmentCount - 1);
					_tensions.RemoveAt(segmentCount - 1);
				}

				// 重置已有的张力值
				for (int i=0; i<_tensions.Count; i++)
				{
					_tensions[i] = 0.5f;
				}

				// 添加缺少的路径段
				while (segmentCount < targetSegmentCount)
				{
					InsertSegment(segmentCount);
					_tensions.Add(0.5f);
				}

				_circular = isCircular;

				// 移除多余的节点
				if (_localNodes.Count > count)
				{
					_localNodes.RemoveRange(count, _localNodes.Count - count);
				}
				else
				{
					// 添加缺少的节点
					while (_localNodes.Count < count)
					{
						_localNodes.Add(Vector3.zero);
					}
				}

				// 复制节点
				for (int i=0; i<count; i++)
				{
					_localNodes[i] = space == Space.Self ? nodes[startIndex + i] : InverseTransformPoint(nodes[startIndex + i]);
				}

				// 更新所有路径段
				for (int i=0; i<segmentCount; i++)
				{
					UpdateSegment(i);
				}

				return true;
			}
			else return false;
		}

	} // class CardinalPath

} // namespace WhiteCat.Paths