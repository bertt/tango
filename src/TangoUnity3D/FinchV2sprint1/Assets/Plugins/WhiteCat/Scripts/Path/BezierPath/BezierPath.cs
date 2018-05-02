using System.Collections.Generic;
using UnityEngine;

namespace WhiteCat.Paths
{
	/// <summary>
	/// 贝塞尔路径
	/// </summary>
	[AddComponentMenu("White Cat/Path/Bezier Path")]
	public partial class BezierPath : Path
	{
		[SerializeField] List<Node> _localNodes;
		[SerializeField] List<float> _tensions;
		[SerializeField] bool _circular;


		// 根据节点更新样条
		void UpdateSegment(int segmentIndex)
		{
			var n0 = _localNodes[segmentIndex];
			var n1 = _localNodes[(segmentIndex + 1) % _localNodes.Count];

			SetLocalBezierSegment(
				segmentIndex,
				n0.middleControlPoint,
				n0.forwardControlPoint,
				n1.backControlPoint, 
				n1.middleControlPoint,
				_tensions[segmentIndex]);
		}


		// 插入节点
		void InsertLocalNode(int nodeIndex, Node node)
		{
			_localNodes.Insert(nodeIndex, node);

			if (_circular)
			{
				int prevIndex = nodeIndex - 1;

				if (nodeIndex == 0) prevIndex = _tensions.Count - 1;
                _tensions.Insert(nodeIndex, _tensions[prevIndex]);
				InsertSegment(nodeIndex);

				if (nodeIndex == 0) prevIndex = _tensions.Count - 1;
				UpdateSegment(prevIndex);
				UpdateSegment(nodeIndex);
			}
			else
			{
				if (nodeIndex == 0)
				{
					_tensions.Insert(nodeIndex, _tensions[0]);
					InsertSegment(nodeIndex);
					UpdateSegment(nodeIndex);
                }
				else if (nodeIndex == _localNodes.Count - 1)
				{
					_tensions.Insert(nodeIndex - 1, _tensions[nodeIndex - 2]);
					InsertSegment(nodeIndex - 1);
					UpdateSegment(nodeIndex - 1);
				}
				else
				{
					_tensions.Insert(nodeIndex, _tensions[nodeIndex - 1]);
					InsertSegment(nodeIndex);
					UpdateSegment(nodeIndex - 1);
					UpdateSegment(nodeIndex);
				}
			}
		}


		/// <summary>
		/// 初始化路径, 保证至少含有一个样条
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			_localNodes = new List<Node>(8);
			_tensions = new List<float>(8);
			_circular = false;

			Node node = new Node();
			_localNodes.Add(node);

			node = new Node();
			node.middleControlPoint = new Vector3(0, 0, 5f);
			_localNodes.Add(node);

			_tensions.Add(1f);

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
						UpdateSegment(count);
                    }
					else
					{
						_tensions.RemoveAt(count - 1);
						RemoveSegment(count - 1);
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
			if(_localNodes.Count > 2)
			{
				_localNodes.RemoveAt(nodeIndex);

				if (_circular)
				{
					_tensions.RemoveAt(nodeIndex);
					RemoveSegment(nodeIndex);
					UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
				}
				else
				{
					if (nodeIndex == segmentCount)
					{
						_tensions.RemoveAt(nodeIndex - 1);
						RemoveSegment(nodeIndex - 1);
					}
					else
					{
						_tensions.RemoveAt(nodeIndex);
						RemoveSegment(nodeIndex);
						if (nodeIndex > 0) UpdateSegment(nodeIndex - 1);
					}
				}
            }
		}


		/// <summary>
		/// 插入节点. 根据给定的参数初始化节点
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		public void InsertNode(int nodeIndex, Vector3 middleControlPoint, Vector3 forwardTangent, Vector3 backTangent, bool broken, Space space = Space.World)
		{
			Node node = new Node();
			node.broken = true;

			if (space == Space.World)
			{
				node.middleControlPoint = InverseTransformPoint(middleControlPoint);
				node.forwardTangent = InverseTransformVector(forwardTangent);
				node.backTangent = InverseTransformVector(backTangent);
			}
			else
			{
				node.middleControlPoint = middleControlPoint;
				node.forwardTangent = forwardTangent;
				node.backTangent = backTangent;
			}

			node.broken = broken;
			InsertLocalNode(nodeIndex, node);
        }


		/// <summary>
		/// 插入节点. 自动初始化节点数据
		/// </summary>
		public void InsertNode(int nodeIndex)
		{
			Node node = new Node();
			node.broken = true;

			if (_circular || (nodeIndex > 0 && nodeIndex < _localNodes.Count))
			{
				Location location = new Location(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1), 0.5f);
                node.middleControlPoint = GetPoint(location, Space.Self);
				Vector3 tangent = GetTangent(location, Space.Self);
                node.forwardTangent = tangent * _localNodes[nodeIndex % _localNodes.Count].backTangent.magnitude;
				node.backTangent = - tangent * _localNodes[location.index].forwardTangent.magnitude;
			}
			else
			{
				if (nodeIndex == 0)
				{
					node.middleControlPoint = _localNodes[0].middleControlPoint +
						(_localNodes[0].middleControlPoint - _localNodes[1].middleControlPoint).magnitude * _localNodes[0].backTangent.normalized;
					node.forwardTangent = -_localNodes[0].backTangent;
					node.backTangent = _localNodes[0].backTangent;
                }
				else
				{
					Node last = _localNodes[nodeIndex - 1];
                    node.middleControlPoint = last.middleControlPoint +
						(last.middleControlPoint - _localNodes[nodeIndex - 2].middleControlPoint).magnitude * last.forwardTangent.normalized;
					node.forwardTangent = last.forwardTangent;
					node.backTangent = -last.forwardTangent;
				}
			}

			node.broken = false;
			InsertLocalNode(nodeIndex, node);
		}


		/// <summary>
		/// 获取节点的中控制点位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 中控制点位置 </returns>
		public Vector3 GetMiddleControlPoint(int nodeIndex, Space space = Space.World)
		{
			if (space == Space.Self) return _localNodes[nodeIndex].middleControlPoint;
			else return TransformPoint(_localNodes[nodeIndex].middleControlPoint);
		}


		/// <summary>
		/// 设置节点的中控制点位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="position"> 中控制点位置 </param>
		public void SetMiddleControlPoint(int nodeIndex, Vector3 position, Space space = Space.World)
		{
			if (space == Space.World) position = InverseTransformPoint(position);
			_localNodes[nodeIndex].middleControlPoint = position;

			if (_circular)
			{
				UpdateSegment(nodeIndex);
				UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
			}
			else
			{
				if (nodeIndex != 0) UpdateSegment(nodeIndex - 1);
				if (nodeIndex != _localNodes.Count - 1) UpdateSegment(nodeIndex);
			}
        }


		/// <summary>
		/// 获取节点前方向切线
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 节点前方向切线 </returns>
		public Vector3 GetForwardTangent(int nodeIndex, Space space = Space.World)
		{
			if (space == Space.Self) return _localNodes[nodeIndex].forwardTangent;
			else return TransformVector(_localNodes[nodeIndex].forwardTangent);
		}


		/// <summary>
		/// 设置节点前方向切线
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="tangent"> 节点前方向切线 </param>
		public void SetForwardTangent(int nodeIndex, Vector3 tangent, Space space = Space.World)
		{
			if (space == Space.World) tangent = InverseTransformVector(tangent);
			_localNodes[nodeIndex].forwardTangent = tangent;

			if (_circular)
			{
				UpdateSegment(nodeIndex);
				if(!_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
			}
			else
			{
				if (nodeIndex != 0 && !_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex - 1);
				if (nodeIndex != _localNodes.Count - 1) UpdateSegment(nodeIndex);
			}
		}


		/// <summary>
		/// 获取节点后方向切线
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 节点后方向切线 </returns>
		public Vector3 GetBackTangent(int nodeIndex, Space space = Space.World)
		{
			if (space == Space.Self) return _localNodes[nodeIndex].backTangent;
			else return TransformVector(_localNodes[nodeIndex].backTangent);
		}


		/// <summary>
		/// 设置节点后方向切线
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="forwardTangent"> 节点后方向切线 </param>
		public void SetBackTangent(int nodeIndex, Vector3 tangent, Space space = Space.World)
		{
			if (space == Space.World) tangent = InverseTransformVector(tangent);
			_localNodes[nodeIndex].backTangent = tangent;

			if (_circular)
			{
				if (!_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex);
				UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
			}
			else
			{
				if (nodeIndex != 0) UpdateSegment(nodeIndex - 1);
				if (nodeIndex != _localNodes.Count - 1 && !_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex);
			}
		}


		/// <summary>
		/// 获取节点前方向控制点位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 节点前方向控制点位置 </returns>
		public Vector3 GetForwardControlPoint(int nodeIndex, Space space = Space.World)
		{
			if (space == Space.Self) return _localNodes[nodeIndex].forwardControlPoint;
			else return TransformPoint(_localNodes[nodeIndex].forwardControlPoint);
		}


		/// <summary>
		/// 设置节点前方向控制点位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="position"> 节点前方向控制点位置 </param>
		public void SetForwardControlPoint(int nodeIndex, Vector3 position, Space space = Space.World)
		{
			if (space == Space.World) position = InverseTransformPoint(position);
			_localNodes[nodeIndex].forwardControlPoint = position;

			if (_circular)
			{
				UpdateSegment(nodeIndex);
				if (!_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
			}
			else
			{
				if (nodeIndex != 0 && !_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex - 1);
				if (nodeIndex != _localNodes.Count - 1) UpdateSegment(nodeIndex);
			}
		}


		/// <summary>
		/// 获取节点后方向控制点位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 节点后方向控制点位置 </returns>
		public Vector3 GetBackControlPoint(int nodeIndex, Space space = Space.World)
		{
			if (space == Space.Self) return _localNodes[nodeIndex].backControlPoint;
			else return TransformPoint(_localNodes[nodeIndex].backControlPoint);
		}


		/// <summary>
		/// 设置节点后方向控制点位置
		/// </summary>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="position"> 节点后方向控制点位置 </param>
		public void SetBackControlPoint(int nodeIndex, Vector3 position, Space space = Space.World)
		{
			if (space == Space.World) position = InverseTransformPoint(position);
			_localNodes[nodeIndex].backControlPoint = position;

			if (_circular)
			{
				if (!_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex);
				UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
			}
			else
			{
				if (nodeIndex != 0) UpdateSegment(nodeIndex - 1);
				if (nodeIndex != _localNodes.Count - 1 && !_localNodes[nodeIndex].broken) UpdateSegment(nodeIndex);
			}
		}


		/// <summary>
		/// 节点是否为拐点
		/// </summary>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <returns> 节点是否为拐点 </returns>
		public bool IsNodeBroken(int nodeIndex)
		{
			return _localNodes[nodeIndex].broken;
		}


		/// <summary>
		/// 设置节点是否为拐点
		/// </summary>
		/// <param name="nodeIndex"> 节点索引 </param>
		/// <param name="broken"> 节点是否为拐点 </param>
		public void SetNodeBroken(int nodeIndex, bool broken)
		{
			_localNodes[nodeIndex].broken = broken;

			if (!broken)
			{
				if (_circular)
				{
					UpdateSegment(nodeIndex);
					UpdateSegment(nodeIndex == 0 ? (segmentCount - 1) : (nodeIndex - 1));
				}
				else
				{
					if (nodeIndex != 0) UpdateSegment(nodeIndex - 1);
					if (nodeIndex != _localNodes.Count - 1) UpdateSegment(nodeIndex);
				}
			}
        }

	} // class BezierPath

} // namespace WhiteCat.Paths