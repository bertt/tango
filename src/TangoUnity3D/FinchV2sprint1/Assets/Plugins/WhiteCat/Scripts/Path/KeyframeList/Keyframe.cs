using System;
using UnityEngine;

namespace WhiteCat.Paths
{
	public partial class Path
	{
		/// <summary>
		/// 路径关键帧
		/// </summary>
		[Serializable]
		public class Keyframe<KeyValue> where KeyValue : struct
		{
			public float position;
			public KeyValue value;

			public Keyframe() { }

			public Keyframe(float position, KeyValue value)
			{
				this.position = position;
				this.value = value;
			}

		} // class Keyframe<T>


		/// <summary>
		/// float 数据类型关键帧
		/// </summary>
		[Serializable]
		public class FloatKeyframe : Keyframe<float> { }


		/// <summary>
		/// Vector3 数据类型关键帧
		/// </summary>
		[Serializable]
		public class Vector3Keyframe : Keyframe<Vector3> { }


		/// <summary>
		/// Quaternion 数据类型关键帧
		/// </summary>
		[Serializable]
		public class QuaternionKeyframe : Keyframe<Quaternion> { }

	} // class Path

} // namespace WhiteCat.Paths