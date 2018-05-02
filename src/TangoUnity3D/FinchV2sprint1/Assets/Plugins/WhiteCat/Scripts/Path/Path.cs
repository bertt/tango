using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteCat.Paths
{
	/// <summary>
	/// 路径基类. 路径是 Cubic Spline 的有序组合
	/// </summary>
	[DisallowMultipleComponent]
	public abstract partial class Path : ScriptableComponentWithEditor
	{
		// 路径段. 一个路径段是一个三次样条
		[Serializable]
		class Segment : CubicSpline
		{
			public float pathLength;        // 从路径起点到当前段终点的路径长度
		}


		[SerializeField] float _worldScale;
		[SerializeField] float _localLengthError;
		[SerializeField] int _firstInvalidPathLengthIndex;
		[SerializeField] List<Segment> _localSegments;


		/// <summary>
		/// 路径是否首尾相接. 修改此属性同时也会添加或删除样条
		/// </summary>
		public abstract bool circular { get; set; }


		/// <summary>
		/// 世界空间缩放. 缩放不可为 0
		/// </summary>
		public float worldScale
		{
			get { return _worldScale; }
			set
			{
				if (value < 0f)
				{
					_worldScale = Mathf.Clamp(value, -Kit.Million, -Kit.OneMillionth);
				}
				else
				{
					_worldScale = Mathf.Clamp(value, Kit.OneMillionth, Kit.Million);
				}
			}
		}


		/// <summary>
		/// 世界缩放的绝对值
		/// </summary>
		public float absWorldScale
		{
			get { return _worldScale >= 0f ? _worldScale : -_worldScale; }
		}


		/// <summary>
		/// 将路径本地坐标转换为世界坐标
		/// 物体的缩放被忽略, 使用路径的 worldScale 替代
		/// </summary>
		/// <param name="localPoint"> 本地坐标 </param>
		/// <returns> 路径本地点对应的世界坐标 </returns>
		public Vector3 TransformPoint(Vector3 localPoint)
		{
			return transform.TransformDirection(_worldScale * localPoint) + transform.position;
		}


		/// <summary>
		/// 将路径本地向量转换为世界向量
		/// 物体的缩放被忽略, 使用路径的 worldScale 替代
		/// 转换前后向量长度可能发生变化
		/// </summary>
		/// <param name="localVector"> 本地向量 </param>
		/// <returns> 路径本地向量对应的世界向量 </returns>
		public Vector3 TransformVector(Vector3 localVector)
		{
			return transform.TransformDirection(_worldScale * localVector);
		}


		/// <summary>
		/// 将路径本地方向转换为世界方向
		/// 物体的缩放被忽略, 路径的 worldScale 符号影响转换结果
		/// 转换前后向量长度不变
		/// </summary>
		/// <param name="localDirection"> 本地方向 </param>
		/// <returns> 路径本地方向对应的世界方向 </returns>
		public Vector3 TransformDirection(Vector3 localDirection)
		{
			return transform.TransformDirection(Mathf.Sign(_worldScale) * localDirection);
		}


		/// <summary>
		/// 将路径本地旋转转换为世界旋转
		/// 路径的 worldScale 符号影响转换结果
		/// </summary>
		/// <param name="localRotation"> 本地旋转 </param>
		/// <returns> 本地旋转对应的世界旋转 </returns>
		public Quaternion TransformRotation(Quaternion localRotation)
		{
			if (_worldScale >= 0f) return transform.rotation * localRotation;
			else return transform.rotation * Quaternion.Inverse(localRotation);
		}


		/// <summary>
		/// 将世界坐标转换为路径本地坐标
		/// 物体的缩放被忽略, 使用路径的 worldScale 替代
		/// </summary>
		/// <param name="worldPoint"> 世界坐标 </param>
		/// <returns> 世界坐标对应的路径本地点 </returns>
		public Vector3 InverseTransformPoint(Vector3 worldPoint)
		{
			return transform.InverseTransformDirection(worldPoint - transform.position) / _worldScale;
		}


		/// <summary>
		/// 将世界向量转换为路径本地向量
		/// 物体的缩放被忽略, 使用路径的 worldScale 
		/// 转换前后向量长度可能发生变化
		/// </summary>
		/// <param name="worldVector"> 世界向量 </param>
		/// <returns> 世界向量对应的路径本地向量 </returns>
		public Vector3 InverseTransformVector(Vector3 worldVector)
		{
			return transform.InverseTransformDirection(worldVector) / _worldScale;
		}


		/// <summary>
		/// 将世界方向转换为路径本地方向
		/// 物体的缩放被忽略, 路径的 worldScale 符号影响转换结果
		/// 转换前后向量长度不变
		/// </summary>
		/// <param name="worldDirection"> 世界方向 </param>
		/// <returns> 世界方向对应的路径本地方向 </returns>
		public Vector3 InverseTransformDirection(Vector3 worldDirection)
		{
			return transform.InverseTransformDirection(worldDirection) * Mathf.Sign(_worldScale);
		}


		/// <summary>
		/// 将世界旋转转换为路径本地旋转
		/// 路径的 worldScale 符号影响转换结果
		/// </summary>
		/// <param name="worldRotation"> 世界旋转 </param>
		/// <returns> 世界旋转对应的本地旋转 </returns>
		public Quaternion InverseTransformRotation(Quaternion worldRotation)
		{
			if (_worldScale >= 0f) return Quaternion.Inverse(transform.rotation) * worldRotation;
			else return Quaternion.Inverse(Quaternion.Inverse(transform.rotation) * worldRotation);
		}


		// 如果路径未初始化 (比如在运行时代码中创建的路径), 在这里完成初始化
		void Awake()
		{
			if (Kit.IsNullOrEmpty(_localSegments)) Reset();
		}


		/// <summary>
		/// 重置 (初始化) 路径. 子类实现需保证至少添加一个路径段
		/// </summary>
		public virtual void Reset()
		{
			_worldScale = 1f;
			_localLengthError = 0.01f;
			_firstInvalidPathLengthIndex = 0;
			_localSegments = new List<Segment>(8);
		}


		/// <summary>
		/// 路径段总数
		/// </summary>
		public int segmentCount
		{
			get { return _localSegments.Count; }
		}


		/// <summary>
		/// 路径本地空间总长度
		/// </summary>
		public float localLength
		{
			get
			{
				int lastSegmentIndex = _localSegments.Count - 1;
				ValidatePathLength(lastSegmentIndex);
				return _localSegments[lastSegmentIndex].pathLength;
			}
		}


		/// <summary>
		/// 路径总长度
		/// </summary>
		public float length
		{
			get
			{
				int lastSegmentIndex = _localSegments.Count - 1;
				ValidatePathLength(lastSegmentIndex);
				return _localSegments[lastSegmentIndex].pathLength * absWorldScale;
			}
		}


		/// <summary>
		/// 路径本地空间长度误差
		/// </summary>
		public float localLengthError
		{
			get { return _localLengthError; }
			set
			{
				value = Mathf.Clamp(value, 0.001f, 1000f);

				if (value != _localLengthError)
				{
					_localLengthError = value;
					for (int i = 0; i < _localSegments.Count; i++)
					{
						_localSegments[i].lengthError = value;
					}
					_firstInvalidPathLengthIndex = 0;
				}
			}
		}


		/// <summary>
		/// 路径长度误差
		/// </summary>
		public float lengthError
		{
			get { return _localLengthError * absWorldScale; }
			set { localLengthError = value / absWorldScale; }
		}


		/// <summary>
		/// 获取路径上指定位置的点坐标
		/// </summary>
		/// <param name="location"> 路径位置 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <returns> 指定位置的点坐标 </returns>
		public Vector3 GetPoint(Location location, Space space = Space.World)
		{
			Vector3 point = _localSegments[location.index].GetPoint(location.time);
			if (space == Space.Self) return point;
			else return TransformPoint(point);
		}


		/// <summary>
		/// 获取路径上指定位置的一阶导数
		/// </summary>
		/// <param name="location"> 路径位置 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <returns> 指定位置的一阶导数 </returns>
		public Vector3 GetDerivative(Location location, Space space = Space.World)
		{
			Vector3 derivative = _localSegments[location.index].GetDerivative(location.time);
			if (space == Space.Self) return derivative;
			else return TransformVector(derivative);
		}


		/// <summary>
		/// 获取路径上指定位置的二阶导数
		/// </summary>
		/// <param name="location"> 路径位置 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <returns> 指定位置的二阶导数 </returns>
		public Vector3 GetSecondDerivative(Location location, Space space = Space.World)
		{
			Vector3 secondDerivative = _localSegments[location.index].GetSecondDerivative(location.time);
			if (space == Space.Self) return secondDerivative;
			else return TransformVector(secondDerivative);
		}


		/// <summary>
		/// 获取路径上指定位置的切线. 切线是单位向量, 如果切线不存在返回 Vector3.zero
		/// </summary>
		/// <param name="location"> 路径位置 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <returns> 指定位置的切线 </returns>
		public Vector3 GetTangent(Location location, Space space = Space.World)
		{
			Vector3 tangent = _localSegments[location.index].GetTangent(location.time);
			if (space == Space.Self) return tangent;
			else return TransformDirection(tangent);
		}


		/// <summary>
		/// 获取从路径起点到路径上指定位置的路径长度
		/// </summary>
		/// <param name="location"> 路径位置 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		public float GetLength(Location location, Space space = Space.World)
		{
			ValidatePathLength(location.index);
			float len = _localSegments[location.index].GetLength(location.time);
			if (location.index != 0)
			{
				len += _localSegments[location.index - 1].pathLength;
			}
			if (space == Space.Self) return len;
			else return len * absWorldScale;
		}


		/// <summary>
		/// 根据从路径起点开始的路径长度, 获取路径上的位置
		/// </summary>
		/// <param name="length"> 从路径起点开始的路径长度. 对于环状路径, 该长度可以为负值或大于路径长度的值 </param>
		/// <param name="startSegmentIndex"> 建议起始查找的路径段索引（负值表示无建议）</param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		/// <returns> 路径在指定长度处的位置 </returns>
		public Location GetLocationByLength(float length, int startSegmentIndex = -1, Space space = Space.World)
		{
			int lastSegmentIndex = _localSegments.Count - 1;
			ValidatePathLength(lastSegmentIndex);
			float totalLength = _localSegments[lastSegmentIndex].pathLength;

			if (space == Space.World) length /= absWorldScale;

			Location location = new Location();

			// 处理计算长度
			if (circular)
			{
				length = (totalLength + length % totalLength) % totalLength;
			}
			else
			{
				if (length <= 0)
				{
					return location;
				}

				if (length >= totalLength)
				{
					location.index = lastSegmentIndex;
					location.time = 1f;
					return location;
				}
			}

			// 如果建议开始索引无效, 则重新估算开始索引
			if (startSegmentIndex < 0 || startSegmentIndex > lastSegmentIndex)
			{
				location.index = (int)(length / totalLength * lastSegmentIndex);
			}
			else
			{
				location.index = startSegmentIndex;
			}

			// 查找样条索引, 并计算样条位置 t
			if (_localSegments[location.index].pathLength > length)
			{
				do
				{
					if (location.index == 0)
					{
						location.time = _localSegments[0].GetLocationByLength(length);
						return location;
					}

				} while (_localSegments[--location.index].pathLength > length);

				location.time = _localSegments[location.index + 1].GetLocationByLength(length - _localSegments[location.index].pathLength);
				location.index++;
			}
			else
			{
				while (_localSegments[++location.index].pathLength < length) ;

				location.time = _localSegments[location.index].GetLocationByLength(length - _localSegments[location.index - 1].pathLength);
			}

			return location;
		}


		/// <summary>
		/// 求路径上与给定点最近的位置
		/// </summary>
		/// <param name="point"> 给定点坐标 </param>
		/// <param name="stepLength"> 每分步长度 </param>
		/// <param name="space"> 参数或返回值的相对空间 </param>
		public Location GetClosestLocation(Vector3 point, float stepLength, Space space = Space.World)
		{
			if (space == Space.World)
			{
				point = InverseTransformPoint(point);
				stepLength /= absWorldScale;
			}

			float bestSqrMagnitude = float.MaxValue;
			float currentSqrMagnitude;
			float currentTime;

			Location location = new Location();
			Segment segment;

			for (int i = 0; i < _localSegments.Count; i++)
			{
				segment = _localSegments[i];
				currentTime = segment.GetClosestLocation(point, Mathf.CeilToInt(segment.length / stepLength));
				currentSqrMagnitude = (segment.GetPoint(currentTime) - point).sqrMagnitude;

				if (currentSqrMagnitude < bestSqrMagnitude)
				{
					location.index = i;
					location.time = currentTime;
					bestSqrMagnitude = currentSqrMagnitude;
				}
			}

			return location;
		}


		// 确保从路径起点到路径段终点的路径长度是有效的
		void ValidatePathLength(int segmentIndex)
		{
			for (; _firstInvalidPathLengthIndex <= segmentIndex; _firstInvalidPathLengthIndex++)
			{
				if (_firstInvalidPathLengthIndex == 0)
				{
					_localSegments[0].pathLength = _localSegments[0].length;
				}
				else
				{
					_localSegments[_firstInvalidPathLengthIndex].pathLength =
						_localSegments[_firstInvalidPathLengthIndex].length +
						_localSegments[_firstInvalidPathLengthIndex - 1].pathLength;
				}
			}
		}


		/// <summary>
		/// 路径长度采样是否有效
		/// </summary>
		public bool isSamplesValid
		{
			get { return _firstInvalidPathLengthIndex >= _localSegments.Count; }
		}


		/// <summary>
		/// 对路径长度采样
		/// </summary>
		public void ValidateSamples()
		{
			ValidatePathLength(_localSegments.Count - 1);
		}


		/// <summary>
		/// 清除路径长度采样
		/// </summary>
		public void InvalidateSamples()
		{
			for (int i = 0; i < _localSegments.Count; i++)
			{
				_localSegments[i].InvalidateSamples();
			}
			_firstInvalidPathLengthIndex = 0;
		}


		// 插入路径段
		protected void InsertSegment(int segmentIndex)
		{
			Segment segment = new Segment();
			segment.lengthError = _localLengthError;
			_localSegments.Insert(segmentIndex, segment);

			if (_firstInvalidPathLengthIndex > segmentIndex)
			{
				_firstInvalidPathLengthIndex = segmentIndex;
			}
		}


		// 移除路径段
		protected void RemoveSegment(int segmentIndex)
		{
			_localSegments.RemoveAt(segmentIndex);

			if (_firstInvalidPathLengthIndex > segmentIndex)
			{
				_firstInvalidPathLengthIndex = segmentIndex;
			}
		}


		// 设置贝塞尔路径段参数
		protected void SetLocalBezierSegment(int segmentIndex, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float tension)
		{
			_localSegments[segmentIndex].SetBezierCurve(p0, p1, p2, p3, tension);

			if (_firstInvalidPathLengthIndex > segmentIndex)
			{
				_firstInvalidPathLengthIndex = segmentIndex;
			}
		}


		// 设置基数路径段参数
		protected void SetLocalCardinalSegment(int segmentIndex, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float tension)
		{
			_localSegments[segmentIndex].SetCardinalCurve(p0, p1, p2, p3, tension);

			if (_firstInvalidPathLengthIndex > segmentIndex)
			{
				_firstInvalidPathLengthIndex = segmentIndex;
			}
		}

	} // class Path

} // namespace WhiteCat.Paths